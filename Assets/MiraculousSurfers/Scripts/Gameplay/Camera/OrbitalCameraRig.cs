using System.Threading;
using Cysharp.Threading.Tasks;
using MiraculousSurfers.Gameplay.Camera;
using Unity.Cinemachine;
using UnityEngine;

namespace MiraculousSurfers.Gameplay
{
    [RequireComponent(typeof(CinemachineCamera))]
    public class OrbitalCameraRig : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera _cam;
        [SerializeField] private CinemachineOrbitalFollow _orbital;
        [SerializeField] private float _defaultDuration = 0.6f;
        [SerializeField] private AnimationCurve _defaultEase;

        private void Reset()
        {
            _cam = GetComponent<CinemachineCamera>();
            _orbital = GetComponent<CinemachineOrbitalFollow>();
        }

        private void Awake()
        {
            if (_cam == null) _cam = GetComponent<CinemachineCamera>();
            if (_orbital == null) _orbital = GetComponent<CinemachineOrbitalFollow>();
        }

        public void Activate() => _cam.Prioritize();

        private void SetImmediate(OrbitPreset p)
        {
            _orbital.HorizontalAxis.Value = p.Horizontal;
            _orbital.VerticalAxis.Value = p.Vertical;
            _orbital.RadialAxis.Value = p.Radius;
        }

        public async UniTask AnimateToAsync(OrbitPreset target, float duration = -1f, CancellationToken ct = default, AnimationCurve ease = null)
        {
            if (duration < 0f) duration = _defaultDuration;
            if (duration <= 0f) { SetImmediate(target); return; }

            var curve = ease != null && ease.length > 0 ? ease : _defaultEase;

            var startH = _orbital.HorizontalAxis.Value;
            var startV = _orbital.VerticalAxis.Value;
            float startR = _orbital.RadialAxis.Value;

            var t = 0f;
            while (t < duration)
            {
                if (ct.IsCancellationRequested) return;
                t += Time.deltaTime;
                var raw = Mathf.Clamp01(t / duration);
                var k = curve != null && curve.length > 0 ? curve.Evaluate(raw) : Mathf.SmoothStep(0f, 1f, raw);
                _orbital.HorizontalAxis.Value = Mathf.LerpAngle(startH, target.Horizontal, k);
                _orbital.VerticalAxis.Value = Mathf.Lerp(startV, target.Vertical, k);
                _orbital.RadialAxis.Value = Mathf.Lerp(startR, target.Radius, k);
                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            if (ct.IsCancellationRequested) return;
            SetImmediate(target);
        }
    }
}
