using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MiraculousSurfers.Gameplay
{
    public class EvaluateDistanceState : GameplayState
    {
        [Header("Refs")]
        [SerializeField] private Character _character;
        [SerializeField] private ScoreManager _scoreManager;

        [Header("Flow")]
        [SerializeField] private GameplayState _newRecordState;
        [SerializeField] private GameplayState _tryEndedState;

        protected override UniTask OnEnterAsync(CancellationToken ct)
        {
            bool isRecord = _scoreManager.Submit(_character.TraveledDistance);
            Machine.GoTo(isRecord ? _newRecordState : _tryEndedState).Forget();
            return UniTask.CompletedTask;
        }
    }
}
