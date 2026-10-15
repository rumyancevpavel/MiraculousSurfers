using MiraculousSurfers.Gameplay.Score;

namespace MiraculousSurfers.Data
{
    public class PlayerHighScoreSaveSystem : PlayerPrefsDataSaveSystem<PlayerHighScoreData>
    {
        private const string StorageKeyRoot = "miraculoussurfers.player_highscore";
        private const string FormatVersion = "v1";

        protected override string Key => StorageKeyRoot;
        protected override string Version => FormatVersion;

        public override PlayerHighScoreData CreateDefault()
        {
            return new PlayerHighScoreData();
        }

        protected override bool IsValid(PlayerHighScoreData data)
        {
            return data != null && data.BestDistance >= 0f;
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("MiraculousSurfers/Reset Saved Data/Player High Score")]
        public static void ResetSavedData()
        {
            UnityEngine.PlayerPrefs.DeleteKey($"{StorageKeyRoot}.{FormatVersion}");
            UnityEngine.PlayerPrefs.Save();
            UnityEngine.Debug.Log("Player high score saved data reset.");
        }
#endif
    }
}
