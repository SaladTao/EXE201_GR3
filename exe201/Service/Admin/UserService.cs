using exe201.Models;
using System;

namespace exe201.Service.Admin
{
    public class UserService : IUserService
    {
        // Giả sử bạn có DbContext hoặc repo để lấy user
        private readonly EcommerceContext _context;
        public UserService(EcommerceContext context)
        {
            _context = context;
        }

        public User GetUserById(int id)
        {
            return _context.Users.FirstOrDefault(u => u.Id == id);
        }

        public UserProfile GetUserProfileById(int id)
        {
            return _context.UserProfiles.FirstOrDefault(up => up.UserId == id);
        }
    }

}
