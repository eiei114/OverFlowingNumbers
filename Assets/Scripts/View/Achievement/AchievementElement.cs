using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Overflow.View.Achievement
{
    public class AchievementElement : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Image achievementImage;

        private Channel<PointerEventData> _achievementClick;
        private ChannelWriter<PointerEventData> AchievementChannelWriter => _achievementClick.Writer;
        public ChannelReader<PointerEventData> AchievementChannelReader => _achievementClick.Reader;

        private void Awake()
        {
            _achievementClick = Channel.CreateSingleConsumerUnbounded<PointerEventData>();
        }

        /// <summary>
        /// 画像を押すと反応
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerDown(PointerEventData eventData)
        {
            AchievementChannelWriter.TryWrite(eventData);
            Debug.Log($"{this.gameObject}Click");
        }

        /// <summary>
        /// 画像を離すと反応
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerUp(PointerEventData eventData)
        {
            AchievementChannelWriter.TryWrite(eventData);
            Debug.Log($"{this.gameObject}Up");
        }

        /// <summary>
        /// 画像の色を変更
        /// </summary>
        /// <param name="color"></param>
        public void OnColorChange(Color color)
        {
            achievementImage.color = color;
        }
    }
}