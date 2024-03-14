using DotnetAPI.Models;

namespace DotnetAPI.Data
{
    public class UserRepository : IUserRepository
    {
        DataContextEF _ef; 

        public UserRepository(IConfiguration config)
        {
            _ef = new DataContextEF(config);

        }

        public bool SaveChanges()
        {
            return _ef.SaveChanges() > 0;
        }

        public void AddEntity<T>(T entityToAdd)
        {
            if (entityToAdd != null)
            {
                 _ef.Add(entityToAdd);
            }
        }

        public void RemoveEntity<T>(T entityToRemove)
        {
            if (entityToRemove != null)
            {
                _ef.Remove(entityToRemove); 
            }
        }

        public IEnumerable<User> GetUsers()
        {
        
            IEnumerable<User> users = _ef.Users.ToList<User>();
            return users;
        }
        public User GetSingleUser(int userId)
        {
            
        User? user = _= _ef.Users.Where(u => u.UserId == userId).FirstOrDefault<User>();
            if(user != null)
            {
                return user;
            }

            throw new Exception("Failed to get user");
        }
    }
}