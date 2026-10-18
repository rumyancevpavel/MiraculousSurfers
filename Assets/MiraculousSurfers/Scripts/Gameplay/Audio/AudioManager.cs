using System;
using System.Collections.Generic;
using MiraculousSurfers.Configuration;
using MiraculousSurfers.Core.Events;
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
            _events.Subscribe<CoinCollected>(GameplayEventType.CoinCollected, OnCoinCollected);
            _unsubscribers.Add(() => _events.Unsubscribe<CoinCollected>(GameplayEventType.CoinCollected, OnCoinCollected));

            foreach (var eventType in SignalEvents)
            {
                var captured = eventType;
                Action<GameSignal> handler = signal => Play(captured);
                _events.Subscribe(captured, handler);
                _unsubscribers.Add(() => _events.Unsubscribe(captured, handler));
            }
            
            _events.Subscribe<GameSignal>(GameplayEventType.RunMusicStarted, OnRunStarted);
            _unsubscribers.Add(() => _events.Unsubscribe<GameSignal>(GameplayEventType.RunMusicStarted, OnRunStarted));
            
            _events.Subscribe<GameSignal>(GameplayEventType.RunMusicStopped, OnRunStopped);
            _unsubscribers.Add(() => _events.Unsubscribe<GameSignal>(GameplayEventType.RunMusicStopped, OnRunStopped));
            
            _events.Subscribe<GameSignal>(GameplayEventType.CoinsCountingStarted, OnStartCounting);
            _unsubscribers.Add(() => _events.Unsubscribe<GameSignal>(GameplayEventType.CoinsCountingStarted, OnStartCounting));
            
            _events.Subscribe<GameSignal>(GameplayEventType.CoinsCountingStopped, OnStopCounting);
            _unsubscribers.Add(() => _events.Unsubscribe<GameSignal>(GameplayEventType.CoinsCountingStopped, OnStopCounting));
        }

        private void OnCoinCollected(CoinCollected coinCollected)
        {
            Play(GameplayEventType.CoinCollected);
        }

        private void OnRunStarted(GameSignal signal)
        {
            _musicTargetVolume = _musicHighVolume;
        }
        
        private void OnRunStopped(GameSignal signal)
        {
            _musicTargetVolume = _musicLowVolume;
        }

        private void OnStartCounting(GameSignal signal)
        {
            PlayLooping(_countingSource, GameplayEventType.CoinsCountingStarted);
        }

        private void OnStopCounting(GameSignal signal)
        {
            _countingSource.Stop();
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
