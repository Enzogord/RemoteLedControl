using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace NotifiedObjectsFramework
{
    public class NotifierConfig<TNotifierObject> : NotifierProvider
        where TNotifierObject : class, INotifyPropertyChanged
    {
        private readonly TNotifierObject notifierObject;
        private readonly NotificationBinder binder;
        private List<string> propertyNameList = new List<string>();

        internal NotifierConfig(TNotifierObject notifierObject, NotificationBinder binder)
        {
            this.notifierObject = notifierObject ?? throw new ArgumentNullException(nameof(notifierObject));
            this.binder = binder ?? throw new ArgumentNullException(nameof(binder));
        }

        public NotifierConfig<TNotifierObject> BindToProperty(Expression<Func<TNotifierObject, object>> propertySelectorExpr)
        {
            binder.CheckBindingFinished();
            string propertyName = PropertyUtility.GetPropertyName(propertySelectorExpr);
            if (propertyNameList.Contains(propertyName))
            {
                return this;
            }
            propertyNameList.Add(propertyName);
            return this;
        }

        public NotifierSelector<TNotifier> SetNotifier<TNotifier>(TNotifier notifierObject)
            where TNotifier : NotifyPropertyChangedBase
        {
            return binder.NotifiedObjectActionConfig.SetNotifier(notifierObject);
        }

        public void End()
        {
            binder.FinishBinding();
        }

        internal override Notifier GetNotifier()
        {
            if (!propertyNameList.Any())
            {
                return null;
            }
            return new Notifier(notifierObject, propertyNameList);
        }
    }
}
