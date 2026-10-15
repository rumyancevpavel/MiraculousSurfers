using System.Threading;
using Cysharp.Threading.Tasks;
using MiraculousSurfers.Core;

namespace MiraculousSurfers.Data
{
    public interface IDataSaveSystem<TData>
    {
        TData CreateDefault();
        UniTask<IResult<TData>> LoadAsync(CancellationToken cancellation = default);
        UniTask<IResult<bool>> SaveAsync(TData data, CancellationToken cancellation = default);
    }
}
