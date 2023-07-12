using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PracticalNineteen.Db.DatabaseContext;
using PracticalNineteen.Db.Interfaces;
using PracticalNineteen.Models.ViewModels;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PracticalNineteen.Db.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserDbContext _userDbContext;


        public UserRepository(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<IdentityUser> signInManager, UserDbContext userDbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _userDbContext = userDbContext;
        }

        public async Task<UserManagerRespose> LoginUserAsync(LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new UserManagerRespose
                {
                    Message = "There is no user with this email try again!",
                    IsSuccess = false,
                };
            }
            var result = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!result)
            {
                return new UserManagerRespose
                {
                    Message = "Invalid Password try again!",
                    IsSuccess = false,
                };
            }
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>()
            {
                new Claim("Email", model.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),

            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
            }

            await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
            var keys = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("abcdefghijklmnopqrstuvwxyz"));

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(2),
                signingCredentials: new SigningCredentials(keys, SecurityAlgorithms.HmacSha256)
                );
            string tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);
            return new UserManagerRespose
            {
                Message = tokenAsString,
                IsSuccess = true,
                ExpireDate = token.ValidTo,
                Email = user.Email,
            };
        }

        public async Task<UserManagerRespose> RegisterUserAsync(RegisterViewModel model)
        {
            var email = await _userManager.FindByEmailAsync(model.Email);
            if (email != null)
            {
                return new UserManagerRespose
                {
                    Message = $"{email} already exist!"
                };
            }
            var IdetityUser = new IdentityUser()
            {
                Email = model.Email,
                UserName = model.Email
            };
            var result = await _userManager.CreateAsync(IdetityUser, model.Password);

            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync("Admin"))
                {
                    await _roleManager.CreateAsync(new IdentityRole() { Name = "Admin" });
                }

                if (!await _roleManager.RoleExistsAsync("User"))
                {
                    await _roleManager.CreateAsync(new IdentityRole() { Name = "User" });
                }
                await _userManager.AddToRoleAsync(IdetityUser, "Admin");

                return new UserManagerRespose
                {
                    Message = "User Created Successfully!",
                    IsSuccess = true,
                };
            }

            return new UserManagerRespose
            {
                Message = "User not created try again!",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public async Task<UserManagerRespose> LogoutUserAsync(Logout model)
        {
            await _signInManager.SignOutAsync();
            return new UserManagerRespose
            {
                Message = "User Signout",
                IsSuccess = true,
            };
        }
        
        public async Task<IEnumerable<RegisteredUser>> GetUsers()
        {

            List<RegisteredUser> list = new List<RegisteredUser>();

            foreach (var item in await _userManager.GetUsersInRoleAsync("Admin"))
            {
                list.Add(new RegisteredUser()
                {
                    Email = item.Email,
                    Role = "Admin",
                });
            }

            foreach (var item in await _userManager.GetUsersInRoleAsync("User"))
            {
                list.Add(new RegisteredUser()
                {
                    Email = item.Email,
                    Role = "User",
                });
            }


            return list;
        }

    }
}
