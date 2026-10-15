using System;
using MiraculousSurfers.Gameplay.Upgrades;
using UnityEngine;

namespace MiraculousSurfers.Configuration
{
    [CreateAssetMenu(menuName = "MiraculousSurfers/Game Configuration", fileName = "GameConfiguration")]
    public class GameConfiguration : ScriptableObject
    {
        [Serializable]
        public struct UpgradeStep
        {
            [SerializeField] private float _value;
            [SerializeField] private int _price;

            public float Value => _value;
            public int Price => _price;
        }

        [Serializable]
        public class UpgradeLadder
        {
            [SerializeField] private UpgradeParam _param;
            [SerializeField] private string _displayName;
            [SerializeField] private Sprite _icon;
            [SerializeField] private float _baseValue;
            [SerializeField] private UpgradeStep[] _steps;

            public UpgradeParam Param => _param;
            public string DisplayName => _displayName;
            public Sprite Icon => _icon;
            public float BaseValue => _baseValue;
            public UpgradeStep[] Steps => _steps;
        }

        [SerializeField] private UpgradeLadder[] _ladders;

        public UpgradeLadder[] Ladders => _ladders;
    }
}
