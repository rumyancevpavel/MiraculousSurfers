using Cysharp.Threading.Tasks;
using MiraculousSurfers.Data;
using MiraculousSurfers.Gameplay.Events;
using UnityEngine;
using VContainer;

namespace MiraculousSurfers.Gameplay.Score
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] private GameObject _indicatorPrefab;
        [SerializeField] private Transform _indicatorParent;

        private IDataSaveSystem<PlayerHighScoreData> _save;
        private GameplayEventBus _events;
        private PlayerHighScoreData _data;
        private GameObject _indicator;

        public float Best => _data != null ? _data.BestDistance : 0f;
        public bool IsNewRecord { get; private set; }

        [Inject]
        public void Construct(IDataSaveSystem<PlayerHighScoreData> save, GameplayEventBus events)
        {
            _save = save;
            _events = events;
            _data = save.CreateDefault();
            _events.Subscribe<RunEnded>(GameplayEventType.RunEnded, OnRunEnded);
        }

        private void OnDestroy()
        {
            _events?.Unsubscribe<RunEnded>(GameplayEventType.RunEnded, OnRunEnded);
        }

        private void Start()
        {
            LoadAndPlace().Forget();
        }

        private async UniTaskVoid LoadAndPlace()
        {
            var result = await _save.LoadAsync(this.GetCancellationTokenOnDestroy());
            _data = result.Success ? result.Value : _save.CreateDefault();
            if (_data.HasRecord) PlaceIndicator(new Vector3(_data.PosX, _data.PosY, _data.PosZ));
        }

        private void OnRunEnded(RunEnded runEnded)
        {
            if (runEnded.Distance <= _data.BestDistance)
            {
                IsNewRecord = false;
                return;
            }

            _data.BestDistance = runEnded.Distance;
            _data.PosX = runEnded.Position.x;
            _data.PosY = runEnded.Position.y;
            _data.PosZ = runEnded.Position.z;
            _data.HasRecord = true;
            IsNewRecord = true;

            _save.SaveAsync(_data).Forget();
            PlaceIndicator(runEnded.Position);
            _events.Publish(GameplayEventType.NewHighScore, new NewHighScore(runEnded.Distance, runEnded.Position));
        }

        private void PlaceIndicator(Vector3 position)
        {
            if (_indicatorPrefab == null) return;
            if (_indicator == null) _indicator = Instantiate(_indicatorPrefab, position, Quaternion.identity, _indicatorParent);
            else _indicator.transform.position = position;
        }
    }
}
