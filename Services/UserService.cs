using System;
using System.Linq;
using System.Collections.Generic;
using JogoApi.Model;
using JogoApi.Models;

namespace JogoApi.Services
{

    public class UserService
    {

        private readonly JogoContext _context;

        public UserService(JogoContext context)
        {
            _context = context;
        }


        public User Create(User user, string password)
        {

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new Exception("Password is required");
            }

            if (_context.Users.Any(x => x.Username == user.Username))
            {
                throw new Exception("Username \"" + user.Username + "\"is already taken");
            }

            byte[] pwdHash, pwdSalt;
            CreatePasswordHash(password, out pwdHash, out pwdSalt);
            user.PasswordHash = pwdHash;
            user.PasswordSalt = pwdSalt;
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }


        public User GetById(int id)
        {
            return _context.Users.Find(id);
        }

        public List<User> GetAll()
        {
            return _context.Users.ToList();
        }

        public void Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }

        public void Update(User userIn, string password = null)
        {
            var user = _context.Users.Find(userIn.Id);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            if (!string.IsNullOrWhiteSpace(userIn.Username))
            {
                user.Username = userIn.Username;
            }

            if (!string.IsNullOrWhiteSpace(userIn.Email))
            {
                user.Email = userIn.Email;
            }


            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passHash, passSalt;
                CreatePasswordHash(password, out passHash, out passSalt);
                user.PasswordHash = passHash;
                user.PasswordSalt = passSalt;
            }

            _context.Users.Update(user);
            _context.SaveChanges();

        }


        public User Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return null;
            }

            var user = _context.Users.SingleOrDefault(x => x.Username == username);

            if (user == null)
            {
                return null;
            }

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }
            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {

            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Cannot be empty or whitespace", "password");
            }
            if (passwordHash.Length != 64)
            {
                throw new ArgumentNullException("Invalid length of pass hash (64 bytes expected)", "passworHash");
            }

            if (passwordSalt.Length != 128)
            {
                throw new ArgumentNullException("Invalid length of pass salt (128 bytes expected)", "passworSalt");
            }
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {

                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void CreatePasswordHash(string password, out byte[] pwdHash, out byte[] pwdSalt)
        {
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Cannot be empty or whitespace", "password");
            }

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                pwdSalt = hmac.Key;
                pwdHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }




    }


}