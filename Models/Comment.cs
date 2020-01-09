using System;

namespace TestApp.API.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public virtual User Sender { get; set; }
        public int BlogId { get; set; }
        public virtual Blog Blog { get; set; }
        public string Content { get; set; }
        public DateTime CommentSent { get; set; }
    }
}