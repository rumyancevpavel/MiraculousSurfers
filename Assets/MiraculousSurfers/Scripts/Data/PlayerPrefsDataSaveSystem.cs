using System;
using MiraculousSurfers.Core;
using UnityEngine;

namespace MiraculousSurfers.Data
{
    public abstract class PlayerPrefsDataSaveSystem<TData> : DataSaveSystem<TData>
    {
        protected override IResult<string> ReadRaw(string key)
        {
            try
            {
                if (!PlayerPrefs.HasKey(key))
                {
                    return Result.Ok<string>(null);
                }

                return Result.Ok(PlayerPrefs.GetString(key));
            }
            catch (Exception exception)
            {
                return Result.Fail<string>(exception);
            }
        }

        protected override IResult<bool> WriteRaw(string key, string json)
        {
            try
            {
                PlayerPrefs.SetString(key, json);
                PlayerPrefs.Save();
                return Result.Ok(true);
            }
            catch (Exception exception)
            {
                return Result.Fail<bool>(exception);
            }
        }
    }
}
