using MiraculousSurfers.Economy;

namespace MiraculousSurfers.Data
{
    public class PlayerWalletSaveSystem : PlayerPrefsDataSaveSystem<PlayerWalletData>
    {
        private const string StorageKeyRoot = "miraculoussurfers.player_wallet";
        private const string FormatVersion = "v1";
        private const int StartingCoins = 1000;

        protected override string Key => StorageKeyRoot;
        protected override string Version => FormatVersion;

        public override PlayerWalletData CreateDefault()
        {
            return new PlayerWalletData { Coins = StartingCoins };
        }

        protected override bool IsValid(PlayerWalletData data)
        {
            return data != null && data.Coins >= 0;
        }
    }
}
