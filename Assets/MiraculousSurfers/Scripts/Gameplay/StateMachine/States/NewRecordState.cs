using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MiraculousSurfers.Gameplay.Events;
using MiraculousSurfers.Gameplay.UI;
using UnityEngine;

namespace MiraculousSurfers.Gameplay.StateMachine.States
{
    public class NewRecordState : GameplayState
    {
        [Header("Presentation")]
        [SerializeField] private NewRecordScreenUiView _screenView;
        [SerializeField] private RewardsWidgetView _rewardsWidget;

        [Header("Timing")]
        [SerializeField] private float _grantDelaySeconds = 0.75f;

        [Header("Flow")]
        [SerializeField] private GameplayState _waitingState;

        private static readonly int EnterStateTrigger = Animator.StringToHash("NewRecordState");

        protected override GameplayStateType StateType => GameplayStateType.NewRecord;

        protected override async UniTask OnEnterAsync(CancellationToken ct)
        {
            CharacterAnimator.SetTrigger(EnterStateTrigger);

            var reward = Config.RecordReward(ScoreManager.Best);
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
