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

        private GameplayEventBus _events;
        private IGameConfigurationSystem _config;
        private readonly List<Action> _unsubscribers = new();

        [Inject]
        public void Construct(GameplayEventBus events, IGameConfigurationSystem config)
        {
            _events = events;
            _config = config;
        }

        private void Start()
        {
            if (_musicSource != null) _musicSource.loop = true;
            if (_events == null) return;

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

            Action<GameSignal> startMusic = _ => PlayMusic(GameplayEventType.RunMusicStarted);
            _events.Subscribe(GameplayEventType.RunMusicStarted, startMusic);
            _unsubscribers.Add(() => _events.Unsubscribe(GameplayEventType.RunMusicStarted, startMusic));

            Action<GameSignal> stopMusic = _ => StopMusic();
            _events.Subscribe(GameplayEventType.RunMusicStopped, stopMusic);
            _unsubscribers.Add(() => _events.Unsubscribe(GameplayEventType.RunMusicStopped, stopMusic));
        }

        private void OnDestroy()
        {
            foreach (var unsubscribe in _unsubscribers) unsubscribe();
            _unsubscribers.Clear();
        }

        public void Play(GameplayEventType eventType)
        {
            if (_sfxSource == null || _config == null) return;

            var clips = _config.AudioClips(eventType);
            if (clips == null || clips.Length == 0) return;

            var clip = clips.Length == 1 ? clips[0] : clips[UnityEngine.Random.Range(0, clips.Length)];
            if (clip != null) _sfxSource.PlayOneShot(clip);
        }

        private void PlayMusic(GameplayEventType eventType)
        {
            if (_musicSource == null || _config == null) return;

            var clips = _config.AudioClips(eventType);
            if (clips == null || clips.Length == 0) return;

            var clip = clips.Length == 1 ? clips[0] : clips[UnityEngine.Random.Range(0, clips.Length)];
            if (clip == null) return;

            _musicSource.clip = clip;
            _musicSource.Play();
        }

        private void StopMusic()
        {
            if (_musicSource != null) _musicSource.Stop();
        }
    }
}
