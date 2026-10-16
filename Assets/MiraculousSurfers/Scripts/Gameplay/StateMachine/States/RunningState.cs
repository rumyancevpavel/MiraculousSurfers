using System.Threading;
using Cysharp.Threading.Tasks;
using MiraculousSurfers.Gameplay.Characters;
using MiraculousSurfers.Gameplay.Events;
using MiraculousSurfers.Gameplay.Score;
using MiraculousSurfers.Gameplay.UI;
using UnityEngine;
using VContainer;

namespace MiraculousSurfers.Gameplay.StateMachine.States
{
    public class RunningState : GameplayState
    {
        [Header("Presentation")]
        [SerializeField] private RunningScreenUiView _runningScreenView;

        [Header("Flow")]
        [SerializeField] private GameplayState _newRecordState;
        [SerializeField] private GameplayState _tryEndedState;
        [SerializeField] private GameplayState _crashState;

        [Header("Animation")]
        [SerializeField] private string _enterStateTrigger = "RunningState";
        [SerializeField] private string _runningSpeedParam = "RunningState_RunningSpeed";
        [SerializeField] private AnimationCurve _speedToBlend = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        
        private IScoreManager _scoreManager;
        private LaunchParams _launchParams;
        private bool _running;

        protected override GameplayStateType StateType => GameplayStateType.Running;
        
        [Inject]
        public void InjectScore(IScoreManager scoreManager) => _scoreManager = scoreManager;

        public void Configure(LaunchParams launchParams)
        {
            _launchParams = launchParams;
        }

        public void BeginRunningAnimation()
        {
            if (CharacterAnimator != null) CharacterAnimator.SetTrigger(_enterStateTrigger);
        }

        protected override async UniTask OnEnterAsync(CancellationToken ct)
        {
            Subscribe();
            Character.Launch(_launchParams);
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
            if (!_running || Character == null) return;
            if (_runningScreenView != null) _runningScreenView.SetDistance(Character.TraveledDistance);
            if (CharacterAnimator != null) CharacterAnimator.SetFloat(_runningSpeedParam, _speedToBlend.Evaluate(Character.SpeedFraction));
        }

        private void OnCrashed()
        {
            Events.Publish(GameplayEventType.RunEnded, new RunEnded(Character.TraveledDistance, Character.transform.position, RunEndReason.Crashed));
            _scoreManager.ReportDistance(Character.TraveledDistance, Character.transform.position);
            Machine.GoTo(_crashState).Forget();
        }

        private void OnMomentumEnded(float distance)
        {
            Events.Publish(GameplayEventType.RunEnded, new RunEnded(distance, Character.transform.position, RunEndReason.MomentumDepleted));
            var isNewHighScore = _scoreManager.ReportDistance(distance, Character.transform.position);
            Machine.GoTo(isNewHighScore ? _newRecordState : _tryEndedState).Forget();
        }

        private void OnSlowedDown() { }

        protected override void Subscribe()
        {
            if (Character == null) return;
            Character.Crashed += OnCrashed;
            Character.MomentumEnded += OnMomentumEnded;
            Character.SlowedDown += OnSlowedDown;
        }

        protected override void Unsubscribe()
        {
            _running = false;
            if (Character == null) return;
            Character.Crashed -= OnCrashed;
            Character.MomentumEnded -= OnMomentumEnded;
            Character.SlowedDown -= OnSlowedDown;
        }
    }
}
