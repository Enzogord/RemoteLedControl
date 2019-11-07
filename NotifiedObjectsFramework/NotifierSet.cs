using System;
using System.Collections.Generic;

namespace NotifiedObjectsFramework
{
    public class NotifierSet
    {
        private readonly NotifyPropertyChangedBase notifiedObject;
        private readonly IEnumerable<NotifiedAction> notifiedActions;

        internal List<Notifier> Notifiers { get; } = new List<Notifier>();

        internal NotifierSet(NotifyPropertyChangedBase notifiedObject, IEnumerable<NotifiedAction> notifiedActions)
        {
            this.notifiedObject = notifiedObject ?? throw new System.ArgumentNullException(nameof(notifiedObject));
            this.notifiedActions = notifiedActions ?? throw new System.ArgumentNullException(nameof(notifiedActions));
        }

        internal void AddNotifier(Notifier notifier)
        {
            Notifiers.Add(notifier);
            notifier.OnNotified += BindingInfo_OnNotified;
        }

        private void BindingInfo_OnNotified(object sender, EventArgs e)
        {
            foreach (var notifiedAction in notifiedActions){
                if(notifiedAction.IsPropertyChangedAction){
                    notifiedObject.InternalRaisePropertyChanged(notifiedAction.PropertyName);
                }
                else{
                    notifiedAction.Action?.Invoke();
                }
            }
        }
    }
}
