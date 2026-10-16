using System.Threading;
using Cysharp.Threading.Tasks;
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

        [Header("Tuning")]
        [SerializeField] private float _minChargeToFire = 25f;

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
        }

        protected override async UniTask OnExitAsync(CancellationToken ct)
        {
            if (_aimScreenView != null) await _aimScreenView.HideAsync(ct);
        }

        private void OnBack() => Machine.GoTo(_waitingState).Forget();

        private void OnDragBegan()
        {
            _charge = 0f;
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
            if (_aimScreenView != null) _aimScreenView.SetCharge(charge);
        }

        private void OnDragEnded(float charge, Vector3 direction)
        {
            if (charge >= _minChargeToFire)
            {
                var power = charge * Upgrades.GetValue(UpgradeParam.LaunchPower);
                var speedMultiplier = Upgrades.GetValue(UpgradeParam.Speed);
                var steerMultiplier = Upgrades.GetValue(UpgradeParam.SteeringResponsiveness);
                _runningState.Configure(new LaunchParams(power, direction, speedMultiplier, steerMultiplier));
                Machine.GoTo(_runningState).Forget();
                return;
            }

            _charge = 0f;
            if (_aimScreenView != null)
            {
                _aimScreenView.SetChargingVisual(false);
                _aimScreenView.SetCharge(0f);
            }
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
