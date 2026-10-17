using MiraculousSurfers.Gameplay.Obstacles;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MiraculousSurfers.Gameplay.Characters
{
    [RequireComponent(typeof(CharacterController))]
    public class RunningCharacter : Character
    {
        [Header("Refs")]
        [SerializeField] private CharacterController _controller;
        [SerializeField] private Transform _spawn;
        [SerializeField] private Transform _leftBound;
        [SerializeField] private Transform _rightBound;

        [Header("Input")]
        [SerializeField] private InputActionReference _steerInput;

        [Header("Momentum")]
        [SerializeField] private float _fuelPerPower = 1f;
        [SerializeField] private float _fuelToSpeed = 2f;
        [SerializeField] private AnimationCurve _drainCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);
        [SerializeField] private float _minDrain = 0.05f;

        [Header("Steering")]
        [SerializeField] private float _steerRate = 90f;
        [SerializeField] private float _maxHeading = 35f;
        [SerializeField] private float _headingReturnRate = 120f;
        [SerializeField] private float _steerFuelCost = 0.5f;

        [Header("Grounding")]
        [SerializeField] private float _gravity = 20f;

        [Header("Obstacles")]
        [SerializeField] private float _slowSpeedFactor = 0.5f;

        private bool _running;
        private float _fuel;
        private float _initialFuel;
        private float _speedMultiplier = 1f;
        private float _steerMultiplier = 1f;
        private float _heading;

        private void Awake()
        {
            if (_controller == null) _controller = GetComponent<CharacterController>();
        }

        public override void Launch(LaunchParams launchParams)
        {
            base.Launch(launchParams);

            _speedMultiplier = launchParams.SpeedMultiplier;
            _steerMultiplier = launchParams.SteerMultiplier;

            _initialFuel = Mathf.Max(0.0001f, launchParams.Power * _fuelPerPower);
            _fuel = _initialFuel;
            _heading = 0f;

            _steerInput?.action.Enable();
            _running = true;
        }

        public override void ResetToSpawn()
        {
            _running = false;
            _fuel = 0f;
            TraveledDistance = 0f;
            CurrentSpeed = 0f;
            SpeedFraction = 0f;
            _heading = 0f;
            _steerInput?.action.Disable();

            if (_spawn == null) return;
            _controller.enabled = false;
            transform.SetPositionAndRotation(_spawn.position, _spawn.rotation);
            _controller.enabled = true;
        }

        public override void SetChargePull(float distance)
        {
            if (_spawn == null || _running) return;

            _controller.enabled = false;
            transform.SetPositionAndRotation(_spawn.position - Vector3.forward * Mathf.Max(0f, distance), _spawn.rotation);
            _controller.enabled = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_running) return;

            var obstacle = other.GetComponentInParent<Obstacle>();
            if (obstacle == null) return;

            if (obstacle.Effect == ObstacleEffect.Crash)
            {
                _running = false;
                _steerInput?.action.Disable();
                RaiseCrashed();
            }
            else
            {
                _speedMultiplier *= _slowSpeedFactor;
                RaiseSlowedDown();
            }
        }

        private void Update()
        {
            if (!_running) return;

            float steer = _steerInput != null ? Mathf.Clamp(_steerInput.action.ReadValue<Vector2>().x, -1f, 1f) : 0f;
            float left = _leftBound != null ? _leftBound.position.x : float.NegativeInfinity;
            float right = _rightBound != null ? _rightBound.position.x : float.PositiveInfinity;

            if (Mathf.Abs(steer) > 0.001f)
                _heading = Mathf.Clamp(_heading + steer * _steerRate * _steerMultiplier * Time.deltaTime, -_maxHeading, _maxHeading);
            else
                _heading = Mathf.MoveTowards(_heading, 0f, _headingReturnRate * Time.deltaTime);

            float speed = _fuel * _fuelToSpeed * _speedMultiplier;
            CurrentSpeed = speed;
            SpeedFraction = Mathf.Clamp01(_fuel / _initialFuel);

            var headingRotation = Quaternion.Euler(0f, _heading, 0f);
            transform.rotation = headingRotation;

            float currentX = transform.position.x;
            Vector3 delta = headingRotation * Vector3.forward * (speed * Time.deltaTime);
            float newX = Mathf.Clamp(currentX + delta.x, left, right);
            delta.x = newX - currentX;
            delta.y = -_gravity * Time.deltaTime;

            Vector3 before = transform.position;
            _controller.Move(delta);
            Vector3 travelled = transform.position - before;
            travelled.y = 0f;
            TraveledDistance += travelled.magnitude;

            float frac = Mathf.Clamp01(_fuel / _initialFuel);
            float drain = Mathf.Max(_drainCurve.Evaluate(frac), _minDrain) + Mathf.Abs(steer) * _steerFuelCost;
            _fuel -= drain * Time.deltaTime;

            if (_fuel <= 0f)
            {
                _fuel = 0f;
                CurrentSpeed = 0f;
                SpeedFraction = 0f;
                _running = false;
                _steerInput?.action.Disable();
                RaiseMomentumEnded();
            }
        }
    }
}
