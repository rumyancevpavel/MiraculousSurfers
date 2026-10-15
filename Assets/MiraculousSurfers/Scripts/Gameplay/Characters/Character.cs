using System;
using UnityEngine;

namespace MiraculousSurfers.Gameplay.Characters
{
    public class Character : MonoBehaviour
    {
        public event Action SlowedDown;
        public event Action Crashed;
        public event Action<float> MomentumEnded;

        public float TraveledDistance { get; protected set; }

        public virtual void Launch(LaunchParams launchParams)
        {
            TraveledDistance = 0f;
        }

        public virtual void ResetToSpawn() { }

        protected void RaiseSlowedDown() => SlowedDown?.Invoke();
        protected void RaiseCrashed() => Crashed?.Invoke();
        protected void RaiseMomentumEnded() => MomentumEnded?.Invoke(TraveledDistance);
    }
}
