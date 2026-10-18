using MiraculousSurfers.Gameplay.Events;
using UnityEngine;
using VContainer;

namespace MiraculousSurfers.Gameplay.Characters
{
    public class AnimationEventsHandler : MonoBehaviour
    {
        private GameplayEventBus _events;

        [Inject]
        public void Construct(GameplayEventBus events) => _events = events;

        public void StepOnGround()
        {
            _events.Publish(GameplayEventType.SingleStep, new GameSignal());
        }
    }
}
