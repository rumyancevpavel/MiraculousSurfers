using MiraculousSurfers.Core.Events;

namespace MiraculousSurfers.Gameplay.Events
{
    public readonly struct CoinCollected : IEvent
    {
        public readonly int Amount;

        public CoinCollected(int amount)
        {
            Amount = amount;
        }
    }
}
