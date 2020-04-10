namespace IDM.WebApi.Persistence.Models
{
    public class RoleAssignDto
    {
        public int RoleId { get; set; }
        public int UserId { get; set; }
        public string RoleName { get; set; }
        public bool HasAssign { get; set; }
    }
}
