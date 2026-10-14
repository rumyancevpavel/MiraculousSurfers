using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

namespace MiraculousSurfers.Gameplay
{
    [RequireComponent(typeof(CinemachineCamera))]
    public class OrbitalCameraRig : MonoBehaviour
    {
        [Serializable]
        public struct OrbitPreset
        {
            public float Horizontal;
            public float Vertical;
            public float Radius;
        }

        [SerializeField] private CinemachineCamera _cam;
        [SerializeField] private CinemachineOrbitalFollow _orbital;
        [SerializeField] private float _defaultDuration = 0.6f;

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

        public void SetImmediate(OrbitPreset p)
        {
            _orbital.HorizontalAxis.Value = p.Horizontal;
            _orbital.VerticalAxis.Value = p.Vertical;
            _orbital.RadialAxis.Value = p.Radius;
        }

        public async UniTask AnimateToAsync(OrbitPreset target, float duration = -1f, CancellationToken ct = default)
        {
            if (duration < 0f) duration = _defaultDuration;
            if (duration <= 0f) { SetImmediate(target); return; }

            float startH = _orbital.HorizontalAxis.Value;
            float startV = _orbital.VerticalAxis.Value;
            float startR = _orbital.RadialAxis.Value;

            float t = 0f;
            while (t < duration)
            {
                if (ct.IsCancellationRequested) return;
                t += Time.deltaTime;
                float k = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / duration));
                _orbital.HorizontalAxis.Value = Mathf.Lerp(startH, target.Horizontal, k);
                _orbital.VerticalAxis.Value = Mathf.Lerp(startV, target.Vertical, k);
                _orbital.RadialAxis.Value = Mathf.Lerp(startR, target.Radius, k);
                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            if (ct.IsCancellationRequested) return;
            SetImmediate(target);
        }
    }
}
