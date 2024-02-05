using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace yoga.Helpers
{
    public class CheckBoxRequired : ValidationAttribute, IClientModelValidator
    {
        public override bool IsValid(object value)
        {
            if (value is bool)
            {
                return (bool)value;
            }
 
            return false;
        }
        
        public void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes.Add("data-val-checkboxrequired", FormatErrorMessage(context.ModelMetadata.GetDisplayName()));
        }

    }
}