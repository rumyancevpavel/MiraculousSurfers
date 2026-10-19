using System;
using System.Collections.Generic;
using MiraculousSurfers.Gameplay.Events;
using UnityEngine;
using VContainer;

namespace MiraculousSurfers.Gameplay.Vfx
{
    public class VfxManager : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _coinCollectedVfx;
        [SerializeField] private ParticleSystem _giftBoxCollectedVfx;
        [SerializeField] private ParticleSystem _upgradeVfx;
        [SerializeField] private Transform _upgradeOrigin;

        private GameplayEventBus _events;
        private readonly List<Action> _unsubscribers = new();

        [Inject]
        public void Construct(GameplayEventBus events)
        {
            _events = events;
        }

        private void Start()
        {
            _events.Subscribe<CoinCollected>(GameplayEventType.CoinCollected, OnCoinCollected);
            _unsubscribers.Add(() => _events.Unsubscribe<CoinCollected>(GameplayEventType.CoinCollected, OnCoinCollected));

            _events.Subscribe<GiftBoxCollected>(GameplayEventType.GiftBoxCollected, OnGiftBoxCollected);
            _unsubscribers.Add(() => _events.Unsubscribe<GiftBoxCollected>(GameplayEventType.GiftBoxCollected, OnGiftBoxCollected));

            _events.Subscribe<GameSignal>(GameplayEventType.Upgrade, OnUpgrade);
            _unsubscribers.Add(() => _events.Unsubscribe<GameSignal>(GameplayEventType.Upgrade, OnUpgrade));
        }

        private void OnDestroy()
        {
            foreach (var unsubscribe in _unsubscribers) unsubscribe();
            _unsubscribers.Clear();
        }

        private void OnCoinCollected(CoinCollected coinCollected) => Spawn(_coinCollectedVfx, coinCollected.Position, Quaternion.identity);

        private void OnGiftBoxCollected(GiftBoxCollected giftBoxCollected) => Spawn(_giftBoxCollectedVfx, giftBoxCollected.Position, Quaternion.identity);

        private void OnUpgrade(GameSignal signal) => Spawn(_upgradeVfx, _upgradeOrigin.position, _upgradeOrigin.rotation);

        private void Spawn(ParticleSystem prefab, Vector3 position, Quaternion rotation)
        {
            var instance = Instantiate(prefab, position, rotation);
            var main = instance.main;
            main.stopAction = ParticleSystemStopAction.Destroy;
            instance.Play(true);
        }
    }
}
