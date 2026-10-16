using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MiraculousSurfers.Configuration;
using MiraculousSurfers.Gameplay.Upgrades;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MiraculousSurfers.Gameplay.UI
{
    public class UpgradeSkillCardView : MonoBehaviour
    {
        public event Action<UpgradeParam> UpgradeRequested;

        [SerializeField] private UpgradeParam _param;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private Image _iconImage;
        [SerializeField] private Slider _progressSlider;
        [SerializeField] private TMP_Text _progressText;
        [SerializeField] private TMP_Text _upgradePriceText;
        [SerializeField] private Button _upgradeButton;
        [SerializeField] private float _fillDuration = 0.35f;

        public UpgradeParam Param => _param;

        private CancellationTokenSource _fillCts;

        private void OnEnable()
        {
            if (_upgradeButton != null) _upgradeButton.onClick.AddListener(RaiseUpgradeRequested);
        }

        private void OnDisable()
        {
            if (_upgradeButton != null) _upgradeButton.onClick.RemoveListener(RaiseUpgradeRequested);
            CancelFill();
        }

        private void OnDestroy() => CancelFill();

        public void Bind(IGameConfigurationSystem configuration)
        {
            if (_nameText != null) _nameText.text = configuration.DisplayName(_param);
            if (_iconImage != null) _iconImage.sprite = configuration.Icon(_param);
        }

        public void SetProgress(int current, int max, int nextCost)
        {
            if (_progressText != null) _progressText.text = $"{current}/{max}";
            if (_upgradePriceText != null) _upgradePriceText.text = nextCost > 0 ? nextCost.ToString() : "MAX";
            AnimateSlider(current, max);
        }

        private void AnimateSlider(int current, int max)
        {
            if (_progressSlider == null) return;

            _progressSlider.minValue = 0f;
            _progressSlider.maxValue = max;

            CancelFill();
            _fillCts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
            FillAsync(_progressSlider.value, current, _fillCts.Token).Forget();
        }

        private async UniTaskVoid FillAsync(float from, float to, CancellationToken ct)
        {
            if (_fillDuration <= 0f || Mathf.Approximately(from, to) || !isActiveAndEnabled)
            {
                if (_progressSlider != null) _progressSlider.value = to;
                return;
            }

            var elapsed = 0f;
            while (elapsed < _fillDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / _fillDuration);
                if (_progressSlider != null) _progressSlider.value = Mathf.Lerp(from, to, t);
                if (await UniTask.Yield(PlayerLoopTiming.Update, ct).SuppressCancellationThrow()) return;
            }

            if (_progressSlider != null) _progressSlider.value = to;
        }

        private void RaiseUpgradeRequested()
        {
            UpgradeRequested?.Invoke(_param);
        }

        private void CancelFill()
        {
            _fillCts?.Cancel();
            _fillCts?.Dispose();
            _fillCts = null;
        }
    }
}
