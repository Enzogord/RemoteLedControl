using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace NotifiedObjectsFramework
{
    internal class Notifier
    {
        internal event EventHandler OnNotified;

        internal Notifier(INotifyPropertyChanged notifierObject, IList<string> propertyNames)
        {
            notifierObject.PropertyChanged += (s, e) => {
                if(propertyNames.Any(x => x == e.PropertyName)) {
                    OnNotified?.Invoke(this, EventArgs.Empty);
                }
            };
        }
    }
}
