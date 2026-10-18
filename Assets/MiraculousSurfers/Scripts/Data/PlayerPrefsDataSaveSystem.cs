using System;
using Cysharp.Threading.Tasks;
using MiraculousSurfers.Core;
using UnityEngine;

namespace MiraculousSurfers.Data
{
    public abstract class PlayerPrefsDataSaveSystem<TData> : DataSaveSystem<TData>
    {
        protected override UniTask<IResult<string>> LoadInternalAsync(string key)
        {
            IResult<string> result = default;
            try
            {
                if (!PlayerPrefs.HasKey(key))
                {
                    result = Result.Ok<string>(null);
                    return UniTask.FromResult(result);
                }

                result = Result.Ok(PlayerPrefs.GetString(key));
                return UniTask.FromResult(result);
            }
            catch (Exception exception)
            {
                result = Result.Fail<string>(exception);
                return UniTask.FromResult(result);
            }
        }

        protected override UniTask<IResult<bool>> SaveInternalAsync(string key, string json)
        {
            try
            {
                PlayerPrefs.SetString(key, json);
                PlayerPrefs.Save();
                var result = Result.Ok(true);
                return UniTask.FromResult(result);
            }
            catch (Exception exception)
            {
                var result = Result.Fail<bool>(exception);
                return UniTask.FromResult(result);
            }
        }
    }
}
