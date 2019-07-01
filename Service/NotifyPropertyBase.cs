using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace Service
{
    public abstract class NotifyPropertyBase : INotifyPropertyChanged
    {
        public virtual event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) {
                return false;
            }
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged<T>(Expression<Func<T>> selectorExpression)
        {
            if (selectorExpression == null) {
                throw new ArgumentNullException("selectorExpression");
            }
            MemberExpression body = selectorExpression.Body as MemberExpression;
            if (body == null) {
                throw new ArgumentException("The body must be a member expression");
            }
            OnPropertyChanged(body.Member.Name);
        }

        protected bool SetField<T>(ref T field, T value, Expression<Func<T>> selectorExpression)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) {
                return false;
            }
            field = value;
            OnPropertyChanged(selectorExpression);
            return true;
        }

        #region Подписки на изменения свойств

        public void Bind<T, TBindObject>(Expression<Func<T>> subjectPropertyExpr, TBindObject bindObject, params Expression<Func<TBindObject, object>>[] bindObjectPropertyExprs)
            where TBindObject : class, INotifyPropertyChanged
        {
            List<string> bindPropertyList = bindObjectPropertyExprs.Select(x => ReflectionUtils.GetName(x)).ToList();
            bindObject.PropertyChanged += (sender, e) => {
                if(bindPropertyList.Contains(e.PropertyName)) {
                    OnPropertyChanged(subjectPropertyExpr);
                }
            };
        }

        public void BindAction<TBindObject>(Action trigeredAction, TBindObject bindObject, params Expression<Func<TBindObject, object>>[] bindObjectPropertyExprs)
            where TBindObject : class, INotifyPropertyChanged
        {
            List<string> bindPropertyList = bindObjectPropertyExprs.Select(x => ReflectionUtils.GetName(x)).ToList();
            bindObject.PropertyChanged += (sender, e) => {
                if(bindPropertyList.Contains(e.PropertyName)) {
                    trigeredAction.Invoke();
                }
            };
        }

        #endregion
    }
}
