using RentFlow.Domain.Entities;

namespace RentFlow.Application.Interfaces;

public interface IAuthTokenGenerator
{
    string Generate(User user);
}
