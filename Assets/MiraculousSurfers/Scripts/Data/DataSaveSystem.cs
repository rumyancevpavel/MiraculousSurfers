using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MiraculousSurfers.Core;
using Newtonsoft.Json;

namespace MiraculousSurfers.Data
{
    public abstract class DataSaveSystem<TData> : IDataSaveSystem<TData>
    {
        protected abstract string Key { get; }
        protected abstract string Version { get; }

        protected string StorageKey => $"{Key}.{Version}";

        public abstract TData CreateDefault();

        public UniTask<IResult<TData>> LoadAsync(CancellationToken cancellation = default)
        {
            IResult<string> read = ReadRaw(StorageKey);
            if (!read.Success)
            {
                return UniTask.FromResult(Result.Fail<TData>(read.Error));
            }

            if (string.IsNullOrEmpty(read.Value))
            {
                return UniTask.FromResult(Result.Ok(CreateDefault()));
            }

            try
            {
                TData data = JsonConvert.DeserializeObject<TData>(read.Value);
                if (data == null)
                {
                    return UniTask.FromResult(Result.Ok(CreateDefault()));
                }

                return UniTask.FromResult(Result.Ok(data));
            }
            catch (Exception exception)
            {
                return UniTask.FromResult(Result.Fail<TData>(exception));
            }
        }

        public UniTask<IResult<bool>> SaveAsync(TData data, CancellationToken cancellation = default)
        {
            if (!IsValid(data))
            {
                return UniTask.FromResult(Result.Fail<bool>(new InvalidOperationException($"Cannot save {typeof(TData).Name}: data is invalid.")));
            }

            try
            {
                string json = JsonConvert.SerializeObject(data);
                return UniTask.FromResult(WriteRaw(StorageKey, json));
            }
            catch (Exception exception)
            {
                return UniTask.FromResult(Result.Fail<bool>(exception));
            }
        }

        protected virtual bool IsValid(TData data) => data != null;

        protected abstract IResult<string> ReadRaw(string key);
        protected abstract IResult<bool> WriteRaw(string key, string json);
    }
}
