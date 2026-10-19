using MiraculousSurfers.Core.Events;
using UnityEngine;

namespace MiraculousSurfers.Gameplay.Events
{
    public readonly struct GameSignal : IEvent
    {
    }

    public readonly struct CoinCollected : IEvent
    {
        public readonly int Amount;
        public readonly Vector3 Position;

        public CoinCollected(int amount, Vector3 position)
        {
            Amount = amount;
            Position = position;
        }
    }

    public readonly struct GiftBoxCollected : IEvent
    {
        public readonly Vector3 Position;

        public GiftBoxCollected(Vector3 position)
        {
            Position = position;
        }
    }
}
