using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MiraculousSurfers.Configuration;
using MiraculousSurfers.Gameplay.Events;
using MiraculousSurfers.Gameplay.Upgrades;
using UnityEngine;
using VContainer;

namespace MiraculousSurfers.Gameplay.UI
{
    public class UpgradeMenuView : MonoBehaviour
    {
        [SerializeField] private UpgradeSkillCardView[] _cards;

        private readonly Dictionary<UpgradeParam, UpgradeSkillCardView> _cardsByParam = new();

        private IGameConfigurationSystem _configuration;
        private IUpgradeService _upgrades;
        private GameplayEventBus _events;

        [Inject]
        public void Construct(IGameConfigurationSystem configuration, IUpgradeService upgrades, GameplayEventBus events)
        {
            _configuration = configuration;
            _upgrades = upgrades;
            _events = events;
        }

        private void Awake()
        {
            if (_cards == null) return;
            foreach (var card in _cards)
            {
                if (card != null) _cardsByParam[card.Param] = card;
            }
        }

        private void OnEnable()
        {
            foreach (var card in _cardsByParam.Values)
            {
                card.UpgradeRequested += OnUpgradeRequested;
            }
        }

        private void OnDisable()
        {
            foreach (var card in _cardsByParam.Values)
            {
                card.UpgradeRequested -= OnUpgradeRequested;
            }
        }

        private void Start()
        {
            foreach (var card in _cardsByParam.Values)
            {
                card.Bind(_configuration);
                Refresh(card);
            }
        }

        private void OnUpgradeRequested(UpgradeParam param)
        {
            _events.Publish(GameplayEventType.UpgradeRequested, new GameSignal());
            HandleUpgrade(param).Forget();
        }

        private async UniTask HandleUpgrade(UpgradeParam param)
        {
            var result = await _upgrades.TryUpgrade(param);
            if (result.Success && result.Value.Applied && _cardsByParam.TryGetValue(param, out var card))
            {
                Refresh(card);
            }
        }

        private void Refresh(UpgradeSkillCardView card)
        {
            card.SetProgress(_upgrades.GetIndex(card.Param), _upgrades.GetMaxIndex(card.Param), _upgrades.GetNextCost(card.Param));
        }
    }
}
