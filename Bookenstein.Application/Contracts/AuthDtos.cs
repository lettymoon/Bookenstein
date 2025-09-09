using System;

namespace Bookenstein.Application.Contracts
{
        public record SignupRequest(string Name, string Username, string Email, string Password);
        public record SignupBody(string Name, string Email, string Password);
        public record LoginRequest(string EmailOrUsername, string Password);
        public record RefreshRequest(string RefreshToken);
        public record AuthResponse(string AccessToken, string RefreshToken, DateTime ExpiresAtUtc);
        public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
}
