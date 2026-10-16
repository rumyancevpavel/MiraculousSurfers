using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MiraculousSurfers.Gameplay.UI
{
    public class ChargeDragArea : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public event Action DragBegan;
        public event Action<float, Vector3> DragChanged;
        public event Action<float, Vector3> DragEnded;

        [SerializeField] private float _maxDrag = 300f;
        [SerializeField] private float _deadZone = 20f;
        [SerializeField] private float _maxCharge = 100f;
        [SerializeField] private AnimationCurve _chargeResponse = new(new Keyframe(0f, 0f, 0f, 2f), new Keyframe(1f, 1f, 0f, 0f));

        public float MaxCharge => _maxCharge;

        public void OnBeginDrag(PointerEventData eventData) => DragBegan?.Invoke();

        public void OnDrag(PointerEventData eventData)
        {
            Compute(eventData, out float charge, out Vector3 direction);
            DragChanged?.Invoke(charge, direction);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Compute(eventData, out float charge, out Vector3 direction);
            DragEnded?.Invoke(charge, direction);
        }

        private void Compute(PointerEventData eventData, out float charge, out Vector3 direction)
        {
            Vector2 delta = eventData.position - eventData.pressPosition;
            float down = Mathf.Max(0f, -delta.y);
            float t = Mathf.Clamp01((down - _deadZone) / Mathf.Max(1f, _maxDrag - _deadZone));
            charge = _maxCharge * _chargeResponse.Evaluate(t);
            direction = down >= _deadZone ? new Vector3(delta.x, 0f, down).normalized : Vector3.forward;
        }
    }
}
