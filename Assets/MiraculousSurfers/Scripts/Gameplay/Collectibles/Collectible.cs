using UnityEngine;

namespace MiraculousSurfers.Gameplay.Collectibles
{
    public enum CollectibleKind
    {
        Coin,
        GiftBox
    }

    [RequireComponent(typeof(Collider))]
    public abstract class Collectible : MonoBehaviour
    {
        [SerializeField] private float _minAnimationSpeed = 0.7f;
        [SerializeField] private float _maxAnimationSpeed = 1f;

        private static readonly int AnimationSpeed = Animator.StringToHash("AnimationSpeed");

        public abstract CollectibleKind Kind { get; }

        protected virtual void Awake()
        {
            var animator = GetComponentInChildren<Animator>(true);
            if (animator == null || !HasParameter(animator, AnimationSpeed)) return;

            animator.SetFloat(AnimationSpeed, Random.Range(_minAnimationSpeed, _maxAnimationSpeed));
        }

        private static bool HasParameter(Animator animator, int nameHash)
        {
            foreach (var parameter in animator.parameters)
                if (parameter.nameHash == nameHash) return true;
            return false;
        }

        public void Collect() => Destroy(gameObject);
    }
}
