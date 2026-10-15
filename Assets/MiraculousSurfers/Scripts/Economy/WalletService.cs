using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MiraculousSurfers.Data;
using VContainer.Unity;

namespace MiraculousSurfers.Economy
{
    public class WalletService : IWalletService, IAsyncStartable
    {
        private readonly IDataSaveSystem<PlayerWalletData> _save;

        private PlayerWalletData _data;

        public event Action Changed;

        public WalletService(IDataSaveSystem<PlayerWalletData> save)
        {
            _save = save;
            _data = save.CreateDefault();
        }

        public int Coins => _data.Coins;

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            var result = await _save.LoadAsync(cancellation);
            _data = result.Success ? result.Value : _save.CreateDefault();
            Changed?.Invoke();
        }

        public async UniTask<bool> TrySpend(int amount)
        {
            if (amount < 0 || _data.Coins < amount) return false;
            _data.Coins -= amount;
            var saved = await _save.SaveAsync(_data);
            if (!saved.Success)
            {
                _data.Coins += amount;
                return false;
            }

            Changed?.Invoke();
            return true;
        }

        public async UniTask AddCoins(int amount)
        {
            if (amount < 0) return;
            _data.Coins += amount;
            var saved = await _save.SaveAsync(_data);
            if (!saved.Success)
            {
                _data.Coins -= amount;
                return;
            }

            Changed?.Invoke();
        }
    }
}
