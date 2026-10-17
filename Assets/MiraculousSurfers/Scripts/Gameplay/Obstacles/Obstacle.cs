using UnityEngine;

namespace MiraculousSurfers.Gameplay.Obstacles
{
    public enum ObstacleEffect
    {
        Slow,
        Crash
    }

    [RequireComponent(typeof(Collider))]
    public class Obstacle : MonoBehaviour
    {
        [SerializeField] private ObstacleEffect _effect = ObstacleEffect.Crash;

        public ObstacleEffect Effect => _effect;
    }
}
