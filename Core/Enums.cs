using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public enum ProjectServerMode
    {
        Test = 1,
        Release = 2
    }

    public enum ClientState
    {
        Wait = 1,
        Play = 2,
        Pause = 3
    }
}
