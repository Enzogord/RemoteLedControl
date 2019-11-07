using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace NotifiedObjectsFramework
{
    public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        public virtual event PropertyChangedEventHandler PropertyChanged;

        internal void InternalRaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region With string property name

        /// <summary>
        /// Raise PropertyChanged event for property with this name (<paramref name="propertyName"/>)
        /// IMPORTANT: Call without <paramref name="propertyName"/> only in property
        /// </summary>
        protected void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Set <paramref name="value"/> to <paramref name="field"/> and raise PropertyChanged event for property with this name (<paramref name="propertyName"/>)
        /// IMPORTANT: Call without <paramref name="propertyName"/> only in property
        /// </summary>
        protected bool SetField<T>(ref T field, T value, [CallerMemberName]string propertyName = "")
        {
            if(EqualityComparer<T>.Default.Equals(field, value)) {
                return false;
            }
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion With string property name

        #region With expression property selector

        protected void OnPropertyChanged<T>(Expression<Func<T>> selectorExpression)
        {
            if(selectorExpression == null) {
                throw new ArgumentNullException("selectorExpression");
            }
            MemberExpression body = selectorExpression.Body as MemberExpression;
            if(body == null) {
                throw new ArgumentException("The body must be a member expression");
            }
            OnPropertyChanged(body.Member.Name);
        }

        protected bool SetField<T>(ref T field, T value, Expression<Func<T>> selectorExpression)
        {
            if(EqualityComparer<T>.Default.Equals(field, value)) {
                return false;
            }
            field = value;
            OnPropertyChanged(selectorExpression);
            return true;
        }

        #endregion With expression property selector

        #region Property changed notification binder

        private List<NotifierSet> notificationBinders = new List<NotifierSet>();

        public NotificationBinder CreateNotificationBinding()
        {
            var binding = new NotificationBinder(this);
            binding.OnFinishBinding += (s, e) => {
                notificationBinders.Add(e);
            };
            return binding;
        }

        public void ClearNotificationBindings()
        {
            notificationBinders.Clear();
        }

        #endregion Property changed notification binder
    }
}
