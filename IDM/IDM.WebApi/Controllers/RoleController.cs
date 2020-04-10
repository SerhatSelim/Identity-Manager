using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IDM.WebApi.Persistence.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IDM.WebApi.Controllers
{
    [Route("api/Role")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<Role> roleManager;
        private readonly UserManager<User> userManager;

        public RoleController(RoleManager<Role> roleManager, UserManager<User> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        [HttpPost]
        [Route("CreateRole")]
        public async Task<IdentityResult> CreateRole(RoleDto model)
        {
            IdentityResult result = await this.roleManager.CreateAsync(new Role { Name = model.Name });
            return result;
        }

        [HttpPost]
        [Route("GetAllRole")]
        [Authorize(Policy = "TimeControl")]
        public List<Role> GetAllRole ()
        {
            var result = roleManager.Roles.ToList();
           
            return result;
        }

        [HttpPost]
        [Route("UpdateRole")]
        public async Task<IdentityResult> UpdateRole(RoleDto model)
        {
            Role role = await roleManager.FindByIdAsync(model.Id.ToString());
            role.Name = model.Name;
            var result = await roleManager.UpdateAsync(role);
            return result;
        }

        [HttpPost]
        [Route("DeleteRole")]
        public async Task<IdentityResult> DeleteRole(string id)
        {
            Role role = await roleManager.FindByIdAsync(id);
            var result = await roleManager.DeleteAsync(role);
            return result;
        }

        [HttpPost]
        [Route("RoleAssign")]
        public async Task<IdentityResult> RoleAssign(RoleAssignDto roleAssignDto)
        {
           var user = await userManager.FindByIdAsync(roleAssignDto.UserId.ToString());
           var role = await roleManager.FindByIdAsync(roleAssignDto.RoleId.ToString());
           var result =   await userManager.AddToRoleAsync(user, role.Name);
           
            return result;
        }
    }
}