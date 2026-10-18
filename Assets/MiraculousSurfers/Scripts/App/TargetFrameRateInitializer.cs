using UnityEngine;
using VContainer.Unity;

namespace MiraculousSurfers.App
{
    public class TargetFrameRateInitializer : IInitializable
    {
        private const int TargetFrameRate = 60;

        public void Initialize()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = TargetFrameRate;
        }
    }
}
