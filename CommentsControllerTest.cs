using System;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TestApp.API.Controllers;
using TestApp.API.Data;
using TestApp.API.Dtos;
using TestApp.API.Models;
using Xunit;

namespace TestApp.API
{
    public class CommentsControllerTest
    {
        CommentsController _controller;

        public CommentsControllerTest()
        {
            var mockUserRepository = new Mock<IUserRepository>();
            var mockBlogRepository = new Mock<IBlogRepository>();
            var mockMapper = new Mock<IMapper>();
            _controller = new CommentsController
            (
                mockBlogRepository.Object,
                mockUserRepository.Object,
                mockMapper.Object
            );
        }

        [Fact]
        public void Add_IvalidObjectPassed_ReturnsBadRequest()
        {
            // Arrange
            var commentWithMissingProperty = new CommentForCreationDto
            {
                SenderId = 5,
                BlogId = 999,
                CommentSent = DateTime.Now
            };

            _controller.ModelState.AddModelError("Content", "Required");

            // Act
            var badResponse = _controller.CreateComment(5, commentWithMissingProperty);

            //Assert
            Assert.IsType<BadRequestObjectResult>(badResponse);
        }

        [Fact]
        public void Add_ValidObjectPassed_ReturnsCreatedAtRouteRequest()
        {
            // Arrange
            var commentWithMissingProperty = new CommentForCreationDto
            {
                SenderId = 5,
                BlogId = 1,
                CommentSent = DateTime.Now,
                Content = "Content"
            };

            // Act
            var createdResponse = _controller.CreateComment(5, commentWithMissingProperty);

            //Assert
            Assert.IsType<CreatedAtRouteResult>(createdResponse);
        }
    }
}