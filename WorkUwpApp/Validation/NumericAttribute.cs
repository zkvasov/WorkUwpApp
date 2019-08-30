using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkUwpApp.Validation
{
    /// <summary>
    /// Sample custom validation for Integer values.
    /// </summary>
    public class NumericAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            // The [Required] attribute should test this.
            if (value == null)
            {
                return true;
            }

            int result;
            return int.TryParse(value.ToString(), out result);
        }
    }
}
