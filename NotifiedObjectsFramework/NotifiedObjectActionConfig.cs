using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace NotifiedObjectsFramework
{
    public class NotifiedObjectActionConfig
    {
        private readonly NotificationBinder binder;

        public NotifiedObjectActionConfig(NotificationBinder binder)
        {
            this.binder = binder ?? throw new ArgumentNullException(nameof(binder));
        }

        public NotifiedObjectActionConfig AddProperty<T>(params Expression<Func<T>>[] propertySelectorExpr)
        {
            return binder.AddProperty(propertySelectorExpr);
        }

        public NotifiedObjectActionConfig AddProperty(params string[] propertyNames)
        {
            return binder.AddProperty(propertyNames);
        }

        public NotifiedObjectActionConfig AddAction(params Action[] actions)
        {
            return binder.AddAction(actions);
        }

        public NotifierSelector<TNotifier> SetNotifier<TNotifier>(TNotifier notifierObject)
            where TNotifier : class, INotifyPropertyChanged
        {
            binder.CheckBindingFinished();
            var notifierSelector = new NotifierSelector<TNotifier>(notifierObject, binder);
            binder.AddBindingInfoSource(notifierSelector);
            return notifierSelector;
        }
    }
}
