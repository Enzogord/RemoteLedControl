﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLCCore
{
    public interface ISettingsProvider
    {
        IDictionary<string, string> GetSettings();
    }
}
