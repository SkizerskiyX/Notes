using NoteModels.Dto;

namespace NotesServices.Interfaces
{
    public interface IAuthServices
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
        Task<AuthResponseDto> RefreshAsync(RefreshTokenRequestDto request);
        Task RevokeAsync(RefreshTokenRequestDto request);
    }
}
