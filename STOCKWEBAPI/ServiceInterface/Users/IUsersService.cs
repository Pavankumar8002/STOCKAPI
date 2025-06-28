using STOCKWEBAPI.DataEntities.Users;

namespace STOCKWEBAPI.ServiceInterface.Users
{
    public interface IUsersService
    {
        //dynamic SaveUsers(UsersRequestClass request);
        Task<dynamic> SaveUsers(UsersRequestClass request);

    }
}
