using System.Threading;
using Cysharp.Threading.Tasks;
using MiraculousSurfers.Economy;
using TMPro;
using UnityEngine;
using VContainer;

namespace MiraculousSurfers.Gameplay.UI
{
    public class PlayerWalletView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _balanceText;
        [SerializeField] private float _countDuration = 0.4f;

        private IWalletService _walletService;
        private bool _subscribed;
        private int _displayedCoins;
        private CancellationTokenSource _countCts;

        [Inject]
        public void Construct(IWalletService wallet)
        {
            _walletService = wallet;
        }

        private void OnEnable()
        {
            Subscribe();
            SnapToBalance();
        }

        private void OnDisable()
        {
            Unsubscribe();
            CancelCount();
        }

        private void OnDestroy() => CancelCount();

        private void Subscribe()
        {
            if (_walletService == null || _subscribed || !isActiveAndEnabled) return;
            _walletService.Changed += OnWalletChanged;
            _subscribed = true;
        }

        private void Unsubscribe()
        {
            if (_walletService == null || !_subscribed) return;
            _walletService.Changed -= OnWalletChanged;
            _subscribed = false;
        }

        private void OnWalletChanged() => AnimateTo(_walletService.Coins);

        private void SnapToBalance()
        {
            if (_walletService == null) return;
            CancelCount();
            _displayedCoins = _walletService.Coins;
            SetText(_displayedCoins);
        }

        private void AnimateTo(int target)
        {
            CancelCount();
            _countCts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
            CountAsync(_displayedCoins, target, _countCts.Token).Forget();
        }

        private async UniTaskVoid CountAsync(int from, int to, CancellationToken ct)
        {
            if (_countDuration <= 0f || from == to)
            {
                _displayedCoins = to;
                SetText(to);
                return;
            }

            var elapsed = 0f;
            while (elapsed < _countDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / _countDuration);
                _displayedCoins = Mathf.RoundToInt(Mathf.Lerp(from, to, t));
                SetText(_displayedCoins);
                if (await UniTask.Yield(PlayerLoopTiming.Update, ct).SuppressCancellationThrow()) return;
            }

            _displayedCoins = to;
            SetText(to);
        }

        private void SetText(int value)
        {
            if (_balanceText != null) _balanceText.text = value.ToString();
        }

        private void CancelCount()
        {
            _countCts?.Cancel();
            _countCts?.Dispose();
            _countCts = null;
        }
    }
}
