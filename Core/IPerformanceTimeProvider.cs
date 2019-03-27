using System;

namespace RLCCore
{
    /// <summary>
    /// Предоставляет текущение время выступления
    /// </summary>
    public interface IPerformanceTimeProvider
    {
        DateTime CurrentTime { get; }
    }
}
