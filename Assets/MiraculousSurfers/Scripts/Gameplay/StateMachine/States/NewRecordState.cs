using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MiraculousSurfers.Gameplay.StateMachine.States
{
    public class NewRecordState : GameplayState
    {
        [Header("Timing")]
        [SerializeField] private float _celebrationSeconds = 1.5f;

        [Header("Flow")]
        [SerializeField] private GameplayState _waitingState;

        [Header("Animation")]
        [SerializeField] private string _enterStateTrigger = "NewRecordState";
        
        protected override GameplayStateType StateType => GameplayStateType.NewRecord;
        
        protected override async UniTask OnEnterAsync(CancellationToken ct)
        {
            if (CharacterAnimator != null) CharacterAnimator.SetTrigger(_enterStateTrigger);

            await UniTask.Delay(TimeSpan.FromSeconds(_celebrationSeconds), cancellationToken: ct)
                         .SuppressCancellationThrow();
            
            if (ct.IsCancellationRequested) return;

            Machine.GoTo(_waitingState).Forget();
        }
    }
}
