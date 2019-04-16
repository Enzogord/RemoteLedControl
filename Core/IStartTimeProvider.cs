using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLCCore
{
    public interface IStartTimeProvider
    {
        TimeSpan StartTime { get; set; }
    }
}
