using System;
using System.Collections.Generic;
using UnityEngine;

namespace MiraculousSurfers.Core.Events
{
    public abstract class EventBus<T> where T : struct, Enum
    {
        private sealed class Subscription
        {
            public Type PayloadType;
            public readonly List<Delegate> Handlers = new List<Delegate>();
        }

        private readonly Dictionary<T, Subscription> _subscriptions = new Dictionary<T, Subscription>();
        private readonly Stack<List<Delegate>> _bufferPool = new Stack<List<Delegate>>();

        public void Subscribe<TEvent>(T eventType, Action<TEvent> handler) where TEvent : IEvent
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var subscription = Bind(eventType, typeof(TEvent));
            if (!subscription.Handlers.Contains(handler))
                subscription.Handlers.Add(handler);
        }

        public void Unsubscribe<TEvent>(T eventType, Action<TEvent> handler) where TEvent : IEvent
        {
            if (handler == null)
                return;
            if (_subscriptions.TryGetValue(eventType, out var subscription))
                subscription.Handlers.Remove(handler);
        }

        public void Publish<TEvent>(T eventType, TEvent eventData) where TEvent : IEvent
        {
            var subscription = Bind(eventType, typeof(TEvent));
            if (subscription.Handlers.Count == 0)
                return;

            var buffer = RentBuffer();
            buffer.AddRange(subscription.Handlers);
            for (var i = 0; i < buffer.Count; i++)
            {
                try
                {
                    ((Action<TEvent>)buffer[i]).Invoke(eventData);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
            ReturnBuffer(buffer);
        }

        public bool HasSubscribers(T eventType)
        {
            return _subscriptions.TryGetValue(eventType, out var subscription) && subscription.Handlers.Count > 0;
        }

        public void Clear()
        {
            _subscriptions.Clear();
        }

        private Subscription Bind(T eventType, Type payloadType)
        {
            if (!_subscriptions.TryGetValue(eventType, out var subscription))
            {
                subscription = new Subscription { PayloadType = payloadType };
                _subscriptions.Add(eventType, subscription);
                return subscription;
            }

            if (subscription.PayloadType != payloadType)
            {
                throw new InvalidOperationException(
                    $"{GetType().Name}: event '{eventType}' carries {subscription.PayloadType.Name}, " +
                    $"but {payloadType.Name} was used. One payload type per event.");
            }

            return subscription;
        }

        private List<Delegate> RentBuffer()
        {
            return _bufferPool.Count > 0 ? _bufferPool.Pop() : new List<Delegate>();
        }

        private void ReturnBuffer(List<Delegate> buffer)
        {
            buffer.Clear();
            _bufferPool.Push(buffer);
        }
    }
}
