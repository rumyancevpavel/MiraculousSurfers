using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MiraculousSurfers.Gameplay
{
    public class ShotState : GameplayState
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
            _character.Crashed += OnCrashed;
            _character.MomentumEnded += OnMomentumEnded;
            _character.SlowedDown += OnSlowedDown;

            _character.Launch(_power, _direction);
            return UniTask.CompletedTask;
        }

        private void OnCrashed() => Machine.GoTo(_crashState).Forget();

        private void OnMomentumEnded(float distance) => Machine.GoTo(_evaluateDistanceState).Forget();

        private void OnSlowedDown() { }

        protected override void Unsubscribe()
        {
            _character.Crashed -= OnCrashed;
            _character.MomentumEnded -= OnMomentumEnded;
            _character.SlowedDown -= OnSlowedDown;
        }
    }
}
