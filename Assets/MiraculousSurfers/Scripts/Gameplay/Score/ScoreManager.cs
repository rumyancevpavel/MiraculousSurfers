using Cysharp.Threading.Tasks;
using MiraculousSurfers.Data;
using MiraculousSurfers.Gameplay.Events;
using UnityEngine;
using VContainer;

namespace MiraculousSurfers.Gameplay.Score
{
    public class ScoreManager : MonoBehaviour, IScoreManager
    {
        [SerializeField] private GameObject _indicatorPrefab;
        [SerializeField] private Transform _indicatorParent;

        private IDataSaveSystem<PlayerHighScoreData> _save;
        private GameplayEventBus _events;
        private PlayerHighScoreData _data;
        private GameObject _indicator;

        public float Best => _data != null ? _data.BestDistance : 0f;

        [Inject]
        public void Construct(IDataSaveSystem<PlayerHighScoreData> save, GameplayEventBus events)
        {
            _save = save;
            _events = events;
            _data = save.CreateDefault();
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

        public bool ReportDistance(float distance, Vector3 position)
        {
            if (distance <= _data.BestDistance) return false;

            _data.BestDistance = distance;
            _data.PosX = position.x;
            _data.PosY = position.y;
            _data.PosZ = position.z;
            _data.HasRecord = true;

            _save.SaveAsync(_data).Forget();
            PlaceIndicator(position);
            _events.Publish(GameplayEventType.NewRecord, new GameSignal());
            _events.Publish(GameplayEventType.NewHighScore, new NewHighScore(distance, position));
            return true;
        }

        private void PlaceIndicator(Vector3 position)
        {
            if (_indicatorPrefab == null) return;
            if (_indicator == null) _indicator = Instantiate(_indicatorPrefab, position, Quaternion.identity, _indicatorParent);
            else _indicator.transform.position = position;
        }
    }
}
