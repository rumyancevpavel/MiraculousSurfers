using System;
using Cysharp.Threading.Tasks;

namespace MiraculousSurfers.Economy
{
    public interface IWalletService
    {
        event Action Changed;

        int Coins { get; }
        UniTask<bool> TrySpend(int amount);
        UniTask AddCoins(int amount);
    }
}
