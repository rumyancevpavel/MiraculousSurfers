using System.Threading;
using Cysharp.Threading.Tasks;
using MiraculousSurfers.Gameplay.Score;
using UnityEngine;

namespace MiraculousSurfers.Gameplay.StateMachine.States
{
    public class EvaluateDistanceState : GameplayState
    {
        [Header("Refs")]
        [SerializeField] private ScoreManager _scoreManager;

        [Header("Flow")]
        [SerializeField] private GameplayState _newRecordState;
        [SerializeField] private GameplayState _tryEndedState;

        protected override UniTask OnEnterAsync(CancellationToken ct)
        {
            Machine.GoTo(_scoreManager.IsNewRecord ? _newRecordState : _tryEndedState).Forget();
            return UniTask.CompletedTask;
        }
    }
}
