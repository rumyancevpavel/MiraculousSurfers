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
        [SerializeField] private string _backActionName = "Back";
        [SerializeField] private string _dragStartActionName = "DragStart";

        [Header("Flow")]
        [SerializeField] private GameplayState _waitingState;
        [SerializeField] private GameplayState _chargeState;

        private InputAction _back;
        private InputAction _dragStart;

        protected override async UniTask OnEnterAsync(CancellationToken ct)
        {
            await _cameraRig.AnimateToAsync(_behindPreset, _sweepDuration, ct);
            if (ct.IsCancellationRequested) return;

            EnableStateInput();
            _back = GetAction(_backActionName);
            _dragStart = GetAction(_dragStartActionName);
            if (_back != null) _back.performed += OnBack;
            if (_dragStart != null) _dragStart.performed += OnDragStart;
        }

        private void OnBack(InputAction.CallbackContext _) => Machine.GoTo(_waitingState).Forget();

        private void OnDragStart(InputAction.CallbackContext _)
        {
            if (_chargeState != null) Machine.GoTo(_chargeState).Forget();
        }

        protected override void Unsubscribe()
        {
            if (_back != null) _back.performed -= OnBack;
            if (_dragStart != null) _dragStart.performed -= OnDragStart;
            _back = null;
            _dragStart = null;
        }
    }
}
