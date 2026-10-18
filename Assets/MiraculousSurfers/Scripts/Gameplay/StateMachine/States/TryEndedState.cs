using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MiraculousSurfers.Gameplay.Events;
using MiraculousSurfers.Gameplay.UI;
using UnityEngine;

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

        private static readonly int EnterStateTrigger = Animator.StringToHash("TryEndedState");
        private static readonly int CelebrationType = Animator.StringToHash("TryEndedState_CelebrationType");

        protected override GameplayStateType StateType => GameplayStateType.TryEnded;

        protected override async UniTask OnEnterAsync(CancellationToken ct)
        {
            var celebrationType = UnityEngine.Random.Range(0, 1) > 0.5 ? 1 : 2;
            CharacterAnimator.SetTrigger(EnterStateTrigger);
            CharacterAnimator.SetInteger(CelebrationType, celebrationType);

            var reward = Config.TryEndedReward(Character.TraveledDistance);
            _rewardsWidget.SetReward(reward);

            await _screenView.ShowAsync(ct);
            if (ct.IsCancellationRequested) return;

            await UniTask.Delay(TimeSpan.FromSeconds(_grantDelaySeconds), cancellationToken: ct).SuppressCancellationThrow();
            if (ct.IsCancellationRequested) return;

            Events.Publish(GameplayEventType.CoinsCountingStarted, new GameSignal());
            Wallet.AddCoins(reward).Forget();
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
