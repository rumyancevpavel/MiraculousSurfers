using System.Threading;
using Cysharp.Threading.Tasks;
using MiraculousSurfers.Configuration;
using MiraculousSurfers.Economy;
using MiraculousSurfers.Gameplay.Characters;
using MiraculousSurfers.Gameplay.Events;
using MiraculousSurfers.Gameplay.Score;
using MiraculousSurfers.Gameplay.Upgrades;
using UnityEngine;
using VContainer;

namespace MiraculousSurfers.Gameplay.StateMachine
{
    public class GameplayStateMachine : MonoBehaviour
    {
        [SerializeField] private GameplayState _initialState;
        [SerializeField] private Character _character;
        [SerializeField] private Animator _characterAnimator;

        public Character Character => _character;
        public Animator CharacterAnimator => _characterAnimator;
        public IUpgradeService Upgrades { get; private set; }
        public GameplayEventBus Events { get; private set; }
        public IScoreManager ScoreManager { get; private set; }
        public IWalletService Wallet { get; private set; }
        public IGameConfigurationSystem Config { get; private set; }

        private GameplayState Current { get; set; }

        private CancellationTokenSource _transitionCts;

        [Inject]
        public void InjectDependencies(
            IUpgradeService upgrades,
            GameplayEventBus events,
            IScoreManager scoreManager,
            IWalletService wallet,
            IGameConfigurationSystem config)
        {
            Upgrades = upgrades;
            Events = events;
            ScoreManager = scoreManager;
            Wallet = wallet;
            Config = config;
        }

        private void Start() => GoTo(_initialState).Forget();

        private void OnDestroy()
        {
            _transitionCts?.Cancel();
            _transitionCts?.Dispose();
        }

        public async UniTask GoTo(GameplayState next)
        {
            if (next == null)
            {
                Debug.LogWarning($"[GameplayStateMachine] GoTo(null) ignored (from {Current?.name}).", this);
                return;
            }

            _transitionCts?.Cancel();
            _transitionCts?.Dispose();
            _transitionCts = new CancellationTokenSource();
            var ct = _transitionCts.Token;

            if (Current != null)
                await Current.ExitAsync(ct);

            if (ct.IsCancellationRequested) return;

            Current = next;
            next.Bind(this);
            await next.EnterAsync(ct);
        }
    }
}
