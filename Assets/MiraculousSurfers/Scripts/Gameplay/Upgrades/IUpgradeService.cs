using System;
using MiraculousSurfers.Core;

namespace MiraculousSurfers.Gameplay.Upgrades
{
    public interface IUpgradeService
    {
        event Action<UpgradeParam> Changed;

        int GetIndex(UpgradeParam param);
        int GetMaxIndex(UpgradeParam param);
        float GetValue(UpgradeParam param);
        int GetNextCost(UpgradeParam param);
        bool CanUpgrade(UpgradeParam param);
        IResult<UpgradePurchase> TryUpgrade(UpgradeParam param);
    }
}
