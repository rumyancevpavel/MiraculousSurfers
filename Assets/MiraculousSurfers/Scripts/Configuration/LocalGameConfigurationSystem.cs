using System.Collections.Generic;
using MiraculousSurfers.Gameplay.Collectibles;
using MiraculousSurfers.Gameplay.Events;
using MiraculousSurfers.Gameplay.Upgrades;
using UnityEngine;

namespace MiraculousSurfers.Configuration
{
    public class LocalGameConfigurationSystem : IGameConfigurationSystem
    {
        private static readonly AudioClip[] NoClips = new AudioClip[0];

        private readonly Dictionary<UpgradeParam, GameConfiguration.UpgradeLadder> _ladders = new();
        private readonly Dictionary<CollectibleKind, int> _collectibleCoins = new();
        private readonly Dictionary<GameplayEventType, AudioClip[]> _audioClips = new();

        public LocalGameConfigurationSystem(GameConfiguration configuration)
        {
            foreach (var ladder in configuration.PowerupsLadders)
            {
                _ladders[ladder.Param] = ladder;
            }

            foreach (var reward in configuration.CollectibleRewards)
            {
                _collectibleCoins[reward.Kind] = reward.Coins;
            }

            foreach (var entry in configuration.AudioEntries)
            {
                _audioClips[entry.Event] = entry.Clips;
            }
        }

        public int MaxIndex(UpgradeParam param)
        {
            return Resolve(param).Steps.Length;
        }

        public float Value(UpgradeParam param, int index)
        {
            var ladder = Resolve(param);
            if (index <= 0) return ladder.BaseValue;
            var steps = ladder.Steps;
            var clamped = index > steps.Length ? steps.Length : index;
            return clamped == 0 ? ladder.BaseValue : steps[clamped - 1].Value;
        }

        public int Price(UpgradeParam param, int index)
        {
            var steps = Resolve(param).Steps;
            if (index <= 0 || index > steps.Length) return 0;
            return steps[index - 1].Price;
        }

        public bool HasNext(UpgradeParam param, int index)
        {
            return index < Resolve(param).Steps.Length;
        }

        public string DisplayName(UpgradeParam param)
        {
            return Resolve(param).DisplayName;
        }

        public Sprite Icon(UpgradeParam param)
        {
            return Resolve(param).Icon;
        }

        public int CollectibleCoins(CollectibleKind kind)
        {
            return _collectibleCoins.TryGetValue(kind, out var coins) ? coins : 0;
        }

        public AudioClip[] AudioClips(GameplayEventType eventType)
        {
            return _audioClips.TryGetValue(eventType, out var clips) ? clips : NoClips;
        }

        private GameConfiguration.UpgradeLadder Resolve(UpgradeParam param)
        {
            return _ladders[param];
        }
    }
}
