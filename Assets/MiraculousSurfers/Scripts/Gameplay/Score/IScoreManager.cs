using UnityEngine;

namespace MiraculousSurfers.Gameplay.Score
{
    public interface IScoreManager
    {
        float Best { get; }
        bool ReportDistance(float distance, Vector3 position);
    }
}
