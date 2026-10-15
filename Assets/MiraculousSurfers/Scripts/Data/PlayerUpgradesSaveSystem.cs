namespace MiraculousSurfers.Data
{
    public class PlayerUpgradesSaveSystem : PlayerPrefsDataSaveSystem<PlayerUpgradesData>
    {
        private const string StorageKeyRoot = "miraculoussurfers.player_upgrades";
        private const string FormatVersion = "v1";
        private const int DefaultIndex = 0;

        protected override string Key => StorageKeyRoot;
        protected override string Version => FormatVersion;

        public override PlayerUpgradesData CreateDefault()
        {
            return new PlayerUpgradesData
            {
                LaunchPower = DefaultIndex,
                Speed = DefaultIndex,
                SteeringResponsiveness = DefaultIndex,
                CollectibleValue = DefaultIndex
            };
        }

        protected override bool IsValid(PlayerUpgradesData data)
        {
            return data != null
                && data.LaunchPower >= 0
                && data.Speed >= 0
                && data.SteeringResponsiveness >= 0
                && data.CollectibleValue >= 0;
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("MiraculousSurfers/Reset Saved Data/Player Upgrades")]
        public static void ResetSavedData()
        {
            UnityEngine.PlayerPrefs.DeleteKey($"{StorageKeyRoot}.{FormatVersion}");
            UnityEngine.PlayerPrefs.Save();
            UnityEngine.Debug.Log("Player upgrades saved data reset.");
        }

        [UnityEditor.MenuItem("MiraculousSurfers/Reset Saved Data/All")]
        public static void ResetAllSavedData()
        {
            ResetSavedData();
            PlayerWalletSaveSystem.ResetSavedData();
        }
#endif
    }
}
