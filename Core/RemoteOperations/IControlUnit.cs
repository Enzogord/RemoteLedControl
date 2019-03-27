using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLCCore.RemoteOperations
{
    public interface IControlUnit
    {
        int Key { get; set; }
        int Port { get; set; }
        ObservableCollection<IRemoteClient> Clients { get; set; }
    }
}
