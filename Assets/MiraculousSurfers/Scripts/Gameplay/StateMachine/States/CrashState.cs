using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MiraculousSurfers.Gameplay.StateMachine.States
{
    public class CrashState : GameplayState
    {
        [Header("Timing")]
        [SerializeField] private float _deathAnimSeconds = 4f;

        [Header("Flow")]
        [SerializeField] private GameplayState _waitingState;

        [Header("Animation")]
        [SerializeField] private string _enterStateTrigger = "CrashState";
        
        protected override GameplayStateType StateType => GameplayStateType.Crashed;

        protected override async UniTask OnEnterAsync(CancellationToken ct)
        {
            if (CharacterAnimator != null) CharacterAnimator.SetTrigger(_enterStateTrigger);

            await UniTask.Delay(TimeSpan.FromSeconds(_deathAnimSeconds), cancellationToken: ct)
                         .SuppressCancellationThrow();
            
            if (ct.IsCancellationRequested) return;

            Machine.GoTo(_waitingState).Forget();
        }
    }
}
