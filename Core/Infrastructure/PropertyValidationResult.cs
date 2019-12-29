using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Infrastructure
{
    public class PropertyValidationResult : ValidationResult
    {
        public PropertyValidationResult(string errorMessage, params string[] memberNames) : base(errorMessage, memberNames)
        {
        }
    }
}
