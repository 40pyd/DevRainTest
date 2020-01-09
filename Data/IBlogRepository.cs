using System.Collections.Generic;
using System.Threading.Tasks;
using TestApp.API.Helpers;
using TestApp.API.Models;

namespace TestApp.API.Data
{
    public interface IBlogRepository : IRepository
    {
        Task<Blog> GetBlog(int id);
        Task<PagedList<Blog>> GetBlogs(BlogParams blogParams);
        Task<Comment> GetComment(int id);
        Task<IEnumerable<Comment>> GetComments(int id);
    }
}