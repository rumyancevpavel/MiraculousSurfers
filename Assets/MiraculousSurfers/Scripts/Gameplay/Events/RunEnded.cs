using MiraculousSurfers.Core.Events;
using UnityEngine;

namespace MiraculousSurfers.Gameplay.Events
{
    public readonly struct RunEnded : IEvent
    {
        public readonly float Distance;
        public readonly Vector3 Position;
        public readonly RunEndReason Reason;

        public RunEnded(float distance, Vector3 position, RunEndReason reason)
        {
            Distance = distance;
            Position = position;
            Reason = reason;
        }
    }
}
