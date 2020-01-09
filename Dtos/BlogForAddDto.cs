using System;
using System.ComponentModel.DataAnnotations;

namespace TestApp.API.Dtos
{
    public class BlogForAddDto
    {
        [Required]
        public string Content { get; set; }
        public DateTime Created { get; set; }
        public BlogForAddDto()
        {
            Created = DateTime.Now;
        }
    }
}