using Microsoft.AspNetCore.Identity;

namespace IDM.WebApi.Persistence.Models
{
    public class UserDto : IdentityUser<int>
    {
        public string Password { get; set; }

        public string OldPassword { get; set; }

        public string NewPassword { get; set; }

        public bool Persistent { get; set; }

        public bool Lock { get; set; }
    }
}
