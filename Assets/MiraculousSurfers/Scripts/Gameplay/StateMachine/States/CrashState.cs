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

        private static readonly int EnterStateTrigger = Animator.StringToHash("CrashState");

        protected override GameplayStateType StateType => GameplayStateType.Crashed;

        protected override async UniTask OnEnterAsync(CancellationToken ct)
        {
            CharacterAnimator.SetTrigger(EnterStateTrigger);

            await UniTask.Delay(TimeSpan.FromSeconds(_deathAnimSeconds), cancellationToken: ct)
                         .SuppressCancellationThrow();
            
            if (ct.IsCancellationRequested) return;

            Machine.GoTo(_waitingState).Forget();
        }
    }
}
