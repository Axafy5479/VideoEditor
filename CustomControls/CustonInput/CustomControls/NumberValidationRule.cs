using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ParameterEditor
{
    public class NumberValidationRule : ValidationRule
    {


        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string? inputText = value as string;
            if (inputText is not string s) return new ValidationResult(false, "数値のみ入力可能です");

            if(!double.TryParse(s, out double n)) return new ValidationResult(false, "数値のみ入力可能です");

            return new ValidationResult(true, null);
        }
    }
}
