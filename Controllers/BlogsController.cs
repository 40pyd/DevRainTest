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
        public BlogsController(IBlogRepository repo, IMapper mapper, IUserRepository userRepo, DataContext context)
        {
            _context = context;
            _userRepo = userRepo;
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetBlogs([FromQuery]BlogParams blogParams)
        {
            var blogs = await _repo.GetBlogs(blogParams);
            var blogsToReturn = _mapper.Map<List<BlogForListDto>>(blogs);

            Response.AddPagination(blogs.CurrentPage, blogs.PageSize,
                blogs.TotalCount, blogs.TotalPages);

            return Ok(blogsToReturn);
        }

        [HttpGet("{id}", Name = "GetBlog")]
        public async Task<IActionResult> GetBlog(int id)
        {
            var blog = await _repo.GetBlog(id);
            var blogToReturn = _mapper.Map<BlogForDetailedDto>(blog);

            return Ok(blogToReturn);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> AddBlog(int id, BlogForAddDto blogForAddDto)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var blogToAdd = _mapper.Map<Blog>(blogForAddDto);
            blogToAdd.UserId = id;

            _repo.Add(blogToAdd);

            if (await _repo.SaveAll())
                return Ok(blogToAdd);

            throw new Exception($"Adding blog failed");
        }

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

        [HttpDelete("{userId}/{id}")]
        public async Task<IActionResult> DeleteBlog(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var user = await _userRepo.GetUser(userId);

            var blogFromRepo = await _repo.GetBlog(id);

            if (blogFromRepo.UserId != userId)
                return Unauthorized();

            _repo.Delete(blogFromRepo);

            if (await _repo.SaveAll())
                return Ok();

            return BadRequest("Failed to delete the blog");
        }

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