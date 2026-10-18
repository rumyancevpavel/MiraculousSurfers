using UnityEngine;

namespace MiraculousSurfers.Gameplay.Collectibles
{
    [RequireComponent(typeof(Collider))]
    public abstract class Collectible : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private float _minAnimationSpeed = 0.7f;
        [SerializeField] private float _maxAnimationSpeed = 1f;

        private static readonly int AnimationSpeed = Animator.StringToHash("AnimationSpeed");

        public abstract CollectibleKind Kind { get; }

        protected virtual void Awake()
        {
            _animator.SetFloat(AnimationSpeed, Random.Range(_minAnimationSpeed, _maxAnimationSpeed));
        }

        public void Collect() => Destroy(gameObject);
    }
}
