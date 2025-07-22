using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagementSystem.Application.Common
{
    public class ValidationResult
    {
        public bool IsValid => Errors.Count == 0;

        public List<string> Errors { get; } = new ();

        public ValidationResult() { }

        public ValidationResult(IEnumerable<string> errors)
        {
            Errors.AddRange(errors);
        }

        public static ValidationResult Success() => new();
        public static ValidationResult Failure(IEnumerable<string> errors) => new (errors);
    }
}
