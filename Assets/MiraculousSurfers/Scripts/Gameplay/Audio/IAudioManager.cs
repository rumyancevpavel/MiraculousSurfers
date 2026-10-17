using MiraculousSurfers.Gameplay.Events;

namespace MiraculousSurfers.Gameplay.Audio
{
    public interface IAudioManager
    {
        void Play(GameplayEventType eventType);
    }
}
