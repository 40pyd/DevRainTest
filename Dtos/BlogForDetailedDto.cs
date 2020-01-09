using System.Collections.Generic;
using TestApp.API.Models;

namespace TestApp.API.Dtos
{
    public class BlogForDetailedDto
    {
        public int Id { get; set; } 
        public string Content { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}