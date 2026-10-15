using System.Collections.Generic;
using MiraculousSurfers.Gameplay.Upgrades;
using UnityEngine;

namespace MiraculousSurfers.Configuration
{
    public class LocalGameConfigurationSystem : IGameConfigurationSystem
    {
        private readonly Dictionary<UpgradeParam, GameConfiguration.UpgradeLadder> _ladders = new();

        public LocalGameConfigurationSystem(GameConfiguration configuration)
        {
            foreach (var ladder in configuration.Ladders)
            {
                _ladders[ladder.Param] = ladder;
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

        private GameConfiguration.UpgradeLadder Resolve(UpgradeParam param)
        {
            return _ladders[param];
        }
    }
}
