using System.Collections.Generic;
using System.Globalization;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Overflow.View
{
    public class BattleUIView : MonoBehaviour, IView
    {
        //増加ボタン関連
        [SerializeField] private List<Button> addScoreButtons;
        [SerializeField] private List<float> addScoreValues;
        [SerializeField] private List<float> addCostValues;
        public List<float> AddScoreValues => addScoreValues;
        public List<float> AddCostValues => addCostValues;

        [SerializeField] private Text scoreText;
        [SerializeField] private Text scoreStockText;
        [SerializeField] private Text addScoreText;
        [SerializeField] private List<Text> addButtonValueTexts;

        //セーブ関連
        [SerializeField] private List<Button> saveButtons;
        [SerializeField] private Text autoSaveText;
        private bool isAutoSave;

        [SerializeField] private List<Button> menuButtons;

        //メニューチャンネル
        private Channel<int> _menuChannel;
        private ChannelWriter<int> MenuChannelWriter => _menuChannel.Writer;

        public ChannelReader<int> MenuChannelReader => _menuChannel.Reader;

        //スコア増加チャンネル
        private Channel<int> _addScoreChannel;
        private ChannelWriter<int> AddScoreChannelWriter => _addScoreChannel.Writer;
        public ChannelReader<int> AddScoreChannelReader => _addScoreChannel.Reader;

        public int ButtonValue => addScoreButtons.Count;

        //セーブチャンネル
        private Channel<int> _saveChannel;
        private ChannelWriter<int> SaveChannelWriter => _saveChannel.Writer;
        public ChannelReader<int> SaveChannelReader => _saveChannel.Reader;

        [SerializeField] private List<GameObject> uiViewList;

        private void Awake()
        {
            _addScoreChannel = Channel.CreateSingleConsumerUnbounded<int>();
            _menuChannel = Channel.CreateSingleConsumerUnbounded<int>();
            _saveChannel = Channel.CreateSingleConsumerUnbounded<int>();

            var token = this.GetCancellationTokenOnDestroy();

            for (var i = 0; i < addScoreButtons.Count; i++)
            {
                var i1 = i;
                addScoreButtons[i].OnClickAsAsyncEnumerable()
                    .Subscribe(x => { AddScoreChannelWriter.TryWrite(i1); }).AddTo(token);
            }

            for (var i = 0; i < menuButtons.Count; i++)
            {
                var i1 = i;
                menuButtons[i].OnClickAsAsyncEnumerable()
                    .Subscribe(x => { MenuChannelWriter.TryWrite(i1); }).AddTo(token);
            }

            for (var i = 0; i < saveButtons.Count; i++)
            {
                var i1 = i;
                saveButtons[i].OnClickAsAsyncEnumerable()
                    .Subscribe(x => { SaveChannelWriter.TryWrite(i1); }).AddTo(token);
            }
        }

        public void AutoSaveText()
        {
            if (isAutoSave)
            {
                autoSaveText.text = $"Auto Save (On)";
                isAutoSave = false;
            }
            else
            {
                autoSaveText.text = $"Auto Save (Off)";
                isAutoSave = true;
            }
        }

        /// <summary>
        /// スコア
        /// </summary>
        /// <param name="value"></param>
        public void ScoreValue(float value)
        {
            scoreText.text = $"Score :{value.ToString(CultureInfo.InvariantCulture)}";
        }

        /// <summary>
        /// 上限達成回数
        /// </summary>
        /// <param name="value"></param>
        public void ScoreStock(float value)
        {
            scoreStockText.text = $"Stock :{value.ToString(CultureInfo.InvariantCulture)}";
        }

        /// <summary>
        /// スコア増加量
        /// </summary>
        /// <param name="value"></param>
        public void AddScoreValue(float value)
        {
            addScoreText.text = $"Increment :{value.ToString(CultureInfo.InvariantCulture)}";
        }

        /// <summary>
        /// 増加ボタンを押した回数
        /// </summary>
        /// <param name="value"></param>
        /// <param name="x"></param>
        public void AddButtonValue(int value, int x)
        {
            addButtonValueTexts[value].text = $"{x}";
        }

        /// <summary>
        /// 表示画面変更
        /// </summary>
        /// <param name="t"></param>
        public void ScreenChange(int t)
        {
            switch (t)
            {
                case 0:
                    uiViewList[0].SetActive(true);
                    uiViewList[1].SetActive(false);
                    uiViewList[2].SetActive(false);
                    uiViewList[3].SetActive(false);
                    break;
                case 1:
                    uiViewList[0].SetActive(false);
                    uiViewList[1].SetActive(true);
                    uiViewList[2].SetActive(false);
                    uiViewList[3].SetActive(false);
                    break;
                case 2:
                    uiViewList[0].SetActive(false);
                    uiViewList[1].SetActive(false);
                    uiViewList[2].SetActive(true);
                    uiViewList[3].SetActive(false);
                    break;
                case 3:
                    uiViewList[0].SetActive(false);
                    uiViewList[1].SetActive(false);
                    uiViewList[2].SetActive(false);
                    uiViewList[3].SetActive(true);
                    break;
            }
        }
    }
}