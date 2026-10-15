using System.Threading;
using Cysharp.Threading.Tasks;
using MiraculousSurfers.Gameplay.Upgrades;
using UnityEngine;
using VContainer;

namespace MiraculousSurfers.Gameplay.StateMachine
{
    public abstract class GameplayState : MonoBehaviour
    {
        protected GameplayStateMachine Machine { get; private set; }
        protected IUpgradeService Upgrades { get; private set; }

        [Inject]
        public void InjectDependencies(IUpgradeService upgrades) => Upgrades = upgrades;

        public void Bind(GameplayStateMachine machine) => Machine = machine;

        public UniTask EnterAsync(CancellationToken ct) => OnEnterAsync(ct);

        public async UniTask ExitAsync(CancellationToken ct)
        {
            Unsubscribe();
            await OnExitAsync(ct);
        }

        protected virtual void OnDisable() => Unsubscribe();

        protected abstract UniTask OnEnterAsync(CancellationToken ct);

        protected virtual UniTask OnExitAsync(CancellationToken ct) => UniTask.CompletedTask;

        protected virtual void Subscribe() { }

        protected virtual void Unsubscribe() { }
    }
}
