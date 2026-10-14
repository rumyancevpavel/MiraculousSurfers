using UnityEngine;

namespace MiraculousSurfers.Gameplay
{
    public class ScoreManager : MonoBehaviour
    {
        public float Best { get; private set; }

        public bool Submit(float distance)
        {
            if (distance <= Best) return false;
            Best = distance;
            return true;
        }
    }
}
