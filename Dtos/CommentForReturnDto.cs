using System;

namespace TestApp.API.Dtos
{
    public class CommentForReturnDto
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int BlogId { get; set; }
        public string SenderKnownAs { get; set; }
        public string Content { get; set; }
        public DateTime CommentSent { get; set; }
    }
}