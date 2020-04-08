using IDM.WebApi.Persistence.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IDM.WebApi.Persistence.Context
{
    public class IdmContext : IdentityDbContext<User>
    {
        public IdmContext(DbContextOptions<IdmContext> dbContext) : base(dbContext) { }
 
         
    }
}
