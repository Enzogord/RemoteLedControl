using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.RemoteOperations
{
    public interface ISequenceTimeProvider
    {
        TimeSpan CurrentTime { get; }
    }
}
