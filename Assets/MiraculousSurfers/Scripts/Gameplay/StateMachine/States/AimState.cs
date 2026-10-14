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
        [SerializeField] private OrbitPreset _behindPreset;
        [SerializeField] private float _sweepDuration = 0.45f;
        [SerializeField] private AnimationCurve _sweepEase;

        [Header("Input")]
        [SerializeField] private InputActionAsset _inputAsset;
        [SerializeField] private string _stateMapName = "Aim";
        [SerializeField] private string _backActionName = "Back";
        [SerializeField] private string _dragStartActionName = "DragStart";

        [Header("Flow")]
        [SerializeField] private GameplayState _waitingState;
        [SerializeField] private GameplayState _chargeState;

        private InputAction _back;
        private InputAction _dragStart;

        protected override async UniTask OnEnterAsync(CancellationToken ct)
        {
            await _cameraRig.AnimateToAsync(_behindPreset, _sweepDuration, ct, _sweepEase);
            if (ct.IsCancellationRequested) return;

            EnableMap();
            Subscribe();
        }

        protected override UniTask OnExitAsync(CancellationToken ct)
        {
            DisableMap();
            return UniTask.CompletedTask;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            DisableMap();
        }

        private InputActionMap EnableMap()
        {
            _inputAsset?.Disable();
            var map = string.IsNullOrEmpty(_stateMapName) ? null : _inputAsset?.FindActionMap(_stateMapName, false);
            map?.Enable();
            return map;
        }

        private void DisableMap()
        {
            if (string.IsNullOrEmpty(_stateMapName)) return;
            _inputAsset?.FindActionMap(_stateMapName, false)?.Disable();
        }

        private void OnBack(InputAction.CallbackContext _) => Machine.GoTo(_waitingState).Forget();

        private void OnDragStart(InputAction.CallbackContext _)
        {
            if (_chargeState != null) Machine.GoTo(_chargeState).Forget();
        }

        protected override void Subscribe()
        {
            var map = string.IsNullOrEmpty(_stateMapName) ? null : _inputAsset?.FindActionMap(_stateMapName, false);
            _back = map?.FindAction(_backActionName, false);
            _dragStart = map?.FindAction(_dragStartActionName, false);
            if (_back != null) _back.performed += OnBack;
            if (_dragStart != null) _dragStart.performed += OnDragStart;
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
