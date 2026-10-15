using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MiraculousSurfers.Configuration;
using MiraculousSurfers.Core;
using MiraculousSurfers.Data;
using MiraculousSurfers.Economy;
using VContainer.Unity;

namespace MiraculousSurfers.Gameplay.Upgrades
{
    public class UpgradeService : IUpgradeService, IAsyncStartable
    {
        private readonly IDataSaveSystem<PlayerUpgradesData> _save;
        private readonly IGameConfigurationSystem _config;
        private readonly IWalletService _wallet;

        private PlayerUpgradesData _data;

        public event Action<UpgradeParam> Changed;

        public UpgradeService(IDataSaveSystem<PlayerUpgradesData> save, IGameConfigurationSystem config, IWalletService wallet)
        {
            _save = save;
            _config = config;
            _wallet = wallet;
            _data = save.CreateDefault();
        }

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            var result = await _save.LoadAsync(cancellation);
            _data = result.Success ? result.Value : _save.CreateDefault();
            ClampAll();
        }

        public int GetMaxIndex(UpgradeParam param)
        {
            return _config.MaxIndex(param);
        }

        public float GetValue(UpgradeParam param)
        {
            return _config.Value(param, GetIndex(param));
        }

        public int GetNextCost(UpgradeParam param)
        {
            var index = GetIndex(param);
            return _config.HasNext(param, index) ? _config.Price(param, index + 1) : 0;
        }

        public bool CanUpgrade(UpgradeParam param)
        {
            var index = GetIndex(param);
            return _config.HasNext(param, index) && _wallet.Coins >= _config.Price(param, index + 1);
        }

        public IResult<UpgradePurchase> TryUpgrade(UpgradeParam param)
        {
            try
            {
                var index = GetIndex(param);
                if (!_config.HasNext(param, index))
                {
                    return Result.Ok(new UpgradePurchase(false, false, index, _config.Value(param, index)));
                }

                var price = _config.Price(param, index + 1);
                if (!_wallet.TrySpend(price))
                {
                    return Result.Ok(new UpgradePurchase(false, false, index, _config.Value(param, index)));
                }

                var newIndex = index + 1;
                SetIndex(param, newIndex);
                var persisted = _save.SaveAsync(_data).GetAwaiter().GetResult().Success;
                var newValue = _config.Value(param, newIndex);
                Changed?.Invoke(param);
                return Result.Ok(new UpgradePurchase(true, persisted, newIndex, newValue));
            }
            catch (Exception exception)
            {
                return Result.Fail<UpgradePurchase>(exception);
            }
        }

        private void ClampAll()
        {
            foreach (UpgradeParam param in Enum.GetValues(typeof(UpgradeParam)))
            {
                var max = _config.MaxIndex(param);
                var index = GetIndex(param);
                if (index < 0) SetIndex(param, 0);
                else if (index > max) SetIndex(param, max);
            }
        }

        public int GetIndex(UpgradeParam param)
        {
            return param switch
            {
                UpgradeParam.LaunchPower => _data.LaunchPower,
                UpgradeParam.Speed => _data.Speed,
                UpgradeParam.SteeringResponsiveness => _data.SteeringResponsiveness,
                UpgradeParam.CollectibleValue => _data.CollectibleValue,
                _ => 0
            };
        }

        private void SetIndex(UpgradeParam param, int value)
        {
            switch (param)
            {
                case UpgradeParam.LaunchPower: _data.LaunchPower = value; break;
                case UpgradeParam.Speed: _data.Speed = value; break;
                case UpgradeParam.SteeringResponsiveness: _data.SteeringResponsiveness = value; break;
                case UpgradeParam.CollectibleValue: _data.CollectibleValue = value; break;
            }
        }
    }
}
