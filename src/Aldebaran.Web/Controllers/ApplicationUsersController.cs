using Aldebaran.Web.Data;
using Aldebaran.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Aldebaran.Web.Controllers
{
    [Authorize]
    [Route("odata/Identity/ApplicationUsers")]
    public partial class ApplicationUsersController : ODataController
    {
        private readonly ApplicationIdentityDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;

        public ApplicationUsersController(ApplicationIdentityDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            this.context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        partial void OnUsersRead(ref IQueryable<ApplicationUser> users);

        [EnableQuery]
        [HttpGet]
        public IEnumerable<ApplicationUser> Get()
        {
            var users = userManager.Users;
            OnUsersRead(ref users);

            return users;
        }

        [EnableQuery]
        [HttpGet]
        [Route("UsersByRole/{roleId}")]
        public async Task<IEnumerable<ApplicationUser>> GetUsersByRole(string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
                return null;
            var usersInRole = await userManager.GetUsersInRoleAsync(role.Name);
            return usersInRole;
        }

        [EnableQuery]
        [HttpGet("{Id}")]
        public SingleResult<ApplicationUser> GetApplicationUser(string key)
        {
            var user = context.Users.Where(i => i.Id == key);

            return SingleResult.Create(user);
        }

        partial void OnUserDeleted(ApplicationUser user);

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(string key)
        {
            var user = await userManager.FindByIdAsync(key);

            if (user == null)
            {
                return NotFound();
            }

            OnUserDeleted(user);

            var result = await userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                return IdentityError(result);
            }

            return new NoContentResult();
        }

        partial void OnUserUpdated(ApplicationUser user);

        [HttpPatch("{Id}")]
        public async Task<IActionResult> Patch(string key, [FromBody] ApplicationUser data)
        {
            var user = await userManager.FindByIdAsync(key);

            if (user == null)
            {
                return NotFound();
            }

            OnUserUpdated(data);

            IdentityResult result = null;

            user.Roles = null;

            result = await userManager.UpdateAsync(user);

            if (data.Roles != null)
            {
                result = await userManager.RemoveFromRolesAsync(user, await userManager.GetRolesAsync(user));

                if (result.Succeeded)
                {
                    result = await userManager.AddToRolesAsync(user, data.Roles.Select(r => r.Name));
                }
            }

            if (!string.IsNullOrEmpty(data.Password))
            {
                result = await userManager.RemovePasswordAsync(user);

                if (result.Succeeded)
                {
                    result = await userManager.AddPasswordAsync(user, data.Password);
                }

                if (!result.Succeeded)
                {
                    return IdentityError(result);
                }
            }

            if (result != null && !result.Succeeded)
            {
                return IdentityError(result);
            }

            return new NoContentResult();
        }

        partial void OnUserCreated(ApplicationUser user);

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ApplicationUser data)
        {
            var username = data.UserName;
            var email = data.Email;
            var password = data.Password;
            var user = new ApplicationUser { UserName = username, Email = email, EmailConfirmed = true };
            try
            {

                IdentityResult result = await userManager.CreateAsync(user, password);
                if (result.Succeeded && data.Roles != null)
                {
                    result = await userManager.AddToRolesAsync(user, data.Roles.Select(r => r.Name));
                }

                if (result.Succeeded)
                {
                    OnUserCreated(user);

                    return Created($"odata/Identity/Users('{user.Id}')", user);
                }
                else
                {
                    return IdentityError(result);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        partial void OnLockUser(ApplicationUser user);
        partial void OnUnlockUser(ApplicationUser user);

        [Route("LockUser/{key}")]
        public async Task<IActionResult> LockUser(string key)
        {
            var user = await userManager.FindByIdAsync(key);

            if (user == null)
            {
                return NotFound();
            }

            OnLockUser(user);
            var result = await userManager.SetLockoutEnabledAsync(user, true);
            if (result != null && !result.Succeeded)
            {
                return IdentityError(result);
            }

            return new NoContentResult();
        }

        [Route("UnlockUser/{key}")]
        public async Task<IActionResult> UnlockUser(string key)
        {
            var user = await userManager.FindByIdAsync(key);

            if (user == null)
            {
                return NotFound();
            }

            OnUnlockUser(user);
            var result = await userManager.SetLockoutEnabledAsync(user, false);
            if (result != null && !result.Succeeded)
            {
                return IdentityError(result);
            }

            return new NoContentResult();
        }

        private IActionResult IdentityError(IdentityResult result)
        {
            var message = string.Join(", ", result.Errors.Select(error => error.Description));

            return BadRequest(new { error = new { message } });
        }
    }
}