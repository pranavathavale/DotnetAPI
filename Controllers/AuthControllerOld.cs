// using System.Data;
// using System.IdentityModel.Tokens.Jwt;
// using System.Security.Claims;
// using System.Security.Cryptography;
// using System.Text;
// using DotnetAPI.Data;
// using DotnetAPI.Dtos;
// using DotnetAPI.Helpers;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Cryptography.KeyDerivation;
// using Microsoft.AspNetCore.Http.HttpResults;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
// using Microsoft.Data.SqlClient;
// using Microsoft.IdentityModel.Tokens;
// using Swashbuckle.AspNetCore.SwaggerUI;


// namespace DotnetAPI.Controllers
// {
//     [Authorize]
//     [ApiController]
//     [Route("[controller]")]
//     public class AuthController: ControllerBase
//     {
//         private readonly DataContextDapper _dapper;
//         // private readonly IConfiguration _config;
//         private readonly AuthHelper _authHelper;
//         public AuthController(IConfiguration config)
//         {
//             _dapper = new DataContextDapper(config);
//             //_config = config;
//             _authHelper = new AuthHelper(config);
//         }

//         [AllowAnonymous]
//         [HttpPost("Register")]
//         public IActionResult Register(UserForRegistrationDto userForRegistration)
//         {
//             if(userForRegistration.Password == userForRegistration.PasswordConfrim)
//             {
//                 string checkUserExists = @"SELECT Email FROM TutorialAppSchema.Auth 
//                 WHERE Email = '" + userForRegistration.Email + "'"; 
//                 IEnumerable<string> existingUsers =  _dapper.LoadData<string>(checkUserExists);
//                 if(existingUsers.Count() == 0)
//                 {
//                     byte[] passwordSalt = new byte[128 / 8];
//                     using(RandomNumberGenerator rng = RandomNumberGenerator.Create())
//                     {
//                         rng.GetNonZeroBytes(passwordSalt);
//                     }

//                     // string PasswordSaltPlusString = _config.GetSection("Appsettings:PasswordKey").Value + Convert.ToBase64String(passwordSalt);

//                     // byte[] passwordHash = KeyDerivation.Pbkdf2(
//                     //     password: userForRegistration.Password,
//                     //     salt: Encoding.ASCII.GetBytes(PasswordSaltPlusString),
//                     //     prf: KeyDerivationPrf.HMACSHA256,
//                     //     iterationCount: 100000,
//                     //     numBytesRequested: 256 / 8
//                     // );

//                     byte[] passwordHash = _authHelper.GetPasswordHash(userForRegistration.Password, passwordSalt);

//                     string sqlAddAuth = @"INSERT INTO TutorialAppSchema.Auth(
//                         [Email],
//                         [PasswordHash],
//                         [PasswordSalt]) 
//                         VALUES('" + userForRegistration.Email + 
//                         "', @PasswordHash, @PasswordSalt)";

//                     List<SqlParameter> sqlParamters = new List<SqlParameter>();
//                     SqlParameter passwordSaltParameter = new SqlParameter
//                     ("@PasswordSalt", SqlDbType.VarBinary);
//                     passwordSaltParameter.Value =passwordSalt;
//                     SqlParameter passwordHashParameter = new SqlParameter("@PasswordHash", SqlDbType.VarBinary);
//                     passwordHashParameter.Value = passwordHash;

//                     sqlParamters.Add(passwordSaltParameter);
//                     sqlParamters.Add(passwordHashParameter);

//                     if(_dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParamters))
//                     {
//                         string sqlAddUser = @"INSERT
//                             INTO TutorialAppSchema.Users(
//                                 [FirstName],
//                                 [LastName],
//                                 [Email],
//                                 [Gender],
//                                 [Active])
//                             VALUES (" + 
//                             "'" + userForRegistration.FirstName +
//                             "',  '" + userForRegistration.LastName + 
//                             "',  '" + userForRegistration.Email + 
//                             "',  '" + userForRegistration.Gender + 
//                             "', 1)";

//                             if(_dapper.ExecuteSql(sqlAddUser))
//                             {
//                                 return Ok();
//                             }
//                             throw new Exception("New user cannot be created");
//                     }
//                     throw new Exception("Failed to register User");
//                 }
//                 throw new Exception("User with this email already exists");
               
//             }
//             throw new Exception("Passwords do not match");
//         }

//         [AllowAnonymous]
//         [HttpPost("Login")]
//         public IActionResult Login(UserForLoginDto userForLogin)
//         {
//             string sqlForHashAndSalt = @"SELECT [PasswordHash],
//             [PasswordSalt] FROM TutorialAppSchema.Auth 
//             WHERE Email = '" + userForLogin.Email + "'";

//             UserForLoginConfirmationDto userForConfirmation = _dapper.LoadDataSingle<UserForLoginConfirmationDto>(sqlForHashAndSalt);

//             byte[] passwordHash = _authHelper.GetPasswordHash(userForLogin.Password, userForConfirmation.PasswordSalt);

//             for (int i = 0; i < passwordHash.Length; i++)
//             {
//                 if (passwordHash[i] != userForConfirmation.PasswordHash[i])
//                 {
//                     return StatusCode(401, "Incorrect password");
//                 }
//             }
//             string userIdSql = @"SELECT UserId FROM TutorialAppSchema.Users 
//             WHERE Email = '" + userForLogin.Email + "'";
//             int userId = _dapper.LoadDataSingle<int>(userIdSql);
//             return Ok(new Dictionary<string, string>{
//                     {"token", _authHelper.CreateToken(userId)}
//             });
//         }


//         [HttpGet("RefreshToken")]
//         public string RefreshToken()
//         {
//             string userIdSql = @"SELECT UserId FROM TutorialAppSchema.Users 
//             WHERE UserId = '" + User.FindFirst("userId")?.Value + "'";

//             int userId = _dapper.LoadDataSingle<int>(userIdSql);

//             return _authHelper.CreateToken(userId);
//         }

      
//     }
// }