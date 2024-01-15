using System;

namespace NuiN.NExtensions
{
    public interface IPoolabeObject<T>
    {
        Action<T> ReleaseToPool { get; set; }
    }
}
