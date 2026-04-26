namespace FCG.Application.Contracts;

public record RegisterRequest(string Name, string Email, string Password);

public record LoginRequest(string Email, string Password);

public record AuthResponse(string Token, DateTime ExpiresAtUtc, Guid UserId, string Name, string Email, string Role);

public record CreateGameRequest(string Titulo, string Genero, decimal Preco);

public record GameResponse(Guid Id, string Titulo, string Genero, decimal Preco);

public record CreatePromocaoRequest(
    string Titulo,
    string? Descricao,
    decimal PercentualDesconto,
    DateTime DataPromoInicio,
    DateTime DataPromoFim,
    Guid? GameId);

public record PromocaoResponse(
    Guid Id,
    string Titulo,
    string? Descricao,
    decimal PercentualDesconto,
    DateTime DataPromoInicio,
    DateTime DataPromoFim,
    Guid? GameId);

public record UserSummaryResponse(Guid Id, string Name, string Email, string Role);

public record UpdateUserRoleRequest(string Role);
