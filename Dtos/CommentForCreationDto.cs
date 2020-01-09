using System;

namespace TestApp.API.Dtos
{
    public class CommentForCreationDto
    {
        public int SenderId { get; set; }
        public int BlogId { get; set; }
        public DateTime CommentSent { get; set; }
        public string Content { get; set; }
        
        public CommentForCreationDto()
        {
            CommentSent = DateTime.Now;
        }
    }
}