using System;
using UnityEngine;

namespace MiraculousSurfers.Gameplay
{
    public class Character : MonoBehaviour
    {
        public event Action SlowedDown;
        public event Action Crashed;
        public event Action<float> MomentumEnded;

        public float TraveledDistance { get; protected set; }

        public virtual void Launch(float power, Vector3 direction)
        {
            TraveledDistance = 0f;
        }

        protected void RaiseSlowedDown() => SlowedDown?.Invoke();
        protected void RaiseCrashed() => Crashed?.Invoke();
        protected void RaiseMomentumEnded() => MomentumEnded?.Invoke(TraveledDistance);
    }
}
