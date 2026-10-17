using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MiraculousSurfers.Gameplay.UI
{
    public class RewardsWidgetView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _rewardText;
        [SerializeField] private Button _continueButton;
        [SerializeField] private float _countDuration = 2.25f;

        public void SetReward(int amount)
        {
            SetText(amount);
            if (_continueButton != null) _continueButton.gameObject.SetActive(false);
        }

        public async UniTask CountToZeroAsync(int from, CancellationToken ct)
        {
            if (_rewardText == null) return;

            if (_countDuration <= 0f)
            {
                SetText(0);
                return;
            }

            var elapsed = 0f;
            while (elapsed < _countDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / _countDuration);
                SetText(Mathf.RoundToInt(Mathf.Lerp(from, 0f, t)));
                if (await UniTask.Yield(PlayerLoopTiming.Update, ct).SuppressCancellationThrow()) return;
            }

            SetText(0);
        }

        public async UniTask WaitForContinueAsync(CancellationToken ct)
        {
            if (_continueButton == null) return;

            _continueButton.gameObject.SetActive(true);

            var tcs = new UniTaskCompletionSource();
            UnityAction handler = () => tcs.TrySetResult();
            _continueButton.onClick.AddListener(handler);
            try
            {
                await tcs.Task.AttachExternalCancellation(ct);
            }
            finally
            {
                _continueButton.onClick.RemoveListener(handler);
            }
        }

        private void SetText(int value)
        {
            if (_rewardText != null) _rewardText.text = value.ToString();
        }
    }
}
