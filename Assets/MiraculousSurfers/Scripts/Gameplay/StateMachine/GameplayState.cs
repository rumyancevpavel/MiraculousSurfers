using System.Threading;
using Cysharp.Threading.Tasks;
using MiraculousSurfers.Gameplay.Characters;
using MiraculousSurfers.Gameplay.Events;
using MiraculousSurfers.Gameplay.Upgrades;
using UnityEngine;
using VContainer;

namespace MiraculousSurfers.Gameplay.StateMachine
{
    public abstract class GameplayState : MonoBehaviour
    {
        protected GameplayStateMachine Machine { get; private set; }
        protected IUpgradeService Upgrades { get; private set; }
        protected GameplayEventBus Events { get; private set; }
        protected Character Character => Machine != null ? Machine.Character : null;
        protected Animator CharacterAnimator => Machine != null ? Machine.CharacterAnimator : null;
        protected abstract GameplayStateType StateType { get; }

        [Inject]
        public void InjectDependencies(IUpgradeService upgrades, GameplayEventBus events)
        {
            Upgrades = upgrades;
            Events = events;
        }

        public void Bind(GameplayStateMachine machine) => Machine = machine;

        public UniTask EnterAsync(CancellationToken ct) => OnEnterAsync(ct);

        public async UniTask ExitAsync(CancellationToken ct)
        {
            Unsubscribe();
            await OnExitAsync(ct);
        }

        protected void OnEnable() => Subscribe();

        protected virtual void OnDisable() => Unsubscribe();

        protected abstract UniTask OnEnterAsync(CancellationToken ct);

        protected virtual UniTask OnExitAsync(CancellationToken ct) => UniTask.CompletedTask;

        protected virtual void Subscribe() { }

        protected virtual void Unsubscribe() { }
    }
}
