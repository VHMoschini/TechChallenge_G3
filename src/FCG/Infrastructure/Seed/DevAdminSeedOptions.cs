namespace FCG.Infrastructure.Seed;

public class DevAdminSeedOptions
{
    public const string SectionName = "Seed";

    public string? AdminEmail { get; set; }
    public string? AdminPassword { get; set; }
    public string AdminName { get; set; } = "Administrador";
}
