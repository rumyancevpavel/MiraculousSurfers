namespace MiraculousSurfers.Economy
{
    public interface IWalletService
    {
        int Coins { get; }
        bool TrySpend(int amount);
    }
}
