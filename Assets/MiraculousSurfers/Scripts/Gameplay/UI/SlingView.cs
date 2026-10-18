using UnityEngine;

namespace MiraculousSurfers.Gameplay.UI
{
    [RequireComponent(typeof(LineRenderer))]
    public class SlingView : MonoBehaviour
    {
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private float _maxPull = 4f;
        [SerializeField] private Vector3 _pullDirection = Vector3.back;
        [SerializeField] private AnimationCurve _pullProfile;

        private Vector3[] _restPositions;

        private void Awake()
        {
            _restPositions = new Vector3[_lineRenderer.positionCount];
            _lineRenderer.GetPositions(_restPositions);
        }

        public void SetCharge(float normalized) => SetCharge(normalized, 1f);

        public void SetCharge(float normalized, float pullScale)
        {
            normalized = Mathf.Clamp01(normalized);
            var direction = _pullDirection.normalized;
            var count = _restPositions.Length;

            for (var i = 0; i < count; i++)
            {
                var t = count > 1 ? (float)i / (count - 1) : 0f;
                var weight = _pullProfile.Evaluate(t);
                _lineRenderer.SetPosition(i, _restPositions[i] + direction * (_maxPull * pullScale * normalized * weight));
            }
        }
    }
}
