using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class RLCProjectController : NotifyPropertyBase
    {
        private Project currentProject;

        public Project CurrentProject {
            get => currentProject;
            set {
                currentProject = value;
                SetField(ref currentProject, value, () => CurrentProject);
            }
        }

        public RLCProjectController()
        {

        }

        public void LoadProject()
        {

        }

        public void SaveProject()
        {

        }
    }
}
