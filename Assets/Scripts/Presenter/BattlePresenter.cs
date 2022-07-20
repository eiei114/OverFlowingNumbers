using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Overflow.Model;
using Overflow.Model.Save;
using Overflow.View;
using Overflow.View.Achievement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Overflow.Presenter
{
    public class BattlePresenter : MonoBehaviour
    {
        [SerializeField] private GameObject _uiView;
        [SerializeField] private AchievementView achievementView;
        [SerializeField] private ScoreData _score;
        [SerializeField] private SaveScore _saveScore;

        private IView _view;

        [SerializeField] private float interval = 0.2f;
        [SerializeField] private float autoSaveInterval = 1.0f;
        private bool isAutoSave = true;

        [SerializeField] private float incrementalValue;

        private int scoreAchieveNum = 0;
        private int stockAchieveNum = 0;

        private const string DataIdentity = "ScoreData";
        private CancellationToken token;

        private void Awake()
        {
            _view = _uiView.GetComponent<IView>();
            token = this.GetCancellationTokenOnDestroy();

            for (var i = 0; i < _view.ButtonValue; i++)
            {
                _score.AddButtonValue.Add(new AsyncReactiveProperty<int>(0));
            }

            foreach (var t in _score.AddButtonValue)
            {
                _saveScore.addButtonValue.Add(t.Value);
            }

            //セーブデータロード
            DataInitLoad();
        }

        private void Start()
        {
            //スコア増加量に変更を加える
            _view.AddScoreChannelReader
                .ReadAllAsync()
                .ForEachAsync(value =>
                {
                    var cost = _view.AddCostValues[value];
                    var increment = _view.AddScoreValues[value];

                    Debug.Log($"{cost}");

                    if (_score.Score.Value > cost)
                    {
                        _score.Score.Value -= cost;
                        _score.IncrementValue.Value += increment;
                        _score.AddButtonValue[value].Value += 1;
                        _score.AddButtonComprehensiveValue.Value += 1;
                    }
                }, token);

            //アンダーメニューバー
            _view.MenuChannelReader
                .ReadAllAsync()
                .ForEachAsync(value => { _view.ScreenChange(value); }, token);
            //セーブ
            _view.SaveChannelReader
                .ReadAllAsync()
                .ForEachAsync(SaveSwitch, token);
            //UI更新
            _score.Score
                .Subscribe(value =>
                {
                    _view.ScoreValue(value);
                    ScoreAchievement();
                }).AddTo(token);

            _score.StockScore
                .Subscribe(value =>
                {
                    _view.ScoreStock(value);
                    StockScoreAchievement();
                }).AddTo(token);

            _score.IncrementValue
                .Subscribe(value =>
                {
                    _view.AddScoreValue(value);
                    AddButtonAchievement();
                }).AddTo(token);

            for (var i = 0; i < _view.ButtonValue; i++)
            {
                var i1 = i;
                _score.AddButtonValue[i].Subscribe(value => { _view.AddButtonValue(i1, value); }).AddTo(token);
            }

            UniTaskAsyncEnumerable.Interval(TimeSpan.FromSeconds(interval))
                .Subscribe(_ => { AddScore(); }, token);

            UniTaskAsyncEnumerable
                .Interval(TimeSpan.FromSeconds(autoSaveInterval))
                .Where(_ => isAutoSave)
                .Subscribe(async _ =>
                {
                    await SetSaveScore(token);
                    SaveHandler.Save(DataIdentity, _saveScore);
                }, token);
        }

        /// <summary>
        /// セーブ一覧の機能発動
        /// </summary>
        /// <param name="x"></param>
        private async void SaveSwitch(int x)
        {
            switch (x)
            {
                case 0:
                    await SetSaveScore(token);
                    SaveHandler.Save(DataIdentity, _saveScore);
                    Debug.Log($"セーブしました。");
                    break;
                case 1:
                    _view.AutoSaveText();
                    if (isAutoSave)
                    {
                        isAutoSave = false;
                    }
                    else
                    {
                        isAutoSave = true;
                    }

                    break;
                case 2:
                    SaveHandler.DeleteSaveData();
                    SceneManager.LoadScene("Main");
                    break;
            }
        }

        /// <summary>
        /// 値をセーブできる型に移し替える
        /// </summary>
        private async UniTask SetSaveScore(CancellationToken token = default)
        {
            _saveScore.score = _score.Score.Value;
            _saveScore.stockScore = _score.StockScore.Value;
            _saveScore.incrementValue = _score.IncrementValue.Value;
            _saveScore.addButtonComprehensiveValue = _score.AddButtonComprehensiveValue.Value;

            for (var i = 0; i < _score.AddButtonValue.Count; i++)
            {
                _saveScore.addButtonValue[i] = _score.AddButtonValue[i].Value;
            }
        }

        private async UniTask GetSaveScore()
        {
            var data = SaveHandler.Load<SaveScore>(DataIdentity);
            _score.Score.Value = data.score;
            _score.StockScore.Value = data.stockScore;
            _score.IncrementValue.Value = data.incrementValue;
            _score.AddButtonComprehensiveValue.Value = data.addButtonComprehensiveValue;

            for (var i = 0; i < data.addButtonValue.Count; i++)
            {
                _score.AddButtonValue[i].Value = data.addButtonValue[i];
            }
        }

        /// <summary>
        /// 初期データ有効化
        /// </summary>
        private async void DataInitLoad()
        {
            if (SaveHandler.ExitSaveData(DataIdentity))
            {
                await GetSaveScore();
                Debug.Log($"データがあるのでロードします。");
            }
            else
            {
                _score.IncrementValue.Value = incrementalValue;
            }
        }

        /// <summary>
        /// 得点を追加
        /// </summary>
        private void AddScore()
        {
            if (_score.Score.Value <= 3.4E+38f)
            {
                _score.Score.Value += _score.IncrementValue;
            }
            else
            {
                _score.Score.Value = 0.0f;

                _score.StockScore.Value += 1;
            }

            if (_score.StockScore.Value >= 2147483647)
            {
                _score.StockScore.Value = 0;
            }

            Debug.Log($"加点しました。");
        }


        /// <summary>
        /// スコア実績マーク有効化
        /// </summary>
        private void ScoreAchievement()
        {
            //スコア実績
            var score = _score.Score.Value;
            if (score >= 1 && scoreAchieveNum == 0)
            {
                achievementView.ValidationImage(0);
                scoreAchieveNum += 1;
            }
            else if (score >= 1000 && scoreAchieveNum == 1)
            {
                achievementView.ValidationImage(1);
                scoreAchieveNum += 1;
            }
            else if (score >= 300000 && scoreAchieveNum == 2)
            {
                achievementView.ValidationImage(2);
                scoreAchieveNum += 1;
            }
            else if (score >= 7000000 && scoreAchieveNum == 3)
            {
                achievementView.ValidationImage(3);
                scoreAchieveNum += 1;
            }
            else if (score >= 500000000 && scoreAchieveNum == 4)
            {
                achievementView.ValidationImage(4);
                scoreAchieveNum += 1;
            }
            else if (score >= 10000000000 && scoreAchieveNum == 5)
            {
                achievementView.ValidationImage(5);
                scoreAchieveNum += 1;
            }
            else if (score >= 300000000000 && scoreAchieveNum == 6)
            {
                achievementView.ValidationImage(6);
                scoreAchieveNum += 1;
            }
            else if (score >= 1000000000000 && scoreAchieveNum == 7)
            {
                achievementView.ValidationImage(7);
                scoreAchieveNum += 1;
            }
            else if (score >= 50000000000000 && scoreAchieveNum == 8)
            {
                achievementView.ValidationImage(8);
                scoreAchieveNum += 1;
            }
            else if (score >= 700000000000000 && scoreAchieveNum == 9)
            {
                achievementView.ValidationImage(9);
                scoreAchieveNum += 1;
            }
            else if (score >= 700000000000000000 && scoreAchieveNum == 9)
            {
                achievementView.ValidationImage(10);
                scoreAchieveNum += 1;
            }
            else if (score >= 3.4E+38f && scoreAchieveNum == 9)
            {
                achievementView.ValidationImage(11);
                scoreAchieveNum += 1;
            }
        }

        /// <summary>
        /// ストック実績マーク有効化
        /// </summary>
        private void StockScoreAchievement()
        {
            var stockScore = _score.StockScore.Value;
            if (stockScore >= 1000 && stockAchieveNum == 0)
            {
                achievementView.ValidationImage(13);
                stockAchieveNum += 1;
            }
            else if (stockScore >= 1000000 && stockAchieveNum == 1)
            {
                achievementView.ValidationImage(14);
                stockAchieveNum += 1;
            }
            else if (stockScore >= 1000000000 && stockAchieveNum == 2)
            {
                achievementView.ValidationImage(15);
                stockAchieveNum += 1;
            }
            else if (stockScore >= 2147483647 && stockAchieveNum == 3)
            {
                achievementView.ValidationImage(16);
                stockAchieveNum += 1;
            }
        }

        /// <summary>
        /// 追加ボタン押した回数実績有効化
        /// </summary>
        private void AddButtonAchievement()
        {
            var addScore = _score.AddButtonComprehensiveValue.Value;
            if (addScore >= 1000)
            {
                achievementView.ValidationImage(12);
            }
        }

        /// <summary>
        /// アプリケーション一時停止時
        /// </summary>
        /// <param name="pauseStatus"></param>
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                SaveHandler.Save(DataIdentity, _saveScore);
            }
        }
    }
}