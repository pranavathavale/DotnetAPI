using DotnetAPI.Data;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserSalaryController : ControllerBase
    {
        DataContextDapper _dapper;

        public UserSalaryController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("GetAllSalaries")]
        public IEnumerable<UserSalary> GetAllSalaries()
        {
            return _dapper.LoadData<UserSalary>(@"SELECT * FROM TutorialAppSchema.UserSalary");
        }

        [HttpGet("GetUserSalary/{userId}")]
        public UserSalary GetUserSalary(int userId)
        {
            string sql = @"SELECT * FROM TutorialAppSchema.UserSalary 
            WHERE UserId = " + userId.ToString();

            return _dapper.LoadDataSingle<UserSalary>(sql);
        }

        [HttpPost("CreateUserSalary")]
        public IActionResult CreateUserSalary(UserSalary userSalary)
        {
            string sql = @"INSERT TutorialAppSchema.UserSalary(
            [UserId],
            [Salary])
            VALUES(
                '" + userSalary.UserId + 
                "', '" + userSalary.Salary + 
                "')";    
            
            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }
            else 
            {
                throw new Exception("User Salary could not be created");
            }
        }

        [HttpPut("UpdateUserSalary")]
        public IActionResult UpdateUserSalary(UserSalary userSalary)
        {
            string sql = @"UPDATE TutorialAppSchema.UserSalary 
            SET [Salary] = '" + userSalary.Salary + 
            "' WHERE UserId = " + userSalary.UserId;

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }
            else
            {
                throw new Exception("User salary could not be updated");
            }
        }

        [HttpDelete("DeleteUserSalary")]
        public IActionResult DeleteUserSalary(int userId)
        {
            string sql = @"DELETE FROM TutorialAppSchema.UserSalary 
            WHERE UserId = " + userId.ToString();

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }
            else
            {
                throw new Exception("User salary could not be deleted");
            }
        }
    }
}