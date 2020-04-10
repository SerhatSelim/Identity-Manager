using IDM.WebApi.Persistence.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace IDM.WebApi.Controllers
{
    [Route("api/User")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public UserController(UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpPost]
        [Route("CreateUserAsync")]
        [Authorize(Policy = "UserClaimPositionPolicy")]
        public async Task<IdentityResult> CreateUserAsync([FromBody] User user)
        {
            var result = await userManager.CreateAsync(user, "1234.qqqQ");

            return result;
        }

        [HttpPost]
        [Route("LoginWClaim")]
        public async Task<Microsoft.AspNetCore.Identity.SignInResult> LoginWClaim(UserDto model)
        {
            User user = await userManager.FindByEmailAsync(model.Email);
            var userClaims = await userManager.GetClaimsAsync(user);
           
                Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.PasswordSignInAsync(user, model.Password, true, true);
                if (result.Succeeded)
                {
                    System.Security.Claims.Claim claim = new System.Security.Claims.Claim("pozisyon", "admin");
                    if (!userClaims.Any(x => x.Type == "pozisyon"))
                        await userManager.AddClaimAsync(user, claim);
                  
                }

            return result;
        }



        [HttpPost]
        [Route("Login")]
        public async Task<Microsoft.AspNetCore.Identity.SignInResult> Login([FromBody] UserDto userDto)
        {
            User user = await userManager.FindByEmailAsync(userDto.Email.ToString());

            //İlgili kullanıcıya dair önceden oluşturulmuş bir Cookie varsa siliyoruz.
            await signInManager.SignOutAsync();
            var result = await signInManager.PasswordSignInAsync(user, userDto.Password, userDto.Persistent, userDto.Lock);
            if (result.Succeeded)
            {
                await userManager.ResetAccessFailedCountAsync(user); //Önceki hataları girişler neticesinde +1 arttırılmış tüm değerleri 0(sıfır)a çekiyoruz.


            }
            else
            {
                await userManager.AccessFailedAsync(user); //Eğer ki başarısız bir account girişi söz konusu ise AccessFailedCount kolonundaki değer +1 arttırılacaktır. 

                int failcount = await userManager.GetAccessFailedCountAsync(user); //Kullanıcının yapmış olduğu başarısız giriş deneme adedini alıyoruz.
                if (failcount == 3)
                {
                    await userManager.SetLockoutEndDateAsync(user, new DateTimeOffset(DateTime.Now.AddMinutes(1))); //Eğer ki başarısız giriş denemesi 3'ü bulduysa ilgili kullanıcının hesabını kilitliyoruz.
                }
                else
                {
                    //if (result.IsLockedOut)
                    //    return result;
                    //else
                    //    return result;
                    //return result;
                    return result;
                }
            }


            return result;
        }





        [HttpPost]
        [Route("PasswordReset")]
        [Authorize(Roles = "Admin")]
        public async Task<string> PasswordReset([FromBody] UserDto userDto)
        {
            User user = await userManager.FindByEmailAsync(userDto.Email);
            if (user != null)
            {
                string resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
                return resetToken;
            }
            return "PasswordReset";
        }

        [HttpPost("UpdatePassword/{userId}/{token}")]
        public async Task<IdentityResult> UpdatePassword(UserDto model, string userId, string token)
        {
            User user = await userManager.FindByIdAsync(userId);
            IdentityResult result = await userManager.ResetPasswordAsync(user, HttpUtility.UrlDecode(token), model.Password);
            if (result.Succeeded)
            {

                await userManager.UpdateSecurityStampAsync(user);
            }
            return result;

        }

        [HttpPost]
        [Route("EditProfile")]
        public async Task<IdentityResult> EditProfile(UserDto model)
        {

            User user = await userManager.FindByNameAsync(User.Identity.Name);
            user.PhoneNumber = model.PhoneNumber;
            IdentityResult result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                result.Errors.ToList().ForEach(e => ModelState.AddModelError(e.Code, e.Description));
                return result;
            }
            await userManager.UpdateSecurityStampAsync(user);
            await signInManager.SignOutAsync();
            await signInManager.SignInAsync(user, true);

            return result;
        }

        [HttpPost]
        [Route("EditPassword")]
        public async Task<bool> EditPassword(UserDto model)
        {

            User user = await userManager.FindByNameAsync(User.Identity.Name);
            if (await userManager.CheckPasswordAsync(user, model.OldPassword))
            {
                IdentityResult result = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (!result.Succeeded)
                {
                    result.Errors.ToList().ForEach(e => ModelState.AddModelError(e.Code, e.Description));
                    return true;
                }
                await userManager.UpdateSecurityStampAsync(user);
                await signInManager.SignOutAsync();
                await signInManager.SignInAsync(user, true);
            }

            return false;
        }

        [HttpPost]
        [Route("Logout")]
        public async Task<bool> Logout()
        {
            await signInManager.SignOutAsync();
            return true;
        }
    }
}