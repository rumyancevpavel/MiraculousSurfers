using System.Threading;
using Cysharp.Threading.Tasks;
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

        [Header("Input")]
        [SerializeField] private InputActionReference _tapAction;

        [Header("Flow")]
        [SerializeField] private GameplayState _aimState;

        protected override async UniTask OnEnterAsync(CancellationToken ct)
        {
            if (_tapToBeginUi != null) _tapToBeginUi.SetActive(false);

            _cameraRig.Activate();
            await _cameraRig.AnimateToAsync(_waitingPreset, ct: ct);
            if (ct.IsCancellationRequested) return;

            if (_tapToBeginUi != null) _tapToBeginUi.SetActive(true);
            EnableStateInput();
            if (_tapAction != null) _tapAction.action.performed += OnTap;
        }

        private void OnTap(InputAction.CallbackContext _)
        {
            if (_tapToBeginUi != null) _tapToBeginUi.SetActive(false);
            Machine.GoTo(_aimState).Forget();
        }

        protected override void Unsubscribe()
        {
            if (_tapAction != null) _tapAction.action.performed -= OnTap;
        }
    }
}
