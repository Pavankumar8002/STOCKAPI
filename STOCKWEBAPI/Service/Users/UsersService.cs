using STOCKWEBAPI.DataEntities.Users;
using STOCKWEBAPI.RepositoryInterface.Users;
using STOCKWEBAPI.ServiceInterface.Users;

namespace STOCKWEBAPI.Service.Users
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepo _usersRepo;

        public UsersService(IUsersRepo usersRepo)
        {
            _usersRepo = usersRepo;
        }

        //public dynamic SaveUsers(UsersRequestClass request)
        //{
        //    return _usersRepo.SaveUsers(request);
        //}
        public async Task<dynamic> SaveUsers(UsersRequestClass request)
        {
            return await _usersRepo.SaveUsers(request);
        }

    }
}
