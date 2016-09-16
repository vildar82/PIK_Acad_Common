using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PIK_Acad_Common.Rename.UI
{
    public class SymbolNameValidator : ValidationRule
    {
        public override ValidationResult Validate
          (object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value != null && RenameSymbolTableRecordService.IsValidName(value.ToString()))
            {
                return new ValidationResult(false, "Недопустимое имя");
            }
            return ValidationResult.ValidResult;
        }
    }
}
