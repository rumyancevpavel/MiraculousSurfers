using System;
using UnityEngine;
using UnityEngine.UI;

namespace MiraculousSurfers.Gameplay.UI
{
    public class HomeScreenUiView : UiScreenView
    {
        public event Action TapToPlay;

        [SerializeField] private Button _tapToPlayButton;

        private void OnEnable() => _tapToPlayButton.onClick.AddListener(OnTapToPlayButtonClick);

        private void OnDisable() => _tapToPlayButton.onClick.RemoveListener(OnTapToPlayButtonClick);

        private void OnTapToPlayButtonClick() => TapToPlay?.Invoke();
    }
}
