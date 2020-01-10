using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using TestApp.API.Models;

namespace TestApp.API.Data
{
    public class Seed
    {
        // method for seeding several simple users, creating roles and admin user
        public static void SeedUsers(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            if (!userManager.Users.Any())
            {
                // reading json file with 3 users info
                var userData = System.IO.File.ReadAllText("../TestApp.API/Data/UserSeedData.json");
                
                // getting list of users from json string
                var users = JsonConvert.DeserializeObject<List<User>>(userData);

                // creating roles
                var roles = new List<Role>
                {
                    new Role {Name="Member"},
                    new Role {Name="Admin"},
                    new Role {Name="Moderator"}
                };

                // adding roles to db
                foreach (var role in roles)
                {
                    roleManager.CreateAsync(role).Wait();
                }

                // adding users to db and adiing them to member role
                foreach (var user in users)
                {
                    userManager.CreateAsync(user, "password").Wait();
                    userManager.AddToRoleAsync(user, "Member");
                }

                // creating and adding admin user
                var adminUser = new User
                {
                    UserName = "Admin"
                };

                var result = userManager.CreateAsync(adminUser, "Pa$$w0rd").Result;

                if (result.Succeeded)
                {
                    var admin = userManager.FindByNameAsync("Admin").Result;
                    // adding admin to two roles 
                    userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });
                }
            }
        }
    }
}