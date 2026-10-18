using System;
using System.Collections.Generic;
using MiraculousSurfers.Configuration;
using MiraculousSurfers.Gameplay.Events;
using UnityEngine;
using VContainer;

namespace MiraculousSurfers.Gameplay.Audio
{
    public class AudioManager : MonoBehaviour, IAudioManager
    {
        private static readonly GameplayEventType[] SignalEvents =
        {
            GameplayEventType.UpgradeRequested,
            GameplayEventType.Crashed,
            GameplayEventType.GiftBoxCollected,
            GameplayEventType.CharacterBeginsRunning,
            GameplayEventType.SingleStep,
            GameplayEventType.Upgrade,
            GameplayEventType.NewRecord,
        };

        [SerializeField] private AudioSource _sfxSource;
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _countingSource;

        [Header("Theme music ducking")]
        [SerializeField] private float _musicLowVolume = 0.15f;
        [SerializeField] private float _musicHighVolume = 0.6f;
        [SerializeField] private float _musicFadeDuration = 0.6f;

        private GameplayEventBus _events;
        private IGameConfigurationSystem _config;
        private readonly List<Action> _unsubscribers = new();
        private float _musicTargetVolume;

        [Inject]
        public void Construct(GameplayEventBus events, IGameConfigurationSystem config)
        {
            _events = events;
            _config = config;
        }

        private void Start()
        {
            _countingSource.loop = true;
            StartTheme();

            Action<CoinCollected> coinHandler = _ => Play(GameplayEventType.CoinCollected);
            _events.Subscribe(GameplayEventType.CoinCollected, coinHandler);
            _unsubscribers.Add(() => _events.Unsubscribe(GameplayEventType.CoinCollected, coinHandler));

            foreach (var eventType in SignalEvents)
            {
                var captured = eventType;
                Action<GameSignal> handler = _ => Play(captured);
                _events.Subscribe(captured, handler);
                _unsubscribers.Add(() => _events.Unsubscribe(captured, handler));
            }

            Action<GameSignal> runStarted = _ => _musicTargetVolume = _musicHighVolume;
            _events.Subscribe(GameplayEventType.RunMusicStarted, runStarted);
            _unsubscribers.Add(() => _events.Unsubscribe(GameplayEventType.RunMusicStarted, runStarted));

            Action<GameSignal> runStopped = _ => _musicTargetVolume = _musicLowVolume;
            _events.Subscribe(GameplayEventType.RunMusicStopped, runStopped);
            _unsubscribers.Add(() => _events.Unsubscribe(GameplayEventType.RunMusicStopped, runStopped));

            Action<GameSignal> startCounting = _ => PlayLooping(_countingSource, GameplayEventType.CoinsCountingStarted);
            _events.Subscribe(GameplayEventType.CoinsCountingStarted, startCounting);
            _unsubscribers.Add(() => _events.Unsubscribe(GameplayEventType.CoinsCountingStarted, startCounting));

            Action<GameSignal> stopCounting = _ => _countingSource.Stop();
            _events.Subscribe(GameplayEventType.CoinsCountingStopped, stopCounting);
            _unsubscribers.Add(() => _events.Unsubscribe(GameplayEventType.CoinsCountingStopped, stopCounting));
        }

        private void OnDestroy()
        {
            foreach (var unsubscribe in _unsubscribers) unsubscribe();
            _unsubscribers.Clear();
        }

        private void Update()
        {
            var maxDelta = Time.unscaledDeltaTime / _musicFadeDuration;
            _musicSource.volume = Mathf.MoveTowards(_musicSource.volume, _musicTargetVolume, maxDelta);
        }

        private void StartTheme()
        {
            _musicTargetVolume = _musicLowVolume;
            _musicSource.clip = _config.AudioClips(GameplayEventType.RunMusicStarted)[0];
            _musicSource.loop = true;
            _musicSource.volume = _musicLowVolume;
            _musicSource.Play();
        }

        public void Play(GameplayEventType eventType)
        {
            var clips = _config.AudioClips(eventType);
            _sfxSource.PlayOneShot(clips.Length == 1 ? clips[0] : clips[UnityEngine.Random.Range(0, clips.Length)]);
        }

        private void PlayLooping(AudioSource source, GameplayEventType eventType)
        {
            var clips = _config.AudioClips(eventType);
            source.clip = clips.Length == 1 ? clips[0] : clips[UnityEngine.Random.Range(0, clips.Length)];
            source.Play();
        }
    }
}
