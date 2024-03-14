// using System.Security.Claims;
// using DotnetAPI.Data;
// using DotnetAPI.Dtos;
// using DotnetAPI.Models;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

// namespace DotnetAPI.Controllers 
// {
//     [Authorize]
//     [ApiController]
//     [Route("[controller]")]
//     public class PostController : ControllerBase
//     {
//         private readonly DataContextDapper _dapper;

//         public PostController(IConfiguration config)
//         {
//             _dapper = new DataContextDapper(config);
//         }

//         [HttpGet("Posts")]
//         public IEnumerable<Post> GetPosts()
//         {
//             string getPostsSql = @"SELECT [PostId],
//                     [UserId],
//                     [PostTitle],
//                     [PostContent],
//                     [PostCreated],
//                     [PostUpdated] FROM TutorialAppSchema.Posts";

//             return _dapper.LoadData<Post>(getPostsSql);
//         }

//         [HttpGet("PostSingle/{postId}")]
//         public Post GetPostSingle(int postId)
//         {
//             string getPostsSql = @"SELECT [PostId],
//                     [UserId],
//                     [PostTitle],
//                     [PostContent],
//                     [PostCreated],
//                     [PostUpdated] FROM TutorialAppSchema.Posts
//                     WHERE PostId = " + postId.ToString();

//             return _dapper.LoadDataSingle<Post>(getPostsSql);
//         }

//         [HttpGet("PostByUser/{userId}")]
//         public IEnumerable<Post> GetPostByUser(int userId)
//         {
//             string getPostsSql = @"SELECT [PostId],
//                     [UserId],
//                     [PostTitle],
//                     [PostContent],
//                     [PostCreated],
//                     [PostUpdated] FROM TutorialAppSchema.Posts
//                     WHERE UserId = " + userId.ToString();

//             return _dapper.LoadData<Post>(getPostsSql);
//         }

//         [HttpGet("MyPosts")]
//         public IEnumerable<Post> GetMyPosts()
//         {
//             string getPostsSql = @"SELECT [PostId],
//                     [UserId],
//                     [PostTitle],
//                     [PostContent],
//                     [PostCreated],
//                     [PostUpdated] FROM TutorialAppSchema.Posts
//                     WHERE UserId = " + this.User.FindFirst("userId")?.Value;

//             return _dapper.LoadData<Post>(getPostsSql);
//         }

//         [HttpPost("Post")]
//         public IActionResult AddPost(PostToAddDto postToAddDto)
//         {
//             string sql = @"INSERT INTO TutorialAppSchema.Posts(
//                 [UserId],
//                 [PostTitle],
//                 [PostContent],
//                 [PostCreated],
//                 [PostUpdated]) 
//                 VALUES (
//                     " + this.User.FindFirst("userId")?.Value + ",'" 
//                     + postToAddDto.PostTitle 
//                     + "', '" + postToAddDto.PostContent 
//                     + "', GETDATE(), GETDATE() )";    
//             if(_dapper.ExecuteSql(sql))
//             {
//                 return Ok();
//             }
//             throw new Exception ("Failed to create post");
//         }

//         [HttpPut("Post")]
//         public IActionResult EditPost(PostToEditDto postToEditDto)
//         {
//             string sql = @"UPDATE TutorialAppSchema.Posts 
//                 SET PostContent = '"+ postToEditDto.PostContent 
//                 + "', PostTitle = '"+ postToEditDto.PostTitle 
//                 + @"', PostUpdated = GETDATE()
//                 WHERE PostId = " + postToEditDto.PostId.ToString() +
//                 "AND UserId = " + this.User.FindFirst("userId")?.Value;  

//             if(_dapper.ExecuteSql(sql))
//             {
//                 return Ok();
//             }
//             throw new Exception ("Failed to edit post");
//         }

//         [HttpDelete("Post/{postId}")]
//         public IActionResult DeletePost(int postId)
//         {
//             string sql = @"DELETE FROM TutorialAppSchema.Posts 
//             WHERE PostId = " + postId.ToString() +
//             "AND UserId = " + this.User.FindFirst("userId")?.Value;  

//             if(_dapper.ExecuteSql(sql))
//             {
//                 return Ok();
//             }
//             throw new Exception ("Failed to delete post");
//         }

//         [HttpGet("PostBySearch/{searchParam}")]
//         public IEnumerable<Post> SearchByPost(string searchParam)
//         {
//             string getPostsSql = @"SELECT [PostId],
//                     [UserId],
//                     [PostTitle],
//                     [PostContent],
//                     [PostCreated],
//                     [PostUpdated] FROM TutorialAppSchema.Posts
//                     WHERE PostTitle LIKE '%" + searchParam + "%'" +
//                     " OR PostContent LIKE '%" + searchParam + "%'";

//             return _dapper.LoadData<Post>(getPostsSql);
//         }
//     }
// }