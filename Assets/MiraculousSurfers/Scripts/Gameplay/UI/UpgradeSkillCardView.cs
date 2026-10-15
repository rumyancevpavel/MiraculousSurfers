using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MiraculousSurfers.Gameplay.UI
{
    public class UpgradeSkillCardView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _upgradeProgressText;
        [SerializeField] private TMP_Text _upgradeNameText;
        [SerializeField] private Slider _upgradeProgressSlider;
        [SerializeField] private Button _upgradeButton;
    }
}