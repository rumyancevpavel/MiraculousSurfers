using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MiraculousSurfers.Gameplay
{
    public class AimState : GameplayState
    {
        [Header("Presentation")]
        [SerializeField] private OrbitalCameraRig _cameraRig;
        [SerializeField] private OrbitalCameraRig.OrbitPreset _behindPreset;
        [SerializeField] private float _sweepDuration = 0.8f;

        [Header("Input")]
        [SerializeField] private InputActionReference _backAction;
        [SerializeField] private InputActionReference _dragStartAction;

        [Header("Flow")]
        [SerializeField] private GameplayState _waitingState;
        [SerializeField] private GameplayState _chargeState;

        protected override async UniTask OnEnterAsync(CancellationToken ct)
        {
            await _cameraRig.AnimateToAsync(_behindPreset, _sweepDuration, ct);
            if (ct.IsCancellationRequested) return;

            EnableStateInput();
            if (_backAction != null) _backAction.action.performed += OnBack;
            if (_dragStartAction != null) _dragStartAction.action.performed += OnDragStart;
        }

        private void OnBack(InputAction.CallbackContext _) => Machine.GoTo(_waitingState).Forget();
        private void OnDragStart(InputAction.CallbackContext _) => Machine.GoTo(_chargeState).Forget();

        protected override void Unsubscribe()
        {
            if (_backAction != null) _backAction.action.performed -= OnBack;
            if (_dragStartAction != null) _dragStartAction.action.performed -= OnDragStart;
        }
    }
}
