using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestApp.API.Helpers;
using TestApp.API.Models;

namespace TestApp.API.Data
{
    public class BlogRepository : IBlogRepository
    {
        private readonly DataContext _context;
        public BlogRepository(DataContext context)
        {
            _context = context;
        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<Blog> GetBlog(int id)
        {
            var query = _context.Blogs.AsQueryable();

            var blog = await query.FirstOrDefaultAsync(b => b.Id == id);

            return blog;
        }

        public async Task<PagedList<Blog>> GetBlogs(BlogParams blogParams)
        {
            var blogs = _context.Blogs.OrderByDescending(c => c.Created).AsQueryable();

            return await PagedList<Blog>.CreateAsync(blogs, blogParams.PageNumber, blogParams.PageSize);
        }

        public async Task<Comment> GetComment(int id)
        {
            return await _context.Comments.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Comment>> GetComments(int id)
        {
            var comments = await _context.Comments
            .Where(m => m.BlogId == id).ToListAsync();

            return comments;
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}