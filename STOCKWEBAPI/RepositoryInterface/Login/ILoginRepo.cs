using System.Threading.Tasks;

namespace STOCKWEBAPI.RepositoryInterface.Login
{
    public interface ILoginRepo
    {
        Task<dynamic> ValidateUser(string username, string password);
    }
}
