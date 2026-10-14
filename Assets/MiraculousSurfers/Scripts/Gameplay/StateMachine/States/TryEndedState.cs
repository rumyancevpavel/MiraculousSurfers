using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MiraculousSurfers.Gameplay
{
    public class TryEndedState : GameplayState
    {
        [Header("Flow")]
        [SerializeField] private GameplayState _waitingState;

        protected override UniTask OnEnterAsync(CancellationToken ct)
        {
            Machine.GoTo(_waitingState).Forget();
            return UniTask.CompletedTask;
        }
    }
}
