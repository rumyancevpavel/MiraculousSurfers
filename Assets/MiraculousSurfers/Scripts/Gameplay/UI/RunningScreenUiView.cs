using TMPro;
using UnityEngine;

namespace MiraculousSurfers.Gameplay.UI
{
    public class RunningScreenUiView : UiScreenView
    {
        [SerializeField] private TMP_Text _distanceText;
        [SerializeField] private string _format = "{0:0}";

        public void SetDistance(float distance)
        {
            if (_distanceText != null) _distanceText.text = string.Format(_format, distance);
        }
    }
}
