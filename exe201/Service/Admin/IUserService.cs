using exe201.Models;

namespace exe201.Service.Admin
{
    public interface IUserService
    {
        User GetUserById(int id);
        UserProfile GetUserProfileById(int id);
    }
}
