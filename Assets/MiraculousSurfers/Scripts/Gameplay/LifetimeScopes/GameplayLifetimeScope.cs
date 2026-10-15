using MiraculousSurfers.Configuration;
using MiraculousSurfers.Data;
using MiraculousSurfers.Economy;
using MiraculousSurfers.Gameplay.Upgrades;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MiraculousSurfers.Gameplay.LifetimeScopes
{
    public class GameplayLifetimeScope : LifetimeScope
    {
        [SerializeField] private GameConfiguration _gameConfiguration;
        [SerializeField] private int _startingCoins = 1000;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<IDataSaveSystem<PlayerUpgradesData>, PlayerUpgradesSaveSystem>(Lifetime.Singleton);
            builder.RegisterInstance(_gameConfiguration);
            builder.Register<IGameConfigurationSystem, LocalGameConfigurationSystem>(Lifetime.Singleton);
            builder.Register<IWalletService, MockWalletService>(Lifetime.Singleton).WithParameter(_startingCoins);
            builder.RegisterEntryPoint<UpgradeService>(Lifetime.Singleton);
        }
    }
}
