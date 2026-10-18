using MiraculousSurfers.App;
using MiraculousSurfers.Configuration;
using MiraculousSurfers.Data;
using MiraculousSurfers.Economy;
using MiraculousSurfers.Gameplay.Audio;
using MiraculousSurfers.Gameplay.Characters;
using MiraculousSurfers.Gameplay.Events;
using MiraculousSurfers.Gameplay.Score;
using MiraculousSurfers.Gameplay.StateMachine;
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
            builder.RegisterEntryPoint<TargetFrameRateInitializer>();
            builder.RegisterComponentInHierarchy<HomeScreenWalletView>();
            builder.RegisterComponentInHierarchy<RunningScreenWalletView>();
            builder.RegisterComponentInHierarchy<NewRecordWalletView>();
            builder.RegisterComponentInHierarchy<TryEndedWalletView>();
            builder.Register<IDataSaveSystem<PlayerUpgradesData>, PlayerUpgradesSaveSystem>(Lifetime.Singleton);
            builder.Register<IDataSaveSystem<PlayerHighScoreData>, PlayerHighScoreSaveSystem>(Lifetime.Singleton);
            builder.Register<IDataSaveSystem<PlayerWalletData>, PlayerWalletSaveSystem>(Lifetime.Singleton);
            builder.Register<IGameConfigurationSystem, LocalGameConfigurationSystem>(Lifetime.Singleton);
            builder.RegisterInstance(_gameConfiguration);
            builder.RegisterEntryPoint<WalletService>();
            builder.Register<GameplayEventBus>(Lifetime.Singleton);
            builder.RegisterEntryPoint<UpgradeService>();
            builder.RegisterComponentInHierarchy<AudioManager>().As<IAudioManager>();
            builder.RegisterComponentInHierarchy<AnimationEventsHandler>();
            builder.RegisterComponentInHierarchy<UpgradeMenuView>();
            builder.RegisterComponentInHierarchy<GameplayStateMachine>();
            builder.RegisterComponentInHierarchy<ScoreManager>().As<IScoreManager>();
        }
    }
}
