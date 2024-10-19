using System.ComponentModel.DataAnnotations;

namespace GestorDeTareas.Models.Custom
{
    public class ValidEnumDataTypeAttribute : ValidationAttribute
    {
        private readonly Type _enumType;

        public ValidEnumDataTypeAttribute(Type enumType)
        {
            _enumType = enumType;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || !Enum.IsDefined(_enumType, value))
            {
                return new ValidationResult($"El valor '{value}' no es válido para el tipo enum {_enumType.Name}.");
            }

            return ValidationResult.Success;
        }
    }
}
