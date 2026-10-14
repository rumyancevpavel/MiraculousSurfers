using System.Threading;
using Cysharp.Threading.Tasks;
using MiraculousSurfers.Gameplay.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MiraculousSurfers.Gameplay
{
    public class WaitingState : GameplayState
    {
        [Header("Presentation")]
        [SerializeField] private OrbitalCameraRig _cameraRig;
        [SerializeField] private OrbitalCameraRig.OrbitPreset _waitingPreset;
        [SerializeField] private GameObject _tapToBeginUi;
        [SerializeField] private HomeScreeUiView _homeScreeUiView;

        [Header("Input")]
        [SerializeField] private string _tapActionName = "Tap";

        [Header("Flow")]
        [SerializeField] private GameplayState _aimState;

        private InputAction _tap;

        protected override async UniTask OnEnterAsync(CancellationToken ct)
        {
            if (_tapToBeginUi != null) _tapToBeginUi.SetActive(false);

            _cameraRig.Activate();
            await _cameraRig.AnimateToAsync(_waitingPreset, ct: ct);
            if (ct.IsCancellationRequested) return;

            if (_tapToBeginUi != null) _tapToBeginUi.SetActive(true);
            EnableStateInput();
            _tap = GetAction(_tapActionName);
            if (_tap != null) _tap.performed += OnTap;
        }

        private void OnTap(InputAction.CallbackContext _)
        {
            if (_tapToBeginUi != null) _tapToBeginUi.SetActive(false);
            Machine.GoTo(_aimState).Forget();
        }

        protected override void Unsubscribe()
        {
            if (_tap != null) _tap.performed -= OnTap;
            _tap = null;
        }
    }
}
