using System.Security.Claims;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace DotnetAPI.Controllers 
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly DataContextDapper _dapper;

        public PostController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("Posts/{postId}/{userId}/{searchParam}")]
        public IEnumerable<Post> GetPosts(int postId = 0, int userId = 0, string searchParam = "None")
        {
            string getPostsSql = @"EXEC TutorialAppSchema.spPosts_GET";
            string parameters = "";

            if(postId != 0)
            {
                parameters += ", @PostId = " + postId.ToString();
            }

            if(userId != 0)
            {
                parameters += ", @UserId = " + userId.ToString();
            }

            if(searchParam.ToLower() != "none")
            {
                parameters += ", @SearchValue = '" + searchParam + "'";
            }

            if (parameters.Length > 0)
            {
                getPostsSql += parameters.Substring(1);
            }
            Console.WriteLine(parameters);
            Console.WriteLine(getPostsSql);
            return _dapper.LoadData<Post>(getPostsSql);
        }

        [HttpGet("MyPosts")]
        public IEnumerable<Post> GetMyPosts()
        {
            string getPostsSql = @"EXEC TutorialAppSchema.spPosts_GET @UserId = " + this.User.FindFirst("userId")?.Value;

            return _dapper.LoadData<Post>(getPostsSql);
        }

        [HttpPut("UpsertPost")]
        public IActionResult UpsertPost(Post postUpsert)
        {
            string sql = @"EXEC TutorialAppSchema.spPosts_Upsert
                @UserId = " + this.User.FindFirst("userId")?.Value + 
                ", @PostTitle = '" + postUpsert.PostTitle + 
                "', @PostContent = '" + postUpsert.PostContent + "'";   

                if(postUpsert.PostId > 0)
                {
                    sql += ", @PostId = " + postUpsert.PostId;
                }


            if(_dapper.ExecuteSql(sql))
            {
                return Ok();
            }
            throw new Exception ("Failed to create post");
        }

        [HttpDelete("Post/{postId}")]
        public IActionResult DeletePost(int postId)
        {
            string sql = @"EXEC TutorialAppSchema.spPost_Delete
                @PostId = " + postId.ToString() +
                ", @UserId = " + this.User.FindFirst("userId")?.Value;
            if(_dapper.ExecuteSql(sql))
            {
                return Ok();
            }
            throw new Exception ("Failed to delete post");
        }
    }
}