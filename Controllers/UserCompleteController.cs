
using DotnetAPI.Data;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Server;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserCompleteController : ControllerBase
{
    DataContextDapper _dapper; 
    public UserCompleteController(IConfiguration config)
    {
        // Console.WriteLine(config.GetConnectionString("DefaultConnection"));
        _dapper = new DataContextDapper(config);
    }

    [HttpGet("GetUsers/{userId}/{active}")]
    public IEnumerable<UserComplete> GetUsers(int userId, bool active)
    {
        string sql = @"TutorialAppSchema.spUsers_Get";
        string parameters = "";

        if (userId != 0)
        {
            parameters += ", @UserId = " + userId.ToString();
        }
        if (active)
        {
            parameters += ", @Active = " + active.ToString();
        }
        sql += parameters.Substring(1);

        IEnumerable<UserComplete> users = _dapper.LoadData<UserComplete>(sql); 

        return users;
    }

    [HttpPut("EditUser")]
    public IActionResult UpsertUser(UserComplete userComplete)
    {
        //You can add dynamic parameters here as well.
        string sql = @"EXEC TutorialAppSchema.spUser_Upsert
            @FirstName = '" + userComplete.FirstName + 
            "', @LastName = '" + userComplete.LastName + 
            "', @Email = '" + userComplete.Email + 
            "', @Gender = '" + userComplete.Gender + 
            "', @JobTitle = '" + userComplete.JobTitle + 
            "', @Department = '" + userComplete.Department + 
            "', @Salary = '" + userComplete.Salary + 
            "', @Active = '" + userComplete.Active + 
            "', @UserId = '" + userComplete.UserId + "'"; 
        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        throw new Exception("Failed to update user");
    }

    [HttpDelete("DeleteUser")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = @"TutorialAppSchema.spUser_Delete 
            @UserId = " + userId.ToString();
        if(_dapper.ExecuteSql(sql))
        {
            return Ok();
        }   

        throw new Exception("User cannot be deleted");      
    }
}