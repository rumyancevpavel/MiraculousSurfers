using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MiraculousSurfers.Gameplay.UI
{
    public class ChargeIndicatorView : MonoBehaviour
    {
        [SerializeField] private Image _fill;
        [SerializeField] private float _maxCharge = 100f;
        [SerializeField] private float _maxFill = 0.5f;
        [SerializeField] private TMP_Text _text;

        public void SetCharge(float charge)
        {
            if (_fill == null) return;
            float t = Mathf.Clamp01(charge / _maxCharge);
            _fill.fillAmount = Mathf.Lerp(0f, _maxFill, t);
            _text.text = ((int)charge).ToString();
        }
    }
}
