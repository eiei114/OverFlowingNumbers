using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Overflow.View.Effect
{
    public static class EffectHandler
    {
        public static async UniTask<Tweener> FadeOut(this CanvasGroup canvasGroup, float duration)
        {
            return canvasGroup.DOFade(0.0F, duration);
        }

        public static async UniTask<Tweener> FadeIn(this CanvasGroup canvasGroup, float duration)
        {
            return canvasGroup.DOFade(1.0F, duration);
        }

    }
}