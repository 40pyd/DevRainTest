using System;
using System.Collections.Generic;

namespace TestApp.API.Models
{
    public class Blog
    {
        public int Id { get; set; } 
        public string Content { get; set; }
        public DateTime Created { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}