using ExamManagement.Data;
using Microsoft.EntityFrameworkCore;

public interface ITokenService
{
    Task InvalidateRefreshTokenAsync(string refreshToken);
    Task<bool> IsTokenBlacklisted(string token);
}

public class TokenService : ITokenService
{
    private readonly AppDbContext _context;

    public TokenService(AppDbContext context)
    {
        _context = context;
    }

    public async Task InvalidateRefreshTokenAsync(string refreshToken)
    {
        var token = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken);
        if (token != null)
        {
            token.Revoked = DateTime.UtcNow;
            _context.RefreshTokens.Update(token);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> IsTokenBlacklisted(string token)
    {
        var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
        return refreshToken?.IsActive == false;
    }
}
