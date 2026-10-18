using UnityEngine;

namespace MiraculousSurfers.Gameplay.UI
{
    [RequireComponent(typeof(LineRenderer))]
    public class SlingView : MonoBehaviour
    {
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private float _maxPull = 4f;
        [SerializeField] private Vector3 _pullDirection = Vector3.back;
        [SerializeField] private AnimationCurve _pullProfile = new (new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

        private Vector3[] _restPositions;

        private void Awake()
        {
            if (_lineRenderer == null) _lineRenderer = GetComponent<LineRenderer>();
            CaptureRest();
        }

        public void SetCharge(float normalized)
        {
            if (_lineRenderer == null) return;
            if (_restPositions == null || _restPositions.Length != _lineRenderer.positionCount) CaptureRest();

            normalized = Mathf.Clamp01(normalized);
            var direction = _pullDirection.sqrMagnitude > 0.0001f ? _pullDirection.normalized : Vector3.back;
            var count = _restPositions.Length;

            for (var i = 0; i < count; i++)
            {
                var t = count > 1 ? (float)i / (count - 1) : 0f;
                var weight = _pullProfile.Evaluate(t);
                _lineRenderer.SetPosition(i, _restPositions[i] + direction * (_maxPull * normalized * weight));
            }
        }

        private void CaptureRest()
        {
            var count = _lineRenderer.positionCount;
            _restPositions = new Vector3[count];
            _lineRenderer.GetPositions(_restPositions);
        }
    }
}
