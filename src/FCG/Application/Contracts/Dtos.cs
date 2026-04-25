namespace FCG.Application.Contracts;

public record RegisterRequest(string Name, string Email, string Password);

public record LoginRequest(string Email, string Password);

public record AuthResponse(string Token, DateTime ExpiresAtUtc, Guid UserId, string Name, string Email, string Role);

public record CreateGameRequest(string Title, string Genre, decimal Price);

public record GameResponse(Guid Id, string Title, string Genre, decimal Price);

public record CreatePromotionRequest(
    string Title,
    string? Description,
    decimal DiscountPercent,
    DateTime ValidFromUtc,
    DateTime ValidToUtc,
    Guid? GameId);

public record PromotionResponse(
    Guid Id,
    string Title,
    string? Description,
    decimal DiscountPercent,
    DateTime ValidFromUtc,
    DateTime ValidToUtc,
    Guid? GameId);

public record UserSummaryResponse(Guid Id, string Name, string Email, string Role);

public record UpdateUserRoleRequest(string Role);
