using System;

namespace STOCKWEBAPI.ServiceInterface.Login
{
    public interface ILoginService
    {
        Task<dynamic> Authenticate(string username, string password);

    }
}

