using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace NotifiedObjectsFramework
{
    public class NotifierSelector<TNotifierObject> : NotifierProvider
        where TNotifierObject : class, INotifyPropertyChanged
    {
        private readonly TNotifierObject notifierObject;
        private readonly NotificationBinder binder;
        private NotifierConfig<TNotifierObject> notifierConfig;
        internal NotifierSelector(TNotifierObject notifierObject, NotificationBinder binder)
        {
            this.notifierObject = notifierObject ?? throw new ArgumentNullException(nameof(notifierObject));
            this.binder = binder ?? throw new ArgumentNullException(nameof(binder));
        }

        public NotifierConfig<TNotifierObject> BindToProperty(Expression<Func<TNotifierObject, object>> propertySelectorExpr)
        {
            notifierConfig = new NotifierConfig<TNotifierObject>(notifierObject, binder);
            notifierConfig.BindToProperty(propertySelectorExpr);
            return notifierConfig;
        }

        internal override Notifier GetNotifier()
        {
            return notifierConfig?.GetNotifier();
        }
    }
}
