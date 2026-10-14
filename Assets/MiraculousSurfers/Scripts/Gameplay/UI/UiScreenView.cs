using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MiraculousSurfers.Gameplay.UI
{
    public class UiScreenView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private GameObject[] _elements;
        [SerializeField] private float _fadeDuration = 0.3f;

        protected virtual void Awake()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            SetElementsActive(false);
        }

        public async UniTask ShowAsync(CancellationToken ct = default)
        {
            gameObject.SetActive(true);
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            await FadeAsync(0f, 1f, ct);
            if (ct.IsCancellationRequested) return;
            SetElementsActive(true);
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        public async UniTask HideAsync(CancellationToken ct = default)
        {
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            SetElementsActive(false);
            await FadeAsync(_canvasGroup.alpha, 0f, ct);
            if (ct.IsCancellationRequested) return;
            gameObject.SetActive(false);
        }

        private async UniTask FadeAsync(float from, float to, CancellationToken ct)
        {
            if (_fadeDuration <= 0f)
            {
                _canvasGroup.alpha = to;
                return;
            }

            _canvasGroup.alpha = from;
            float t = 0f;
            while (t < _fadeDuration)
            {
                if (ct.IsCancellationRequested) return;
                t += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(from, to, Mathf.Clamp01(t / _fadeDuration));
                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            if (ct.IsCancellationRequested) return;
            _canvasGroup.alpha = to;
        }

        private void SetElementsActive(bool active)
        {
            if (_elements == null) return;
            foreach (var element in _elements)
                if (element != null) element.SetActive(active);
        }
    }
}
