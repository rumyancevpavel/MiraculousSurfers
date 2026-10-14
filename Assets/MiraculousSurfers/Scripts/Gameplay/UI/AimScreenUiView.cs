using System;
using UnityEngine;
using UnityEngine.UI;

namespace MiraculousSurfers.Gameplay.UI
{
    public class AimScreenUiView : UiScreenView
    {
        public event Action Back;

        [SerializeField] private Button _backButton;

        private void OnEnable() => _backButton.onClick.AddListener(OnBackButtonClick);

        private void OnDisable() => _backButton.onClick.RemoveListener(OnBackButtonClick);

        private void OnBackButtonClick() => Back?.Invoke();
    }
}
