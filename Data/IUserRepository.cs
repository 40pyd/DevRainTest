using System.Collections.Generic;
using System.Threading.Tasks;
using TestApp.API.Models;

namespace TestApp.API.Data
{
    public interface IUserRepository: IRepository
    {
        Task<User> GetUser(int id);
        Task<IEnumerable<User>> GetUsers();
    }
}