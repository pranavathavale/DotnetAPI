
using DotnetAPI.Data;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Server;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    DataContextDapper _dapper; 
    public UserController(IConfiguration config)
    {
        // Console.WriteLine(config.GetConnectionString("DefaultConnection"));
        _dapper = new DataContextDapper(config);
    }

    [HttpGet("GetUsers")]
    public IEnumerable<User> GetUsers()
    {
 
        IEnumerable<User> users = _dapper.LoadData<User>(@"SELECT * FROM TutorialAppSchema.Users");

        return users;
    }

    [HttpGet("GetSingleUser/{userId}")]
    public User GetSingleUser(int userId)
    {
        
        User user = _dapper.LoadDataSingle<User>(@"SELECT * FROM TutorialAppSchema.Users WHERE UserId = " + userId.ToString());

        return user;
    }

    [HttpGet("TestConnection")]
    public DateTime TestConnection()
    {
       return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
    }

    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        string sql = @"
        UPDATE TutorialAppSchema.Users
        SET [FirstName] = '" + user.FirstName + 
        "', [LastName] = '" + user.LastName + 
        "', [Email] = '" + user.Email + 
        "', [Gender]= '" + user.Gender + 
        "', [Active]= '" + user.Active + 
        "' WHERE UserId = '" + user.UserId + "'"; 
        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        throw new Exception("Failed to update user");
    }

    [HttpPost("AddUser")]
    public IActionResult AddUser(UserToAddDto user)
    {
        string sql = @"INSERT
        INTO TutorialAppSchema.Users(
            [FirstName],
            [LastName],
            [Email],
            [Gender],
            [Active])
        VALUES (" + 
        "'" + user.FirstName +
        "',  '" + user.LastName + 
        "',  '" + user.Email + 
        "',  '" + user.Gender + 
        "',  '" + user.Active +
        "')";
        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to add user");
    }

    [HttpDelete("DeleteUser")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = @"
            DELETE FROM TutorialAppSchema.Users
            WHERE UserId = " + userId.ToString();
        if(_dapper.ExecuteSql(sql))
        {
            return Ok();
        }   

        throw new Exception("User cannot be deleted");      
    }


}