using MiraculousSurfers.Core.Events;
using UnityEngine;

namespace MiraculousSurfers.Gameplay.Events
{
    public readonly struct NewHighScore : IEvent
    {
        public readonly float Distance;
        public readonly Vector3 Position;

        public NewHighScore(float distance, Vector3 position)
        {
            Distance = distance;
            Position = position;
        }
    }
}
