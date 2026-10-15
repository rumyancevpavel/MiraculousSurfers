using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MiraculousSurfers.Gameplay
{
    public class DebugChargeState : GameplayState
    {
        [Header("Launch params")]
        [SerializeField] private float _power = 15f;
        [SerializeField] private Vector3 _direction = Vector3.forward;

        [Header("Flow")]
        [SerializeField] private RunningState _runningState;

        protected override UniTask OnEnterAsync(CancellationToken ct)
        {
            _runningState.Configure(_power, _direction);
            Machine.GoTo(_runningState).Forget();
            return UniTask.CompletedTask;
        }
    }
}
