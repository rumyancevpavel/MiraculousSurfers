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

        [Header("Animation")]
        [SerializeField] private string _celebrateTrigger = "WaitState_UpgradePurchased";
        [SerializeField] private string _enterStateTrigger = "WaitState";
        
        [Header("Flow")]
        [SerializeField] private GameplayState _aimState;

        protected override GameplayStateType StateType => GameplayStateType.Wait;

        protected override async UniTask OnEnterAsync(CancellationToken ct)
        {
            if (Character != null) Character.ResetToSpawn();
            CharacterAnimator.SetTrigger(_enterStateTrigger);

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
            if (CharacterAnimator != null) CharacterAnimator.SetTrigger(_celebrateTrigger);
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
