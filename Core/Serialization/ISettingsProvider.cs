using System.Collections.Generic;

namespace RLCCore.Serialization
{
    public interface ISettingsProvider
    {
        IDictionary<string, string> GetSettings();
    }
}
