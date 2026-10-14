using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MiraculousSurfers.Gameplay
{
    public class CrashState : GameplayState
    {
        [Header("Refs")]
        [SerializeField] private Animator _characterAnimator;
        [SerializeField] private string _dieTrigger = "Die";

        [Header("Timing")]
        [SerializeField] private float _deathAnimSeconds = 1.5f;

        [Header("Flow")]
        [SerializeField] private GameplayState _waitingState;

        protected override async UniTask OnEnterAsync(CancellationToken ct)
        {
            if (_characterAnimator != null) _characterAnimator.SetTrigger(_dieTrigger);

            await UniTask.Delay(TimeSpan.FromSeconds(_deathAnimSeconds), cancellationToken: ct)
                         .SuppressCancellationThrow();
            if (ct.IsCancellationRequested) return;

            Machine.GoTo(_waitingState).Forget();
        }
    }
}
