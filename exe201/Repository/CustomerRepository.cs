using exe201.Models;
using exe201.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace exe201.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly EcommerceContext _context;
        public CustomerRepository(EcommerceContext context)
        {
            _context = context;
        }

        public async Task<CustomerProfileViewModel?> GetCustomerProfileAsync(int userId)
        {
            return await (from up in _context.UserProfiles
                          join u in _context.Users
                          on up.UserId equals u.Id
                          where up.UserId == userId
                          select new CustomerProfileViewModel
                          {
                              FullName = up.FullName,
                              Address = up.Address,
                              Phone = up.Phone,
                              AvatarUrl = up.AvatarUrl,
                              Email = u.Email
                          }).FirstOrDefaultAsync();
        }
    }
}