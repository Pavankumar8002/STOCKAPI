using STOCKWEBAPI.DataEntities.Users;

namespace STOCKWEBAPI.RepositoryInterface.Users
{
    public interface IUsersRepo
    {
        //dynamic SaveUsers(UsersRequestClass request);
        Task<dynamic> SaveUsers(UsersRequestClass request);

    }
}
