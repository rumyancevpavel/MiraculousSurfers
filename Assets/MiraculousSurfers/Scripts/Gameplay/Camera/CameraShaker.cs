using System;
using System.Collections.Generic;
using MiraculousSurfers.Gameplay.Events;
using Unity.Cinemachine;
using UnityEngine;
using VContainer;

namespace MiraculousSurfers.Gameplay.Camera
{
    public class CameraShaker : MonoBehaviour
    {
        [SerializeField] private CinemachineImpulseSource _impulseSource;
        [SerializeField] private float _crashForce = 1f;

        private GameplayEventBus _events;
        private readonly List<Action> _unsubscribers = new();

        [Inject]
        public void Construct(GameplayEventBus events)
        {
            _events = events;
        }

        private void Start()
        {
            _events.Subscribe<GameSignal>(GameplayEventType.Crashed, OnCrashed);
            _unsubscribers.Add(() => _events.Unsubscribe<GameSignal>(GameplayEventType.Crashed, OnCrashed));
        }

        private void OnDestroy()
        {
            foreach (var unsubscribe in _unsubscribers) unsubscribe();
            _unsubscribers.Clear();
        }

        private void OnCrashed(GameSignal signal) => _impulseSource.GenerateImpulseWithForce(_crashForce);
    }
}
