using exe201.ViewModel;

namespace exe201.Repository
{
    public interface ICustomerRepository
    {
        Task<CustomerProfileViewModel?> GetCustomerProfileAsync(int userId);
    }
}