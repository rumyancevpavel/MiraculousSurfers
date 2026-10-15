using System;
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

        public UpgradeParam Param => _param;

        private void OnEnable()
        {
            if (_upgradeButton != null) _upgradeButton.onClick.AddListener(RaiseUpgradeRequested);
        }

        private void OnDisable()
        {
            if (_upgradeButton != null) _upgradeButton.onClick.RemoveListener(RaiseUpgradeRequested);
        }

        public void Bind(IGameConfigurationSystem configuration)
        {
            if (_nameText != null) _nameText.text = configuration.DisplayName(_param);
            if (_iconImage != null) _iconImage.sprite = configuration.Icon(_param);
        }

        public void SetProgress(int current, int max, int nextCost)
        {
            if (_progressSlider != null)
            {
                _progressSlider.minValue = 0f;
                _progressSlider.maxValue = max;
                _progressSlider.value = current;
            }

            if (_progressText != null) _progressText.text = $"{current}/{max}";
            if (_upgradePriceText != null) _upgradePriceText.text = nextCost > 0 ? nextCost.ToString() : "MAX";
        }

        private void RaiseUpgradeRequested()
        {
            UpgradeRequested?.Invoke(_param);
        }
    }
}
