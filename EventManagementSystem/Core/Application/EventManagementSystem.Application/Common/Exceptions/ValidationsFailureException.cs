namespace EventManagementSystem.Application.Common.Exceptions
{
    public class ValidationsFailureException : Exception
    {
        public ValidationsFailureException(Dictionary<string, List<string>> errors)
            : base("One or more validation failures occurred.")
        {
            this.Errors = errors;
        }

        public Dictionary<string, List<string>> Errors { get; }
    }
}
