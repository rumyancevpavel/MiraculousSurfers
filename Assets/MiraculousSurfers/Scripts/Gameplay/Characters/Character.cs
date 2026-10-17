using System;
using UnityEngine;

namespace MiraculousSurfers.Gameplay.Characters
{
    public class Character : MonoBehaviour
    {
        public event Action SlowedDown;
        public event Action Crashed;
        public event Action<float> MomentumEnded;
        public event Action<int> CoinPickedUp;

        public float TraveledDistance { get; protected set; }
        public float CurrentSpeed { get; protected set; }
        public float SpeedFraction { get; protected set; }

        public virtual void Launch(LaunchParams launchParams)
        {
            TraveledDistance = 0f;
            CurrentSpeed = 0f;
            SpeedFraction = 0f;
        }

        public virtual void ResetToSpawn() { }

        public virtual void SetChargePull(float distance) { }

        protected void RaiseSlowedDown() => SlowedDown?.Invoke();
        protected void RaiseCrashed() => Crashed?.Invoke();
        protected void RaiseMomentumEnded() => MomentumEnded?.Invoke(TraveledDistance);
        protected void RaiseCoinPickedUp(int value) => CoinPickedUp?.Invoke(value);
    }
}
