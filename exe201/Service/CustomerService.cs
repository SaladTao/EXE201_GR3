using exe201.Repository;
using exe201.ViewModel;

namespace exe201.Service
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repo;
        public CustomerService(ICustomerRepository repo)
        {
            _repo = repo;
        }

        public async Task<CustomerProfileViewModel?> GetCustomerProfileAsync(int userId)
        {
            return await _repo.GetCustomerProfileAsync(userId);
        }
    }
}