using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MiraculousSurfers.Configuration;
using MiraculousSurfers.Economy;
using MiraculousSurfers.Gameplay.Events;
using MiraculousSurfers.Gameplay.UI;
using UnityEngine;
using VContainer;

namespace MiraculousSurfers.Gameplay.StateMachine.States
{
    public class TryEndedState : GameplayState
    {
        [Header("Presentation")]
        [SerializeField] private TryEndedScreenUiView _screenView;
        [SerializeField] private RewardsWidgetView _rewardsWidget;

        [Header("Timing")]
        [SerializeField] private float _grantDelaySeconds = 0.75f;

        [Header("Flow")]
        [SerializeField] private GameplayState _waitingState;

        [Header("Animation")]
        [SerializeField] private string _enterStateTrigger = "TryEndedState";
        [SerializeField] private string _celebrationType = "TryEndedState_CelebrationType";

        private IWalletService _wallet;
        private IGameConfigurationSystem _config;

        protected override GameplayStateType StateType => GameplayStateType.TryEnded;

        [Inject]
        public void InjectTryEnded(IWalletService wallet, IGameConfigurationSystem config)
        {
            _wallet = wallet;
            _config = config;
        }

        protected override async UniTask OnEnterAsync(CancellationToken ct)
        {
            var celebrationType = UnityEngine.Random.Range(0, 1) > 0.5 ? 1 : 2;
            CharacterAnimator.SetTrigger(_enterStateTrigger);
            CharacterAnimator.SetInteger(_celebrationType, celebrationType);

            var reward = _config.TryEndedReward(Character.TraveledDistance);
            _rewardsWidget.SetReward(reward);

            await _screenView.ShowAsync(ct);
            if (ct.IsCancellationRequested) return;

            await UniTask.Delay(TimeSpan.FromSeconds(_grantDelaySeconds), cancellationToken: ct).SuppressCancellationThrow();
            if (ct.IsCancellationRequested) return;

            Events.Publish(GameplayEventType.CoinsCountingStarted, new GameSignal());
            _wallet.AddCoins(reward).Forget();
            await _rewardsWidget.CountToZeroAsync(reward, ct);
            Events.Publish(GameplayEventType.CoinsCountingStopped, new GameSignal());
            if (ct.IsCancellationRequested) return;

            await _rewardsWidget.WaitForContinueAsync(ct);
            if (ct.IsCancellationRequested) return;

            Machine.GoTo(_waitingState).Forget();
        }

        protected override async UniTask OnExitAsync(CancellationToken ct)
        {
            await _screenView.HideAsync(ct);
        }
    }
}
