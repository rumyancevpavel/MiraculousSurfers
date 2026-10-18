using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MiraculousSurfers.Configuration;
using MiraculousSurfers.Economy;
using MiraculousSurfers.Gameplay.Events;
using MiraculousSurfers.Gameplay.Score;
using MiraculousSurfers.Gameplay.UI;
using UnityEngine;
using VContainer;

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

        [Header("Animation")]
        [SerializeField] private string _enterStateTrigger = "NewRecordState";

        private IScoreManager _scoreManager;
        private IWalletService _wallet;
        private IGameConfigurationSystem _config;

        protected override GameplayStateType StateType => GameplayStateType.NewRecord;

        [Inject]
        public void InjectNewRecord(IScoreManager scoreManager, IWalletService wallet, IGameConfigurationSystem config)
        {
            _scoreManager = scoreManager;
            _wallet = wallet;
            _config = config;
        }

        protected override async UniTask OnEnterAsync(CancellationToken ct)
        {
            CharacterAnimator.SetTrigger(_enterStateTrigger);

            var reward = _config.RecordReward(_scoreManager.Best);
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
