namespace MiraculousSurfers.Economy
{
    public class MockWalletService : IWalletService
    {
        private int _coins;

        public MockWalletService(int startingCoins)
        {
            _coins = startingCoins;
        }

        public int Coins => _coins;

        public bool TrySpend(int amount)
        {
            if (amount < 0 || _coins < amount) return false;
            _coins -= amount;
            return true;
        }
    }
}
