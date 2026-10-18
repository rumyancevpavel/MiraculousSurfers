using System.Threading;
using Cysharp.Threading.Tasks;
using MiraculousSurfers.Configuration;
using MiraculousSurfers.Economy;
using MiraculousSurfers.Gameplay.Characters;
using MiraculousSurfers.Gameplay.Events;
using MiraculousSurfers.Gameplay.Score;
using MiraculousSurfers.Gameplay.Upgrades;
using UnityEngine;

namespace MiraculousSurfers.Gameplay.StateMachine
{
    public abstract class GameplayState : MonoBehaviour
    {
        protected GameplayStateMachine Machine { get; private set; }
        protected IUpgradeService Upgrades => Machine != null ? Machine.Upgrades : null;
        protected GameplayEventBus Events => Machine != null ? Machine.Events : null;
        protected IScoreManager ScoreManager => Machine != null ? Machine.ScoreManager : null;
        protected IWalletService Wallet => Machine != null ? Machine.Wallet : null;
        protected IGameConfigurationSystem Config => Machine != null ? Machine.Config : null;
        protected Character Character => Machine != null ? Machine.Character : null;
        protected Animator CharacterAnimator => Machine != null ? Machine.CharacterAnimator : null;
        protected abstract GameplayStateType StateType { get; }

        public void Bind(GameplayStateMachine machine) => Machine = machine;

        public UniTask EnterAsync(CancellationToken ct) => OnEnterAsync(ct);

        public async UniTask ExitAsync(CancellationToken ct)
        {
            Unsubscribe();
            await OnExitAsync(ct);
        }

        protected abstract UniTask OnEnterAsync(CancellationToken ct);

        protected virtual UniTask OnExitAsync(CancellationToken ct) => UniTask.CompletedTask;

        protected virtual void Subscribe() { }

        protected virtual void Unsubscribe() { }
    }
}
