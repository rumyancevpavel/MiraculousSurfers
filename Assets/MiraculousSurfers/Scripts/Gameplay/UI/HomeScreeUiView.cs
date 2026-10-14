using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace MiraculousSurfers.Gameplay.UI
{
    public class HomeScreeUiView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Button _tapToPlayButton;

        private void OnEnable()
        {
            _tapToPlayButton.onClick.AddListener(OnTapToPlayButtonClick);
        }

        private void OnDisable()
        {
            _tapToPlayButton.onClick.RemoveListener(OnTapToPlayButtonClick);
        }

        public async UniTask ShowAsync()
        {
        }

        public async UniTask HideAsync()
        {
        }

        private void OnTapToPlayButtonClick()
        {
            Debug.Log("OnTapToPlayButtonClick");
        }
    }
}