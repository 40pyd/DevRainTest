using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TestApp.API.Data;
using TestApp.API.Dtos;
using TestApp.API.Models;

namespace TestApp.API.Controllers
{
    [Route("api/blogs/{userId}/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly IBlogRepository _repo;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepo;
        public CommentsController(IBlogRepository repo, IUserRepository userRepo, IMapper mapper)
        {
            _userRepo = userRepo;
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetComments(int id)
        {
            var comments = await _repo.GetComments(id);

            var commentsToReturn = _mapper.Map<IEnumerable<CommentForReturnDto>>(comments);

            return Ok(commentsToReturn);
        }

        [HttpGet("{id}", Name = "GetComment")]
        public async Task<IActionResult> GetComment(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var commentFromRepo = await _repo.GetComment(id);

            if (commentFromRepo == null)
                return NotFound();

            return Ok(commentFromRepo);
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment(int userId, CommentForCreationDto commentForCreationDto)
        {
            var sender = await _userRepo.GetUser(userId); // for automapper magic!!!!
            if (sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            commentForCreationDto.SenderId = userId;

            var blog = await _repo.GetBlog(commentForCreationDto.BlogId); // for automapper magic!!!!

            if (blog == null)
                return BadRequest("Could not find the blog");

            var comment = _mapper.Map<Comment>(commentForCreationDto);

            _repo.Add(comment);

            if (await _repo.SaveAll())
            {
                var commentToReturn = _mapper.Map<CommentForReturnDto>(comment);
                return CreatedAtRoute("GetComment", new { userId, id = comment.Id }, commentToReturn);
            }

            throw new Exception("Failed to create the comment");
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteComment(int id, int userId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var commentFormRepo = await _repo.GetComment(id);

            _repo.Delete(commentFormRepo);

            if (await _repo.SaveAll())
                return NoContent();

            throw new Exception("Error deleting the comment");
        }
    }
}