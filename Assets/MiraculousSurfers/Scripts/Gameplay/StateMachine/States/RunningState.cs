using System.Threading;
using Cysharp.Threading.Tasks;
using MiraculousSurfers.Gameplay.Characters;
using MiraculousSurfers.Gameplay.UI;
using UnityEngine;

namespace MiraculousSurfers.Gameplay
{
    public class RunningState : GameplayState
    {
        [Header("Refs")]
        [SerializeField] private Character _character;

        [Header("Presentation")]
        [SerializeField] private RunningScreenUiView _runningScreenView;

        [Header("Flow")]
        [SerializeField] private GameplayState _evaluateDistanceState;
        [SerializeField] private GameplayState _crashState;

        private float _power;
        private Vector3 _direction;
        private bool _running;

        public void Configure(float power, Vector3 direction)
        {
            _power = power;
            _direction = direction;
        }

        protected override async UniTask OnEnterAsync(CancellationToken ct)
        {
            Subscribe();
            _character.Launch(_power, _direction);
            _running = true;

            if (_runningScreenView != null)
            {
                _runningScreenView.SetDistance(0f);
                await _runningScreenView.ShowAsync(ct);
            }
        }

        protected override async UniTask OnExitAsync(CancellationToken ct)
        {
            if (_runningScreenView != null) await _runningScreenView.HideAsync(ct);
        }

        private void Update()
        {
            if (!_running || _character == null || _runningScreenView == null) return;
            _runningScreenView.SetDistance(_character.TraveledDistance);
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
            _running = false;
            if (_character == null) return;
            _character.Crashed -= OnCrashed;
            _character.MomentumEnded -= OnMomentumEnded;
            _character.SlowedDown -= OnSlowedDown;
        }
    }
}
