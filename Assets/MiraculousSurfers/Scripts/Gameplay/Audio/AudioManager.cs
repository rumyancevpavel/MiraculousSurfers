using System;
using System.Collections.Generic;
using MiraculousSurfers.Configuration;
using MiraculousSurfers.Gameplay.Events;
using UnityEngine;
using VContainer.Unity;

namespace MiraculousSurfers.Gameplay.Audio
{
    public class AudioManager : IAudioManager, IStartable, IDisposable
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

        private readonly GameplayEventBus _events;
        private readonly IGameConfigurationSystem _config;
        private readonly List<Action> _unsubscribers = new();

        private AudioSource _source;
        private AudioSource _musicSource;

        public AudioManager(GameplayEventBus events, IGameConfigurationSystem config)
        {
            _events = events;
            _config = config;
        }

        public void Start()
        {
            var host = new GameObject("[AudioManager]");
            _source = host.AddComponent<AudioSource>();
            _source.playOnAwake = false;
            _musicSource = host.AddComponent<AudioSource>();
            _musicSource.playOnAwake = false;
            _musicSource.loop = true;

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

        public void Play(GameplayEventType eventType)
        {
            if (_source == null) return;

            var clips = _config.AudioClips(eventType);
            if (clips == null || clips.Length == 0) return;

            var clip = clips.Length == 1 ? clips[0] : clips[UnityEngine.Random.Range(0, clips.Length)];
            if (clip != null) _source.PlayOneShot(clip);
        }

        private void PlayMusic(GameplayEventType eventType)
        {
            if (_musicSource == null) return;

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

        public void Dispose()
        {
            foreach (var unsubscribe in _unsubscribers) unsubscribe();
            _unsubscribers.Clear();

            if (_source != null) UnityEngine.Object.Destroy(_source.gameObject);
        }
    }
}
