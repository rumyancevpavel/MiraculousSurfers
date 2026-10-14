using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MiraculousSurfers.Gameplay
{
    public abstract class GameplayState : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private InputActionAsset _inputAsset;
        [SerializeField] private InputActionMap _stateInputMap;

        protected GameplayStateMachine Machine { get; private set; }

        public void Bind(GameplayStateMachine machine) => Machine = machine;

        public async UniTask EnterAsync(CancellationToken ct)
        {
            _inputAsset?.Disable();
            await OnEnterAsync(ct);
        }

        public async UniTask ExitAsync(CancellationToken ct)
        {
            Unsubscribe();
            if (_stateInputMap != null) _stateInputMap.Disable();
            await OnExitAsync(ct);
        }

        protected void EnableStateInput()
        {
            _inputAsset?.Disable();
            if (_stateInputMap != null) _stateInputMap.Enable();
        }

        protected abstract UniTask OnEnterAsync(CancellationToken ct);

        protected virtual UniTask OnExitAsync(CancellationToken ct) => UniTask.CompletedTask;

        protected virtual void Unsubscribe() { }
    }
}
