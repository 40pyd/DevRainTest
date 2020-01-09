using System;

namespace TestApp.API.Dtos
{
    public class UserForDetailedDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
    }
}