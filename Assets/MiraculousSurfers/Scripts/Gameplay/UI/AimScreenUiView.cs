using System;
using UnityEngine;
using UnityEngine.UI;

namespace MiraculousSurfers.Gameplay.UI
{
    public class AimScreenUiView : UiScreenView
    {
        public event Action Back;

        [SerializeField] private Button _backButton;
        [SerializeField] private ChargeIndicatorView _chargeIndicator;

        private void OnEnable() => _backButton.onClick.AddListener(OnBackButtonClick);

        private void OnDisable() => _backButton.onClick.RemoveListener(OnBackButtonClick);

        private void OnBackButtonClick() => Back?.Invoke();

        public void SetChargingVisual(bool charging)
        {
            if (_backButton != null) _backButton.gameObject.SetActive(!charging);
            if (_chargeIndicator != null) _chargeIndicator.gameObject.SetActive(charging);
        }

        public void SetCharge(float charge)
        {
            if (_chargeIndicator != null) _chargeIndicator.SetCharge(charge);
        }
    }
}
