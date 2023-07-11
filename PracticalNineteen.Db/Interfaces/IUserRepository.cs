using PracticalNineteen.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticalNineteen.Db.Interfaces
{
    public interface IUserRepository
    {
        Task<UserManagerRespose> RegisterUserAsync(RegisterViewModel model);
        Task<UserManagerRespose> LoginUserAsync(LoginViewModel model);
        Task<UserManagerRespose> LogoutUserAsync(Logout model);
    }
}
