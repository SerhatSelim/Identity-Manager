using Microsoft.AspNetCore.Identity;
using System;

namespace IDM.WebApi.Persistence.Models
{
    public class User : IdentityUser<int>
    {
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenEndDate { get; set; }
    }
}
