namespace ReadingTracker.Domain.Constants;

public static class UserConstants
{
    // Email constraints
    public const int MaxEmailLength = 256;

    // Display Name constraints
    public const int MinDisplayNameLength = 2;
    public const int MaxDisplayNameLength = 50;

    // Password policy constraints
    public const int MinPasswordLength = 8;
    public const int MaxPasswordLength = 100;
    public const bool RequireDigit = true;
    public const bool RequireNonAlphanumeric = false;
}