using UnityEngine;

namespace MiraculousSurfers.Gameplay.Characters
{
    public readonly struct LaunchParams
    {
        public float Power { get; }
        public Vector3 Direction { get; }
        public float SpeedMultiplier { get; }
        public float SteerMultiplier { get; }

        public LaunchParams(float power, Vector3 direction, float speedMultiplier, float steerMultiplier)
        {
            Power = power;
            Direction = direction;
            SpeedMultiplier = speedMultiplier;
            SteerMultiplier = steerMultiplier;
        }
    }
}
