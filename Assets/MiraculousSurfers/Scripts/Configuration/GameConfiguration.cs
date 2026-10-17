using System;
using MiraculousSurfers.Gameplay.Collectibles;
using MiraculousSurfers.Gameplay.Events;
using MiraculousSurfers.Gameplay.Upgrades;
using UnityEngine;
using UnityEngine.Serialization;

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

        [Serializable]
        public class CollectibleReward
        {
            [SerializeField] private CollectibleKind _kind;
            [SerializeField] private int _coins;

            public CollectibleKind Kind => _kind;
            public int Coins => _coins;
        }

        [Serializable]
        public class AudioEntry
        {
            [SerializeField] private GameplayEventType _event;
            [SerializeField] private AudioClip[] _clips;

            public GameplayEventType Event => _event;
            public AudioClip[] Clips => _clips;
        }

        [FormerlySerializedAs("_ladders")]
        [SerializeField] private UpgradeLadder[] _powerupsLadders;
        [SerializeField] private CollectibleReward[] _collectibleRewards;
        [SerializeField] private AudioEntry[] _audioEntries;

        public UpgradeLadder[] PowerupsLadders => _powerupsLadders;
        public CollectibleReward[] CollectibleRewards => _collectibleRewards;
        public AudioEntry[] AudioEntries => _audioEntries;
    }
}
