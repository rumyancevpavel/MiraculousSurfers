using MiraculousSurfers.Configuration;
using MiraculousSurfers.Data;
using MiraculousSurfers.Economy;
using MiraculousSurfers.Gameplay.Events;
using MiraculousSurfers.Gameplay.UI;
using MiraculousSurfers.Gameplay.Upgrades;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MiraculousSurfers.Gameplay.LifetimeScopes
{
    public class GameplayLifetimeScope : LifetimeScope
    {
        [SerializeField] private GameConfiguration _gameConfiguration;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<IDataSaveSystem<PlayerUpgradesData>, PlayerUpgradesSaveSystem>(Lifetime.Singleton);
            builder.RegisterInstance(_gameConfiguration);
            builder.Register<IGameConfigurationSystem, LocalGameConfigurationSystem>(Lifetime.Singleton);
            builder.Register<IDataSaveSystem<PlayerWalletData>, PlayerWalletSaveSystem>(Lifetime.Singleton);
            builder.RegisterEntryPoint<WalletService>(Lifetime.Singleton);
            builder.Register<GameplayEventBus>(Lifetime.Singleton);
            builder.RegisterEntryPoint<UpgradeService>(Lifetime.Singleton);
            builder.RegisterComponentInHierarchy<UpgradeMenuView>();
        }
    }
}
