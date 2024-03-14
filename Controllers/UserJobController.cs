
using DotnetAPI.Data;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserJobController: ControllerBase
    {
        DataContextDapper _dapper; 

        public UserJobController(IConfiguration config)
        {
                _dapper = new DataContextDapper(config);
        }

        [HttpGet("GetUsersJobInfo")]
        public IEnumerable<UserJobInfo> GetUsersJobInfo()
        {
            string sql = "SELECT * FROM TutorialAppSchema.UserJobInfo";

            return _dapper.LoadData<UserJobInfo>(sql);
        }

        [HttpGet("GetSingleUserJobInfo/{userId}")]
        public UserJobInfo GetSingleUserJobInfo(int userId)
        {
            UserJobInfo userInfo = _dapper.LoadDataSingle<UserJobInfo>(@"SELECT * FROM TutorialAppSchema.UserJobInfo WHERE UserId = " + userId.ToString());

            return userInfo;
        }

        [HttpPost("CreatUserJobInfo")]
        public IActionResult CreateUserJob(UserJobToAdd userJobInfo)
        {
            string sql = @"INSERT TutorialAppSchema.UserJobInfo (
                            [JobTitle],
                            [Department])
                            VALUES ('" + userJobInfo.JobTitle 
                            + "', '" + userJobInfo.Department 
                            + "')";

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }
            else
            {
                throw new Exception("Cannot create user job info");
            }    
        }

        [HttpPut("UpdateUserJobInfo")]
        public IActionResult UpdateUserJobInfo(UserJobInfo userJobInfo)
        {
            string sql = @"UPDATE TutorialAppSchema.UserJobInfo
            SET Jobtitle = '"+ userJobInfo.JobTitle + 
            "', Department = '" + userJobInfo.Department + 
            "' WHERE UserId = " + userJobInfo.UserId;
            
            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }
            else
            {
                throw new Exception("User job info could not be updated");
            }
        }

        [HttpDelete("DeleteUserJobInfo")]
        public IActionResult DeleteUserJobInfo(int userId)
        {
            string sql = @"DELETE FROM TutorialAppSchema.UserJobInfo 
            WHERE UserId = " + userId.ToString();

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }
            else
            {
                throw new Exception("User could not be deleted");
            }
        }
    }
}