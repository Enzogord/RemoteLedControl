using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Core.IO
{
    public class RemovableDrivesProvider
    {
        public IEnumerable<string> GetRemovebleDrives()
        {
            return DriveInfo.GetDrives().Where(x => x.DriveType == DriveType.Removable).Select(x => x.Name);
        }
    }
}
