
using AutoMapper;
using DotnetAPI.Data;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Server;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserEFController : ControllerBase
{
    //DataContextEF _ef;

    IUserRepository _userRepository; 
    IMapper _mapper;
    public UserEFController(IConfiguration config, IUserRepository userRepository)
    {
        // Console.WriteLine(config.GetConnectionString("DefaultConnection"));
       // _ef = new DataContextEF(config);
        _userRepository = userRepository;
        _mapper = new Mapper(new MapperConfiguration(cfg => {
            cfg.CreateMap<UserToAddDto, User>();
        }));
    }

    [HttpGet("GetUsers")]
    public IEnumerable<User> GetUsers()
    {
 
        IEnumerable<User> users = _userRepository.GetUsers();
        return users;
    }

    [HttpGet("GetSingleUser/{userId}")]
    public User GetSingleUser(int userId)
    {
        return _userRepository.GetSingleUser(userId);
    }

    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        User? userDb = _= _userRepository.GetSingleUser(user.UserId);
        if(userDb != null)
        {
            userDb.Active       = user.Active;
            userDb.FirstName    = user.FirstName;
            userDb.LastName     = user.LastName;
            userDb.Gender       = user.Gender;
            userDb.Email        = user.Email;
            if(_userRepository.SaveChanges())
            {
                return Ok();
            }
        }

        throw new Exception("Failed to update user");
    }

    [HttpPost("AddUser")]
    public IActionResult AddUser(UserToAddDto user)
    {
        User userDb = _mapper.Map<User>(user);
            // userDb.Active       = user.Active;
            // userDb.FirstName    = user.FirstName;
            // userDb.LastName     = user.LastName;
            // userDb.Gender       = user.Gender;
            // userDb.Email        = user.Email;
        _userRepository.AddEntity<User>(userDb);
        if (_userRepository.SaveChanges())
        {
            return Ok();
        }

        throw new Exception("Failed to add user");
    }

    [HttpDelete("DeleteUser")]
    public IActionResult DeleteUser(int userId)
    {
        User? userDb = _= _userRepository.GetSingleUser(userId);
        if(userDb != null)
        {
            _userRepository.RemoveEntity<User>(userDb);
            if(_userRepository.SaveChanges())
            {
                return Ok();
            }
        }

        throw new Exception("User cannot be deleted");      
    }

}