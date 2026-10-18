using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MiraculousSurfers.Core;

namespace MiraculousSurfers.Data
{
    public abstract class InMemoryDataSaveSystem<TData> : DataSaveSystem<TData>
    {
        private readonly Dictionary<string, string> _store = new();

        protected override UniTask<IResult<string>> LoadInternalAsync(string key)
        {
            var resultValue = _store.TryGetValue(key, out string json) ? Result.Ok(json) : Result.Ok<string>(null);
            return UniTask.FromResult(resultValue);
        }

        protected override UniTask<IResult<bool>> SaveInternalAsync(string key, string json)
        {
            _store[key] = json;
            var resultValue = Result.Ok(true);
            return UniTask.FromResult(resultValue);
        }
    }
}
