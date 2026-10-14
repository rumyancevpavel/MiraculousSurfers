using System.Threading;
using Cysharp.Threading.Tasks;
using MiraculousSurfers.Gameplay.Characters;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MiraculousSurfers.Gameplay
{
    public class ChargeState : GameplayState
    {
        [Header("Refs")]
        [SerializeField] private Character _character;

        [Header("Input")]
        [SerializeField] private InputActionReference _pointerPositionAction;
        [SerializeField] private InputActionReference _releaseAction;

        [Header("Tuning")]
        [SerializeField] private float _maxDrag = 300f;
        [SerializeField] private float _deadZone = 20f;
        [SerializeField] private float _fireThreshold = 0.25f;
        [SerializeField] private float _launchPower = 20f;

        [Header("Flow")]
        [SerializeField] private GameplayState _aimState;
        [SerializeField] private ShotState _shotState;

        private bool _active;
        private Vector2 _dragOrigin;
        private float _power;
        private Vector3 _direction = Vector3.forward;

        protected override UniTask OnEnterAsync(CancellationToken ct)
        {
            _pointerPositionAction?.action.Enable();
            _releaseAction?.action.Enable();
            _dragOrigin = ReadPointer();
            _power = 0f;
            Subscribe();
            _active = true;
            return UniTask.CompletedTask;
        }

        private void Update()
        {
            if (!_active) return;

            Vector2 current = ReadPointer();
            Vector2 delta = current - _dragOrigin;
            float dist = delta.magnitude;

            if (dist < _deadZone) { _power = 0f; return; }

            _power = Mathf.Clamp01((dist - _deadZone) / Mathf.Max(1f, _maxDrag - _deadZone));
            _direction = new Vector3(delta.x, 0f, Mathf.Abs(delta.y)).normalized;
        }

        private void OnRelease(InputAction.CallbackContext _)
        {
            _active = false;

            if (_power >= _fireThreshold)
            {
                _shotState.Configure(_power * _launchPower, _direction);
                Machine.GoTo(_shotState).Forget();
            }
            else
            {
                Machine.GoTo(_aimState).Forget();
            }
        }

        private Vector2 ReadPointer()
            => _pointerPositionAction != null ? _pointerPositionAction.action.ReadValue<Vector2>() : Vector2.zero;

        protected override void Subscribe()
        {
            if (_releaseAction != null) _releaseAction.action.performed += OnRelease;
        }

        protected override void Unsubscribe()
        {
            _active = false;
            if (_releaseAction != null) _releaseAction.action.performed -= OnRelease;
            _pointerPositionAction?.action.Disable();
            _releaseAction?.action.Disable();
        }
    }
}
