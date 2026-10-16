using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MiraculousSurfers.Gameplay.StateMachine.States
{
    public class CrashState : GameplayState
    {
        [Header("Refs")]
        [SerializeField] private string _dieTrigger = "Die";

        [Header("Timing")]
        [SerializeField] private float _deathAnimSeconds = 1.5f;

        [Header("Flow")]
        [SerializeField] private GameplayState _waitingState;

        protected override GameplayStateType StateType => GameplayStateType.Crashed;
        
        protected override async UniTask OnEnterAsync(CancellationToken ct)
        {
            if (CharacterAnimator != null) CharacterAnimator.SetTrigger(_dieTrigger);

            await UniTask.Delay(TimeSpan.FromSeconds(_deathAnimSeconds), cancellationToken: ct)
                         .SuppressCancellationThrow();
            if (ct.IsCancellationRequested) return;

            Machine.GoTo(_waitingState).Forget();
        }
    }
}
