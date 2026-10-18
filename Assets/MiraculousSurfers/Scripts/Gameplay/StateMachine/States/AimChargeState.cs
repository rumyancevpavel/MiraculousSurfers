using System.Threading;
using Cysharp.Threading.Tasks;
using MiraculousSurfers.Gameplay.Camera;
using MiraculousSurfers.Gameplay.Characters;
using MiraculousSurfers.Gameplay.UI;
using MiraculousSurfers.Gameplay.Upgrades;
using UnityEngine;

namespace MiraculousSurfers.Gameplay.StateMachine.States
{
    public class AimChargeState : GameplayState
    {
        [Header("Presentation")]
        [SerializeField] private OrbitalCameraRig _cameraRig;
        [SerializeField] private OrbitPreset _behindPreset;
        [SerializeField] private float _sweepDuration = 0.45f;
        [SerializeField] private AnimationCurve _sweepEase;
        [SerializeField] private AimScreenUiView _aimScreenView;

        [Header("Input")]
        [SerializeField] private ChargeDragArea _chargeDragArea;

        [Header("Sling")]
        [SerializeField] private SlingView _sling;

        [Header("Animation")]
        [SerializeField] private string _enterStateTrigger = "AimChargeState";
        [SerializeField] private string _isChargingParam = "AimChargeState_IsCharging";
        
        [Header("Tuning")]
        [SerializeField] private float _minChargeToFire = 25f;
        [SerializeField] private float _maxPullDistance = 3f;

        [Header("Flow")]
        [SerializeField] private GameplayState _waitingState;
        [SerializeField] private RunningState _runningState;

        private float _charge;
        private Vector3 _direction = Vector3.forward;

        protected override GameplayStateType StateType => GameplayStateType.AimCharge;

        protected override async UniTask OnEnterAsync(CancellationToken ct)
        {
            await _cameraRig.AnimateToAsync(_behindPreset, _sweepDuration, ct, _sweepEase);
            if (ct.IsCancellationRequested) return;

            _charge = 0f;
            Subscribe();
            if (_aimScreenView != null)
            {
                _aimScreenView.SetChargingVisual(false);
                await _aimScreenView.ShowAsync(ct);
            }
            CharacterAnimator.SetTrigger(_enterStateTrigger);
            SetCharging(false);
            if (_sling != null) _sling.SetCharge(0f);
        }

        protected override UniTask OnExitAsync(CancellationToken ct)
        {
            SetCharging(false);
            if (_sling != null) _sling.SetCharge(0f);
            if (_aimScreenView != null) _aimScreenView.HideAsync(ct).Forget();
            return UniTask.CompletedTask;
        }

        private void OnBack()
        {
            SetCharging(false);
            ApplyPull(0f);
            Machine.GoTo(_waitingState).Forget();
        }

        private void OnDragBegan()
        {
            _charge = 0f;
            SetCharging(true);
            ApplyPull(0f);
            if (_aimScreenView != null)
            {
                _aimScreenView.SetChargingVisual(true);
                _aimScreenView.SetCharge(0f);
            }
        }

        private void OnDragChanged(float charge, Vector3 direction)
        {
            _charge = charge;
            _direction = direction;
            ApplyPull(charge);
            if (_aimScreenView != null) _aimScreenView.SetCharge(charge);
        }

        private void OnDragEnded(float charge, Vector3 direction)
        {
            SetCharging(false);

            if (charge >= _minChargeToFire)
            {
                var power = charge * Upgrades.GetValue(UpgradeParam.LaunchPower);
                var speedMultiplier = Upgrades.GetValue(UpgradeParam.Speed);
                var steerMultiplier = Upgrades.GetValue(UpgradeParam.SteeringResponsiveness);
                _runningState.Configure(new LaunchParams(power, direction, speedMultiplier, steerMultiplier));
                _runningState.Bind(Machine);
                _runningState.BeginRunningAnimation();
                Machine.GoTo(_runningState).Forget();
                return;
            }

            _charge = 0f;
            ApplyPull(0f);
            if (_aimScreenView != null)
            {
                _aimScreenView.SetChargingVisual(false);
                _aimScreenView.SetCharge(0f);
            }
        }

        private void SetCharging(bool value)
        {
            if (CharacterAnimator != null) CharacterAnimator.SetBool(_isChargingParam, value);
        }

        private void ApplyPull(float charge)
        {
            if (Character == null) return;
            var max = _chargeDragArea != null ? _chargeDragArea.MaxCharge : 0f;
            var normalized = max > 0f ? Mathf.Clamp01(charge / max) : 0f;
            Character.SetChargePull(normalized * _maxPullDistance);
            if (_sling != null) _sling.SetCharge(normalized);
        }

        protected override void Subscribe()
        {
            if (_aimScreenView != null) _aimScreenView.Back += OnBack;
            if (_chargeDragArea != null)
            {
                _chargeDragArea.DragBegan += OnDragBegan;
                _chargeDragArea.DragChanged += OnDragChanged;
                _chargeDragArea.DragEnded += OnDragEnded;
            }
        }

        protected override void Unsubscribe()
        {
            if (_aimScreenView != null) _aimScreenView.Back -= OnBack;
            if (_chargeDragArea != null)
            {
                _chargeDragArea.DragBegan -= OnDragBegan;
                _chargeDragArea.DragChanged -= OnDragChanged;
                _chargeDragArea.DragEnded -= OnDragEnded;
            }
        }
    }
}
