using MiraculousSurfers.Economy;
using TMPro;
using UnityEngine;
using VContainer;

namespace MiraculousSurfers.Gameplay.UI
{
    public class PlayerWalletView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _balanceText;

        private IWalletService _walletService;
        private bool _subscribed;

        [Inject]
        public void Construct(IWalletService wallet)
        {
            _walletService = wallet;
        }

        private void OnEnable()
        {
            Subscribe();
            Refresh();
        }

        private void OnDisable() => Unsubscribe();

        private void Subscribe()
        {
            if (_walletService == null || _subscribed || !isActiveAndEnabled) return;
            _walletService.Changed += Refresh;
            _subscribed = true;
        }

        private void Unsubscribe()
        {
            if (_walletService == null || !_subscribed) return;
            _walletService.Changed -= Refresh;
            _subscribed = false;
        }

        private void Refresh()
        {
            if (_balanceText != null) _balanceText.text = _walletService.Coins.ToString();
        }
    }
}
