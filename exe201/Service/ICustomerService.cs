using exe201.ViewModel;

namespace exe201.Service
{
    public interface ICustomerService
    {
        Task<CustomerProfileViewModel?> GetCustomerProfileAsync(int userId);
    }
}