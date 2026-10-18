using System.Threading;
using Cysharp.Threading.Tasks;
using MiraculousSurfers.Gameplay.Camera;
using MiraculousSurfers.Gameplay.UI;
using MiraculousSurfers.Gameplay.Upgrades;
using UnityEngine;

namespace MiraculousSurfers.Gameplay.StateMachine.States
{
    public class WaitingState : GameplayState
    {
        [Header("Presentation")]
        [SerializeField] private OrbitalCameraRig _cameraRig;
        [SerializeField] private OrbitPreset _waitingPreset;
        [SerializeField] private HomeScreenUiView _homeScreenView;

        [Header("Flow")]
        [SerializeField] private GameplayState _aimState;

        private static readonly int EnterStateTrigger = Animator.StringToHash("WaitState");
        private static readonly int CelebrateTrigger = Animator.StringToHash("WaitState_UpgradePurchased");

        protected override GameplayStateType StateType => GameplayStateType.Wait;

        protected override async UniTask OnEnterAsync(CancellationToken ct)
        {
            if (Character != null) Character.ResetToSpawn();
            CharacterAnimator.SetTrigger(EnterStateTrigger);

            _cameraRig.Activate();
            await _cameraRig.AnimateToAsync(_waitingPreset, ct: ct);
            if (ct.IsCancellationRequested) return;

            Subscribe();
            
            await _homeScreenView.ShowAsync(ct);
        }

        protected override async UniTask OnExitAsync(CancellationToken ct)
        {
            if (_homeScreenView != null) await _homeScreenView.HideAsync(ct);
        }

        private void OnTapToPlay() => Machine.GoTo(_aimState).Forget();

        private void OnUpgradePurchased(UpgradeParam param)
        {
            CharacterAnimator.SetTrigger(CelebrateTrigger);
        }

        protected override void Subscribe()
        {
            if (_homeScreenView != null) _homeScreenView.TapToPlay += OnTapToPlay;
            if (Upgrades != null)
            {
                Upgrades.Changed -= OnUpgradePurchased;
                Upgrades.Changed += OnUpgradePurchased;
            }
        }

        protected override void Unsubscribe()
        {
            if (_homeScreenView != null) _homeScreenView.TapToPlay -= OnTapToPlay;
            if (Upgrades != null) Upgrades.Changed -= OnUpgradePurchased;
        }
    }
}
