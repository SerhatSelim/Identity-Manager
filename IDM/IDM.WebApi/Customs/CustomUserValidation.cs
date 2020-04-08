using IDM.WebApi.Persistence.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDM.WebApi.Customs
{
    public class CustomUserValidation : IUserValidator<User>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user)
        {
            List<IdentityError> errors = new List<IdentityError>();

            if (int.TryParse(user.UserName[0].ToString(), out int _)) 
                errors.Add(new IdentityError { Code = "UserNameNumberStartWith", Description = "Kullanıcı adı sayısal ifadeyle başlayamaz..." });
            if (user.UserName.Length < 3 && user.UserName.Length > 25) 
                errors.Add(new IdentityError { Code = "UserNameLenhth", Description = "Kullanıcı adı 3 - 15 karakter arasında olmalıdır..." });
            if (user.Email.Length > 70) 
                errors.Add(new IdentityError { Code = "EmailLenhth", Description = "Email 70 karakterden fazla olamaz..." });

            if (!errors.Any())
                return Task.FromResult(IdentityResult.Success);
            return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
        }
    }