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
        [SerializeField] private string _animationSpeedParam = "AnimationSpeed";
        [SerializeField] private float _minAnimationSpeed = 0.7f;
        [SerializeField] private float _maxAnimationSpeed = 1f;

        public abstract CollectibleKind Kind { get; }

        protected virtual void Awake()
        {
            if (string.IsNullOrEmpty(_animationSpeedParam)) return;

            var animator = GetComponentInChildren<Animator>(true);
            if (animator == null || !HasParameter(animator, _animationSpeedParam)) return;

            animator.SetFloat(_animationSpeedParam, Random.Range(_minAnimationSpeed, _maxAnimationSpeed));
        }

        private static bool HasParameter(Animator animator, string parameterName)
        {
            foreach (var parameter in animator.parameters)
                if (parameter.name == parameterName) return true;
            return false;
        }

        public void Collect() => Destroy(gameObject);
    }
}
