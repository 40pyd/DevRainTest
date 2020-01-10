using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestApp.API.Data;
using TestApp.API.Dtos;
using TestApp.API.Helpers;
using TestApp.API.Models;

namespace TestApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController: ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IBlogRepository _repo;
        private readonly IUserRepository _userRepo;
        private readonly DataContext _context;

        public BlogsController(
            IBlogRepository repo, 
            IMapper mapper, 
            IUserRepository userRepo, 
            DataContext context)
        {
            _context = context;
            _userRepo = userRepo;
            _repo = repo;
            _mapper = mapper;
        }

        // get list of blogs using parameters user added on client-side filters
        [HttpGet]
        public async Task<IActionResult> GetBlogs([FromQuery]BlogParams blogParams)
        {
            var blogs = await _repo.GetBlogs(blogParams);
            var blogsToReturn = _mapper.Map<List<BlogForListDto>>(blogs);

            // adding new headers to response for pagination
            // static method from Extension class in "Helpers" folder
            Response.AddPagination(blogs.CurrentPage, blogs.PageSize,
                blogs.TotalCount, blogs.TotalPages);

            return Ok(blogsToReturn);
        }

        // method to get single blog for detailed page
        [HttpGet("{id}", Name = "GetBlog")]
        public async Task<IActionResult> GetBlog(int id)
        {
            var blog = await _repo.GetBlog(id);
            var blogToReturn = _mapper.Map<BlogForDetailedDto>(blog);

            return Ok(blogToReturn);
        }

        // method for adding new blog
        [HttpPost("{id}")]
        public async Task<IActionResult> AddBlog(int id, BlogForAddDto blogForAddDto)
        {
            // if user is not authorized => return 400 NotAuthorized
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var blogToAdd = _mapper.Map<Blog>(blogForAddDto);
            blogToAdd.UserId = id;

            _repo.Add(blogToAdd);

            // if blog adding to db is successfull => return http response status 200 Ok with blog info
            if (await _repo.SaveAll())
                return Ok(blogToAdd);

            throw new Exception($"Adding blog failed");
        }

        // method to change parameters of existing blog
        [HttpPut("{userId}/{id}")]
        public async Task<IActionResult> UpdateBlog(int userId, int id,
            BlogForUpdateDto blogForUpdateDto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var blogFromRepo = await _repo.GetBlog(id);
            _mapper.Map(blogForUpdateDto, blogFromRepo);
            blogFromRepo.Created = DateTime.Now;

            if (await _repo.SaveAll())
                return NoContent();

            throw new Exception($"Updating blog {id} failed on save");
        }

        // method to delete the blog
        [HttpDelete("{userId}/{id}")]
        public async Task<IActionResult> DeleteBlog(int userId, int id)
        {
            // if not authorized => return 400 NotAuthorized
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            // search for the user
            var user = await _userRepo.GetUser(userId);

            // search for the blog
            var blogFromRepo = await _repo.GetBlog(id);

            // if blog is not one of users blogs => return 400 NotAuthorized
            if (blogFromRepo.UserId != userId)
                return Unauthorized();

            _repo.Delete(blogFromRepo);

            // if delete was successfull => return http response status 200
            if (await _repo.SaveAll())
                return Ok();

            return BadRequest("Failed to delete the blog");
        }

        // method for admin panel to delete the blog
        [Authorize(Policy = "RequireAdminRole")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> AdminDeleteBlog(int id)
        {
            var carFromRepo = await _repo.GetBlog(id);

            _repo.Delete(carFromRepo);

            if (await _repo.SaveAll())
                return Ok();

            return BadRequest("Failed to delete the blog");
        }
    }
}