using MiraculousSurfers.Gameplay.Collectibles;
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
        [SerializeField] private InputActionReference _pointerPositionInput;
        [SerializeField] private InputActionReference _pointerPressInput;

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
        [SerializeField] private float _dragSteerScreenFraction = 0.15f;

        [Header("Grounding")]
        [SerializeField] private float _gravity = 20f;

        [Header("Obstacles")]
        [SerializeField] private float _slowSpeedFactor = 0.5f;

        private bool _running;
        private bool _dragging;
        private float _dragStartX;
        private float _momentumFuel;
        private float _initialFuel;
        private float _speedMultiplier = 1f;
        private float _steerMultiplier = 1f;
        private float _heading;

        public override void Launch(LaunchParams launchParams)
        {
            base.Launch(launchParams);

            _speedMultiplier = launchParams.SpeedMultiplier;
            _steerMultiplier = launchParams.SteerMultiplier;

            _initialFuel = Mathf.Max(0.0001f, launchParams.Power * _fuelPerPower);
            _momentumFuel = _initialFuel;
            _heading = 0f;

            EnableInput();
            _running = true;
        }

        private void EnableInput()
        {
            _steerInput.action.Enable();
            _pointerPositionInput.action.Enable();
            _pointerPressInput.action.Enable();
        }

        private void DisableInput()
        {
            _steerInput.action.Disable();
            _pointerPositionInput.action.Disable();
            _pointerPressInput.action.Disable();
        }

        public override void ResetToSpawn()
        {
            _running = false;
            _momentumFuel = 0f;
            TraveledDistance = 0f;
            CurrentSpeed = 0f;
            SpeedFraction = 0f;
            _heading = 0f;
            DisableInput();

            _controller.enabled = false;
            transform.SetPositionAndRotation(_spawn.position, _spawn.rotation);
            _controller.enabled = true;
        }

        public override void SetChargePull(float distance)
        {
            if (_running) return;

            _controller.enabled = false;
            transform.SetPositionAndRotation(_spawn.position - Vector3.forward * Mathf.Max(0f, distance), _spawn.rotation);
            _controller.enabled = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_running) return;

            var collectible = other.GetComponentInParent<Collectible>();
            if (collectible != null)
            {
                RaiseCollectiblePickedUp(collectible.Kind, collectible.transform.position);
                collectible.Collect();
                return;
            }

            var obstacle = other.GetComponentInParent<Obstacle>();
            if (obstacle == null) return;

            if (obstacle.Effect == ObstacleEffect.Crash)
            {
                _running = false;
                DisableInput();
                RaiseCrashed();
            }
            else
            {
                _speedMultiplier *= _slowSpeedFactor;
                RaiseSlowedDown();
            }
        }

        private float ReadSteer()
        {
            if (_pointerPressInput.action.IsPressed())
            {
                var x = _pointerPositionInput.action.ReadValue<Vector2>().x;
                if (!_dragging)
                {
                    _dragging = true;
                    _dragStartX = x;
                }

                var range = Screen.width * _dragSteerScreenFraction;
                return Mathf.Clamp((x - _dragStartX) / range, -1f, 1f);
            }

            _dragging = false;
            return Mathf.Clamp(_steerInput.action.ReadValue<Vector2>().x, -1f, 1f);
        }

        private void Update()
        {
            if (!_running) return;

            var steer = ReadSteer();
            UpdateHeading(steer);
            var speed = UpdateSpeedState();
            ApplyMovement(speed);
            ConsumeMomentumFuel(steer);
        }

        private void UpdateHeading(float steer)
        {
            if (Mathf.Abs(steer) > 0.001f)
                _heading = Mathf.Clamp(_heading + steer * _steerRate * _steerMultiplier * Time.deltaTime, -_maxHeading, _maxHeading);
            else
                _heading = Mathf.MoveTowards(_heading, 0f, _headingReturnRate * Time.deltaTime);
        }

        private float UpdateSpeedState()
        {
            var speed = _momentumFuel * _fuelToSpeed * _speedMultiplier;
            CurrentSpeed = speed;
            SpeedFraction = Mathf.Clamp01(_momentumFuel / _initialFuel);
            return speed;
        }

        private void ApplyMovement(float speed)
        {
            var headingRotation = Quaternion.Euler(0f, _heading, 0f);
            transform.rotation = headingRotation;

            var left = _leftBound.position.x;
            var right = _rightBound.position.x;
            var currentX = transform.position.x;

            var delta = headingRotation * Vector3.forward * (speed * Time.deltaTime);
            var newX = Mathf.Clamp(currentX + delta.x, left, right);
            delta.x = newX - currentX;
            delta.y = -_gravity * Time.deltaTime;

            var before = transform.position;
            _controller.Move(delta);
            var travelled = transform.position - before;
            travelled.y = 0f;
            TraveledDistance += travelled.magnitude;
        }

        private void ConsumeMomentumFuel(float steer)
        {
            var frac = Mathf.Clamp01(_momentumFuel / _initialFuel);
            var drain = Mathf.Max(_drainCurve.Evaluate(frac), _minDrain) + Mathf.Abs(steer) * _steerFuelCost;
            _momentumFuel -= drain * Time.deltaTime;

            if (_momentumFuel > 0f) return;

            _momentumFuel = 0f;
            CurrentSpeed = 0f;
            SpeedFraction = 0f;
            _running = false;
            DisableInput();
            RaiseMomentumEnded();
        }
    }
}
