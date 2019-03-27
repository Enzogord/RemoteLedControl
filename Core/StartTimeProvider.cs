using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLCCore
{
    public class StartTimeProvider : IStartTimeProvider
    {
        //TODO сделать контроль времени с зависимостью на плеер
        public TimeSpan StartTime {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
    }
}
