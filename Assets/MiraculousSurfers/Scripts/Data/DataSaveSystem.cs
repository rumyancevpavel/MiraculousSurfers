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

        public async UniTask<IResult<TData>> LoadAsync(CancellationToken cancellation = default)
        {
            var read = await LoadInternalAsync(StorageKey);
            if (!read.Success)
            {
                return Result.Fail<TData>(read.Error);
            }

            if (string.IsNullOrEmpty(read.Value))
            {
                return Result.Ok(CreateDefault());
            }

            try
            {
                var data = JsonConvert.DeserializeObject<TData>(read.Value);
                if (data == null)
                {
                    return Result.Ok(CreateDefault());
                }

                return Result.Ok(data);
            }
            catch (Exception exception)
            {
                return Result.Fail<TData>(exception);
            }
        }

        public async UniTask<IResult<bool>> SaveAsync(TData data, CancellationToken cancellation = default)
        {
            if (!IsValid(data))
            {
                return Result.Fail<bool>(new InvalidOperationException($"Cannot save {typeof(TData).Name}: data is invalid."));
            }

            try
            {
                string json = JsonConvert.SerializeObject(data);
                return await SaveInternalAsync(StorageKey, json);
            }
            catch (Exception exception)
            {
                return Result.Fail<bool>(exception);
            }
        }

        protected virtual bool IsValid(TData data) => data != null;

        protected abstract UniTask<IResult<string>> LoadInternalAsync(string key);
        protected abstract UniTask<IResult<bool>> SaveInternalAsync(string key, string json);
    }
}
