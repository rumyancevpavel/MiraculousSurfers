using System.Threading;
using Cysharp.Threading.Tasks;
using MiraculousSurfers.Gameplay.UI;
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

        [Header("Refs")]
        [SerializeField] private Characters.Character _character;

        protected override async UniTask OnEnterAsync(CancellationToken ct)
        {
            if (_character != null) _character.ResetToSpawn();

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

        protected override void Subscribe()
        {
            if (_homeScreenView != null) _homeScreenView.TapToPlay += OnTapToPlay;
        }

        protected override void Unsubscribe()
        {
            if (_homeScreenView != null) _homeScreenView.TapToPlay -= OnTapToPlay;
        }
    }
}
