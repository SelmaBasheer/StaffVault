namespace StaffVault.Domain.Exceptions
{
    public sealed class DuplicateEmailException : DomainException
    {
        public string Email { get; }
        private static string BuildMessage(string email) => $"An employee with email '{email}' already exists.";

        public DuplicateEmailException(string email)
            : base(BuildMessage(email))
        {
            Email = email;
        }

        public DuplicateEmailException(string email, Exception innerException)
            : base(BuildMessage(email), innerException)
        {
            Email = email;
        }
    }
}
