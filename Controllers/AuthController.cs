using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerUI;


namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthController: ControllerBase
    {
        private readonly DataContextDapper _dapper;
        // private readonly IConfiguration _config;
        private readonly AuthHelper _authHelper;
        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            //_config = config;
            _authHelper = new AuthHelper(config);
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register(UserForRegistrationDto userForRegistration)
        {
            if(userForRegistration.Password == userForRegistration.PasswordConfrim)
            {
                string checkUserExists = @"SELECT Email FROM TutorialAppSchema.Auth 
                WHERE Email = '" + userForRegistration.Email + "'"; 
                IEnumerable<string> existingUsers =  _dapper.LoadData<string>(checkUserExists);
                if(existingUsers.Count() == 0)
                {

                    UserForLoginDto userForSetPassword = new UserForLoginDto()
                    {
                        Email = userForRegistration.Email,
                        Password = userForRegistration.Password
                    };
                    if(_authHelper.SetPassword(userForSetPassword))
                    {
                        string sqlAddUser = @"EXEC TutorialAppSchema.spUser_Upsert
                            @FirstName = '" + userForRegistration.FirstName + 
                            "', @LastName = '" + userForRegistration.LastName + 
                            "', @Email = '" + userForRegistration.Email + 
                            "', @Gender = '" + userForRegistration.Gender + 
                            "', @JobTitle = '" + userForRegistration.JobTitle + 
                            "', @Department = '" + userForRegistration.Department + 
                            "', @Salary = '" + userForRegistration.Salary + 
                            "', @Active =  1";

                            if(_dapper.ExecuteSql(sqlAddUser))
                            {
                                return Ok();
                            }
                            throw new Exception("New user cannot be created");
                    }
                    throw new Exception("Failed to register User");
                }
                throw new Exception("User with this email already exists");
               
            }
            throw new Exception("Passwords do not match");
        }

        [HttpPut("ResetPassword")]
        public IActionResult ResetPassword(UserForLoginDto userForSetPassword)
        {
              if(_authHelper.SetPassword(userForSetPassword))
              {
                    return Ok();
              }
              throw new Exception("Failed to reset password!");
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDto userForLogin)
        {
            string sqlForHashAndSalt = @"EXEC TutorialAppSchema.spLoginConfirmation_Get
                @Email = @EmailParam";

            DynamicParameters sqlParamters = new DynamicParameters();
            sqlParamters.Add("@EmailParam", userForLogin.Email, DbType.String);

            UserForLoginConfirmationDto userForConfirmation = _dapper.LoadDataSingleWithParameters<UserForLoginConfirmationDto>(sqlForHashAndSalt, sqlParamters);

            byte[] passwordHash = _authHelper.GetPasswordHash(userForLogin.Password, userForConfirmation.PasswordSalt);

            for (int i = 0; i < passwordHash.Length; i++)
            {
                if (passwordHash[i] != userForConfirmation.PasswordHash[i])
                {
                    return StatusCode(401, "Incorrect password");
                }
            }
            string userIdSql = @"SELECT UserId FROM TutorialAppSchema.Users 
            WHERE Email = '" + userForLogin.Email + "'";
            int userId = _dapper.LoadDataSingle<int>(userIdSql);
            return Ok(new Dictionary<string, string>{
                    {"token", _authHelper.CreateToken(userId)}
            });
        }


        [HttpGet("RefreshToken")]
        public string RefreshToken()
        {
            string userIdSql = @"SELECT UserId FROM TutorialAppSchema.Users 
            WHERE UserId = '" + User.FindFirst("userId")?.Value + "'";

            int userId = _dapper.LoadDataSingle<int>(userIdSql);

            return _authHelper.CreateToken(userId);
        }

      
    }
}