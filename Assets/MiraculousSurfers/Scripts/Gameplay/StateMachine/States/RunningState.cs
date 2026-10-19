using System.Threading;
using Cysharp.Threading.Tasks;
using MiraculousSurfers.Gameplay.Camera;
using MiraculousSurfers.Gameplay.Characters;
using MiraculousSurfers.Gameplay.Collectibles;
using MiraculousSurfers.Gameplay.Events;
using MiraculousSurfers.Gameplay.UI;
using MiraculousSurfers.Gameplay.Upgrades;
using UnityEngine;

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
        [SerializeField] private AnimationCurve _speedToBlend = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        [Header("Speed Feel")]
        [SerializeField] private OrbitalCameraRig _cameraRig;
        [SerializeField] private float _fovSpeedMin = 14f;
        [SerializeField] private float _fovSpeedMax = 26f;

        private static readonly int EnterStateTrigger = Animator.StringToHash("RunningState");
        private static readonly int RunningSpeed = Animator.StringToHash("RunningState_RunningSpeed");

        private LaunchParams _launchParams;
        private bool _running;

        protected override GameplayStateType StateType => GameplayStateType.Running;

        public void Configure(LaunchParams launchParams)
        {
            _launchParams = launchParams;
        }

        public void BeginRunningAnimation()
        {
            CharacterAnimator.SetTrigger(EnterStateTrigger);
        }

        protected override async UniTask OnEnterAsync(CancellationToken ct)
        {
            Subscribe();
            Character.Launch(_launchParams);
            _running = true;
            Events.Publish(GameplayEventType.CharacterBeginsRunning, new GameSignal());
            Events.Publish(GameplayEventType.RunMusicStarted, new GameSignal());

            if (_runningScreenView != null)
            {
                _runningScreenView.SetDistance(0f);
                await _runningScreenView.ShowAsync(ct);
            }
        }

        protected override UniTask OnExitAsync(CancellationToken ct)
        {
            Events.Publish(GameplayEventType.RunMusicStopped, new GameSignal());
            _cameraRig.ResetFov();
            if (_runningScreenView != null) _runningScreenView.HideAsync(ct).Forget();
            return UniTask.CompletedTask;
        }

        private void Update()
        {
            if (!_running || Character == null) return;
            if (_runningScreenView != null) _runningScreenView.SetDistance(Character.TraveledDistance);
            CharacterAnimator.SetFloat(RunningSpeed, _speedToBlend.Evaluate(Character.SpeedFraction));
            _cameraRig.SetSpeedFactor(Mathf.InverseLerp(_fovSpeedMin, _fovSpeedMax, Character.CurrentSpeed));
        }

        private void OnCrashed()
        {
            Events.Publish(GameplayEventType.RunEnded, new RunEnded(Character.TraveledDistance, Character.transform.position, RunEndReason.Crashed));
            Events.Publish(GameplayEventType.Crashed, new GameSignal());
            ScoreManager.ReportDistance(Character.TraveledDistance, Character.transform.position);
            Machine.GoTo(_crashState).Forget();
        }

        private void OnMomentumEnded(float distance)
        {
            Events.Publish(GameplayEventType.RunEnded, new RunEnded(distance, Character.transform.position, RunEndReason.MomentumDepleted));
            var isNewHighScore = ScoreManager.ReportDistance(distance, Character.transform.position);
            Machine.GoTo(isNewHighScore ? _newRecordState : _tryEndedState).Forget();
        }

        private void OnSlowedDown() { }

        private void OnCollectiblePickedUp(CollectibleKind kind, Vector3 position)
        {
            var baseCoins = Config.CollectibleCoins(kind);
            var amount = Mathf.Max(0, Mathf.RoundToInt(baseCoins * Upgrades.GetValue(UpgradeParam.CollectibleValue)));
            if (amount <= 0) return;
            Wallet.AddCoins(amount).Forget();

            if (kind == CollectibleKind.GiftBox)
                Events.Publish(GameplayEventType.GiftBoxCollected, new GiftBoxCollected(position));
            else
                Events.Publish(GameplayEventType.CoinCollected, new CoinCollected(amount, position));
        }

        protected override void Subscribe()
        {
            if (Character == null) return;
            Character.Crashed += OnCrashed;
            Character.MomentumEnded += OnMomentumEnded;
            Character.SlowedDown += OnSlowedDown;
            Character.CollectiblePickedUp += OnCollectiblePickedUp;
        }

        protected override void Unsubscribe()
        {
            _running = false;
            if (Character == null) return;
            Character.Crashed -= OnCrashed;
            Character.MomentumEnded -= OnMomentumEnded;
            Character.SlowedDown -= OnSlowedDown;
            Character.CollectiblePickedUp -= OnCollectiblePickedUp;
        }
    }
}
