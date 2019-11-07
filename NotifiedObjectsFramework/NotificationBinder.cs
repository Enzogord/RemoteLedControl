using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NotifiedObjectsFramework
{
    public class NotificationBinder
    {
        private bool bindingFinished = false;
        private NotifyPropertyChangedBase notifiedObject;
        private List<NotifiedAction> actionsList = new List<NotifiedAction>();
        private List<NotifierProvider> notifierProviders = new List<NotifierProvider>();
        internal NotifiedObjectActionConfig NotifiedObjectActionConfig { get; }
        internal event EventHandler<NotifierSet> OnFinishBinding;

        internal NotificationBinder(NotifyPropertyChangedBase notifiedObject)
        {
            this.notifiedObject = notifiedObject ?? throw new ArgumentNullException(nameof(notifiedObject));
            NotifiedObjectActionConfig = new NotifiedObjectActionConfig(this);
        }

        internal void FinishBinding()
        {
            var notifierSet = new NotifierSet(notifiedObject, actionsList);
            foreach(var bindingInfoSource in notifierProviders)
            {
                var notifier = bindingInfoSource.GetNotifier();
                if(notifier == null) {
                    continue;
                }
                notifierSet.AddNotifier(notifier);
            }
            OnFinishBinding?.Invoke(this, notifierSet);
            bindingFinished = true;
        }        

        internal void CheckBindingFinished()
        {
            if(bindingFinished) {
                throw new InvalidOperationException("Binding configuration already finished");
            }
        }

        public NotifiedObjectActionConfig AddProperty<T>(params Expression<Func<T>>[] propertySelectorExpr)
        {
            CheckBindingFinished();
            var propertyNames = propertySelectorExpr.Select(x => PropertyUtility.GetPropertyName(x));
            return AddProperty(propertyNames.ToArray());
        }

        public NotifiedObjectActionConfig AddProperty(params string[] propertyNames)
        {
            CheckBindingFinished();
            var validPropertyNames = propertyNames.Where(x => !actionsList.Any(y => y.PropertyName == x));
            actionsList.AddRange(validPropertyNames.Select(x => new NotifiedAction(x)));
            return NotifiedObjectActionConfig;
        }

        public NotifiedObjectActionConfig AddAction(params Action[] actions)
        {
            CheckBindingFinished();
            var validActions = actions.Where(x => !actionsList.Any(y => ReferenceEquals(y.Action, x)));
            actionsList.AddRange(validActions.Select(x => new NotifiedAction(x)));
            return NotifiedObjectActionConfig;
        }

        internal void AddBindingInfoSource(NotifierProvider bindingInfoSource)
        {
            notifierProviders.Add(bindingInfoSource);
        }
    }
}
