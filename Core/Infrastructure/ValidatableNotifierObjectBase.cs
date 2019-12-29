using NotifiedObjectsFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Core.Infrastructure
{
    public abstract class ValidatableNotifierObjectBase : NotifyPropertyChangedBase, IValidatableObject, INotifyDataErrorInfo, IDataErrorInfo
    {
        private Dictionary<string, List<string>> propertyErrors = new Dictionary<string, List<string>>();

        private void AddPropertyError(string propertyName, string error)
        {
            if(!propertyErrors.ContainsKey(propertyName)) {
                propertyErrors.Add(propertyName, new List<string>());
            }
            propertyErrors[propertyName].Add(error);
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        private void ClearDataErrors()
        {
            foreach(var error in propertyErrors) {
                error.Value.Clear();
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(error.Key));
            }
        }

        private string GetValidationResultText()
        {
            Validate();
            string result = "";
            foreach(var error in propertyErrors.SelectMany(x => x.Value)) {
                result += $"{error}{Environment.NewLine}";
            }
            return result;
        }

        private string GetValidationResultText(string memberName)
        {
            Validate();
            string result = "";
            if(!propertyErrors.ContainsKey("memberName")) {
                return result;
            }
            foreach(var error in propertyErrors[memberName]) {
                result += $"{error}{Environment.NewLine}";
            }
            return result;
        }

        #region INotifyDataErrorInfo

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public bool HasErrors => propertyErrors.Values.Any(x => x.Any());

        public IEnumerable GetErrors(string propertyName)
        {
            return propertyErrors[propertyName];
        }

        #endregion

        #region IDataErrorInfo implementation

        public string Error => GetValidationResultText();

        public string this[string columnName] => GetValidationResultText(columnName);

        #endregion IDataErrorInfo implementation

        #region IValidatableObject implementation

        public IEnumerable<ValidationResult> Validate()
        {
            return Validate(new ValidationContext(this));
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            ClearDataErrors();
            List<ValidationResult> results = new List<ValidationResult>();
            foreach(var vr in ValidateWithDataErrorNotification(validationContext)) {
                foreach(var memberName in vr.MemberNames) {
                    AddPropertyError(memberName, vr.ErrorMessage);
                }
                results.Add(vr);
            }
            return results;
        }

        #endregion IValidatableObject implementation        

        protected abstract IEnumerable<ValidationResult> ValidateWithDataErrorNotification(ValidationContext validationContext);

        protected bool ValidatableSetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            bool result = base.SetField(ref field, value, propertyName);
            if(result) {
                Validate();
            }
            return result;
        }
    }
}
