using UnityEngine;

namespace MiraculousSurfers.Gameplay.Collectibles
{
    [RequireComponent(typeof(Collider))]
    public class Coin : MonoBehaviour
    {
        [SerializeField] private int _value = 10;

        public int Value => _value;

        public void Collect() => Destroy(gameObject);
    }
}
