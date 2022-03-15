using Ich.Saas.Core.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Ich.Saas.Core.Code.Identity
{
    // ** Facade Design Pattern

    // ** Service Design Pattern

    #region Interface

    public interface IIdentityService
    {
        Task InsertUserAsync(User user, string password, params string[] roles);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(User user);

        Task<SignInResult> PasswordSignInAsync(string email, string password);
        Task SignOutAsync();
        Task RefreshClaimsAsync(User user);
        Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword);

        Task CreateRole(string role);
    }

    #endregion

    #region Implementation

    public class IdentityService : IIdentityService
    {
        #region Dependency Injection

        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SaaSContext _db;

        public IdentityService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager, SaaSContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _db = db;
        }

        #endregion

        public async Task<SignInResult> PasswordSignInAsync(string email, string password)
        {
            var user = _db.User.Single(u => u.Email == email);

            // This triggers 'CreateAsync' in ClaimsPrincipalFactory
            return await _signInManager.PasswordSignInAsync(user.IdentityName, password, true, false);
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task InsertUserAsync(User user, string password, params string[] roles)
        {
            try
            {
                user.IdentityName = Guid.NewGuid().ToString();
                var appUser = new IdentityUser
                {
                    UserName = user.IdentityName,
                    Email = user.Email
                }; // provide email, but not used

                if (password != null)
                    await _userManager.CreateAsync(appUser, password);
                else
                    await _userManager.CreateAsync(appUser); // user with external login

                await _userManager.AddToRolesAsync(appUser, roles);

                user.IdentityId = appUser.Id;
                _db.User.Add(user);
                await _db.SaveChangesAsync();

            }
            catch (Exception ex)
            {

                throw new Exception("In InsertUserAsync :", ex);
            }
        }

        public async Task UpdateUserAsync(User user)
        {
            try
            {
                var original = _db.User.Single(u => u.Id == user.Id);

                if (original.Role != user.Role)
                    await ChangeRoleAsync(user, original.Role);

                if (original.Email != user.Email)
                    await ChangeEmailAsync(user);

                // cannot track 2 entites with same key: 'original' and 'user'

                _db.Entry(original).State = EntityState.Detached;

                _db.User.Update(user);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw new Exception("InUpdateUserAsync :", ex);
            }
        }

        public async Task DeleteUserAsync(User user)
        {
            try
            {
                var appUser = await _userManager.FindByIdAsync(user.IdentityId);

                await _userManager.RemoveFromRoleAsync(appUser, user.Role);
                await _userManager.DeleteAsync(appUser);

                _db.User.Remove(user);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw new Exception("InDeleteUserAsync :", ex);
            }
        }

        public async Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
        {
            var appUser = await _userManager.FindByIdAsync(user.IdentityId);
            return await _userManager.ChangePasswordAsync(appUser, currentPassword, newPassword);
        }

        public async Task RefreshClaimsAsync(User user)
        {
            var appUser = await _userManager.FindByIdAsync(user.IdentityId);

            // This triggers 'CreateAsync' in ClaimsPrincipalFactory
            await _signInManager.RefreshSignInAsync(appUser);
        }

        private async Task ChangeRoleAsync(User user, string oldRole)
        {
            var appUser = await _userManager.FindByIdAsync(user.IdentityId);

            await _userManager.RemoveFromRoleAsync(appUser, oldRole);

            await _userManager.AddToRoleAsync(appUser, user.Role);
        }

        private async Task ChangeEmailAsync(User user)
        {
            var appUser = await _userManager.FindByIdAsync(user.IdentityId);
            await _userManager.SetEmailAsync(appUser, user.Email);
        }

        public async Task CreateRole(string role)
        {
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    #endregion
}
