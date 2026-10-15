using System.Threading;
using Cysharp.Threading.Tasks;
using MiraculousSurfers.Gameplay.Characters;
using UnityEngine;

namespace MiraculousSurfers.Gameplay
{
    public class RunningState : GameplayState
    {
        [Header("Refs")]
        [SerializeField] private Character _character;

        [Header("Flow")]
        [SerializeField] private GameplayState _evaluateDistanceState;
        [SerializeField] private GameplayState _crashState;

        private float _power;
        private Vector3 _direction;

        public void Configure(float power, Vector3 direction)
        {
            _power = power;
            _direction = direction;
        }

        protected override UniTask OnEnterAsync(CancellationToken ct)
        {
            Subscribe();
            _character.Launch(_power, _direction);
            return UniTask.CompletedTask;
        }

        private void OnCrashed() => Machine.GoTo(_crashState).Forget();

        private void OnMomentumEnded(float distance) => Machine.GoTo(_evaluateDistanceState).Forget();

        private void OnSlowedDown() { }

        protected override void Subscribe()
        {
            if (_character == null) return;
            _character.Crashed += OnCrashed;
            _character.MomentumEnded += OnMomentumEnded;
            _character.SlowedDown += OnSlowedDown;
        }

        protected override void Unsubscribe()
        {
            if (_character == null) return;
            _character.Crashed -= OnCrashed;
            _character.MomentumEnded -= OnMomentumEnded;
            _character.SlowedDown -= OnSlowedDown;
        }
    }
}
