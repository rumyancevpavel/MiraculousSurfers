using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MiraculousSurfers.Gameplay.StateMachine.States
{
    public class TryEndedState : GameplayState
    {
        [Header("Flow")]
        [SerializeField] private GameplayState _waitingState;

        [Header("Animation")]
        [SerializeField] private string _enterStateTrigger = "TryEndedState";
        [SerializeField] private string _celebrationType = "TryEndedState_CelebrationType";
        [SerializeField] private float _delayBeforeWaitState = 4f;
        
        protected override GameplayStateType StateType => GameplayStateType.TryEnded;
        
        protected override async UniTask OnEnterAsync(CancellationToken ct)
        {
            var celebrationType = Random.Range(0, 1) > 0.5 ? 1 : 2;
            CharacterAnimator.SetTrigger(_enterStateTrigger);
            CharacterAnimator.SetInteger(_celebrationType, celebrationType);
            
            await UniTask.WaitForSeconds(_delayBeforeWaitState, cancellationToken: ct)
                .SuppressCancellationThrow();
            
            if (ct.IsCancellationRequested) return;
            
            Machine.GoTo(_waitingState).Forget();
        }
    }
}
