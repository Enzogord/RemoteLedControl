using System;

namespace NotifiedObjectsFramework
{
    internal class NotifiedAction
    {
        internal bool IsPropertyChangedAction => !string.IsNullOrEmpty(PropertyName);
        internal Action Action { get; set; }
        internal string PropertyName { get; set; }

        public NotifiedAction(Action action)
        {
            Action = action;
        }

        public NotifiedAction(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}
