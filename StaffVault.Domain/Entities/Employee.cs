namespace StaffVault.Domain.Entities
{
    public sealed class Employee
    {
        public int Id { get; init; }
        public required string Name { get; init; }
        public required DateOnly DateOfBirth { get; init; }
        public required string Email { get; init; }
        public string? PicturePath { get; init; }
        public DateTime CreatedOnUtc { get; init; }
        public DateTime UpdatedOnUtc { get; init; }
    }
}
