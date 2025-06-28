using STOCKWEBAPI.RepositoryInterface.Login;
using STOCKWEBAPI.ServiceInterface.Login;

namespace STOCKWEBAPI.Service.Login
{
    public class LoginService : ILoginService
    {
        private readonly ILoginRepo _loginRepo;

        public LoginService(ILoginRepo loginRepo)
        {
            _loginRepo = loginRepo;
        }

        //public dynamic Authenticate(string username, string password)
        //{
        //    return _loginRepo.ValidateUser(username, password);
        //}
        public async Task<dynamic> Authenticate(string username, string password)
        {
            return await _loginRepo.ValidateUser(username, password);
        }

    }
}
