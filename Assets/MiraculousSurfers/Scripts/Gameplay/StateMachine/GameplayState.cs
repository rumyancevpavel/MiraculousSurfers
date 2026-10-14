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
        [SerializeField] private string _stateMapName;

        protected GameplayStateMachine Machine { get; private set; }

        protected InputActionMap StateMap =>
            string.IsNullOrEmpty(_stateMapName) ? null : _inputAsset?.FindActionMap(_stateMapName, false);

        public void Bind(GameplayStateMachine machine) => Machine = machine;

        public async UniTask EnterAsync(CancellationToken ct)
        {
            _inputAsset?.Disable();
            await OnEnterAsync(ct);
        }

        public async UniTask ExitAsync(CancellationToken ct)
        {
            Unsubscribe();
            StateMap?.Disable();
            await OnExitAsync(ct);
        }

        protected void EnableStateInput()
        {
            _inputAsset?.Disable();
            StateMap?.Enable();
        }

        protected InputAction GetAction(string actionName) => StateMap?.FindAction(actionName, false);

        protected virtual void OnDisable()
        {
            Unsubscribe();
            StateMap?.Disable();
        }

        protected abstract UniTask OnEnterAsync(CancellationToken ct);

        protected virtual UniTask OnExitAsync(CancellationToken ct) => UniTask.CompletedTask;

        protected virtual void Unsubscribe() { }
        protected virtual void Subscribe() { }
    }
}
