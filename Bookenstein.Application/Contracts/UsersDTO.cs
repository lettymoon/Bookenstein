namespace Bookenstein.Application.Contracts;
public record UserResponse(
    Guid Id, 
    string Name, 
    string Username, 
    string Email, 
    string Role
);

