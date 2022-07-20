using UnityEngine;
using UnityEngine.UI;

namespace Overflow.View.UI
{
    public class SetCanvasScalerSize : MonoBehaviour
    {
        private CanvasScaler _canvasScaler;
        private RectTransform _canvas;

        [SerializeField] private GameObject addScores;

        private void Awake()
        {
            _canvasScaler = GetComponent<CanvasScaler>();
            _canvas = GetComponent<RectTransform>();
            
            var sizeDelta = _canvas.sizeDelta;
            _canvasScaler.referenceResolution = new Vector2(sizeDelta.x, sizeDelta.y);

            var acpect = 18 / 9;
            if (acpect==sizeDelta.y/sizeDelta.x)
            {
                addScores.GetComponent<RectTransform>().offsetMin = new Vector2(800, -957.3502f);
            }
        }
    }
}