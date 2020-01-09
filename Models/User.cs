using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace TestApp.API.Models
{
    public class User: IdentityUser<int>
    {
        public virtual ICollection<Blog> Blog { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}