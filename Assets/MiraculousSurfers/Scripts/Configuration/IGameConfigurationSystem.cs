using MiraculousSurfers.Gameplay.Upgrades;
using UnityEngine;

namespace MiraculousSurfers.Configuration
{
    public interface IGameConfigurationSystem
    {
        int MaxIndex(UpgradeParam param);
        float Value(UpgradeParam param, int index);
        int Price(UpgradeParam param, int index);
        bool HasNext(UpgradeParam param, int index);
        string DisplayName(UpgradeParam param);
        Sprite Icon(UpgradeParam param);
    }
}
