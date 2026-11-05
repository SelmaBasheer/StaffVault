namespace StaffVault.Domain.Rules
{
    public static class EmployeeRules
    {
        public const int NameMaxLength = 100;

        public const int EmailMaxLength = 254;

        public const int PicturePathMaxLength = 260;

        public const long MaxPictureSizeBytes = 2 * 1024 * 1024; // 2 MB

        private static readonly string[] PictureExtensions = [".jpg", ".jpeg", ".png"];

        public static readonly IReadOnlySet<string> AllowedPictureExtensions = new HashSet<string>(
            PictureExtensions,
            StringComparer.OrdinalIgnoreCase
        );

        public static readonly string AllowedPictureExtensionsDisplay = string.Join(
            ", ",
            PictureExtensions
        );
    }
}
