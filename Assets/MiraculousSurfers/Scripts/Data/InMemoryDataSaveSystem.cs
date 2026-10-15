using System.Collections.Generic;
using MiraculousSurfers.Core;

namespace MiraculousSurfers.Data
{
    public abstract class InMemoryDataSaveSystem<TData> : DataSaveSystem<TData>
    {
        private readonly Dictionary<string, string> _store = new();

        protected override IResult<string> ReadRaw(string key)
        {
            return _store.TryGetValue(key, out string json) ? Result.Ok(json) : Result.Ok<string>(null);
        }

        protected override IResult<bool> WriteRaw(string key, string json)
        {
            _store[key] = json;
            return Result.Ok(true);
        }
    }
}
