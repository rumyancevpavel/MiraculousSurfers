using UnityEngine;

namespace MiraculousSurfers.Gameplay.Collectibles
{
    public enum CollectibleKind
    {
        MacaroonCoin,
        GiftBox
    }

    [RequireComponent(typeof(Collider))]
    public abstract class Collectible : MonoBehaviour
    {
        public abstract CollectibleKind Kind { get; }

        public void Collect() => Destroy(gameObject);
    }
}
