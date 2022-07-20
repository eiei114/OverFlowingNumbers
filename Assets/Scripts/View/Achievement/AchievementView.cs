using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Overflow.View.Effect;
using UnityEngine;
using UnityEngine.UI;

namespace Overflow.View.Achievement
{
    public class AchievementView : MonoBehaviour
    {
        [SerializeField] private List<AchievementElement> achievementButtons;

        [SerializeField] private List<string> achievementString;
        private readonly Color32 _validationColor = new Color32(255, 255, 255, 255);

        [SerializeField] private GameObject achievementDetailPrefab;

        private CanvasGroup achieveCanvas;
        [SerializeField] private Text detailText;
        [SerializeField] private float achieveDetailFadeTime;

        private void Awake()
        {
            achieveCanvas = achievementDetailPrefab.GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            var token = this.GetCancellationTokenOnDestroy();

            for (var i = 0; i < achievementButtons.Count; i++)
            {
                var i1 = i;
                achievementButtons[i].AchievementChannelReader
                    .ReadAllAsync()
                    .ForEachAsync(_ => AchievementDetail(achievementString[i1]), token);
            }
        }

        /// <summary>
        /// 条件をクリアしたら色が濃くなる
        /// </summary>
        /// <param name="t"></param>
        public void ValidationImage(int t)
        {
            achievementButtons[t].OnColorChange(_validationColor);
        }

        /// <summary>
        /// アチーブメントの画像を触れると取得条件が見れる
        /// </summary>
        /// <param name="str"></param>
        private async void AchievementDetail(string str)
        {
            detailText.text = str;
            switch (achievementDetailPrefab.activeSelf)
            {
                case true:
                    await achieveCanvas.FadeOut(achieveDetailFadeTime);
                    achievementDetailPrefab.SetActive(false);
                    break;
                case false:
                    achievementDetailPrefab.SetActive(true);
                    await achieveCanvas.FadeIn(achieveDetailFadeTime);
                    break;
            }
        }
    }
}