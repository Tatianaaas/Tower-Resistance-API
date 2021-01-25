using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using JogoApi.DataAccess;
using JogoApi.Interfaces;
using JogoApi.Model;
using JogoApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JogoApi.Controllers
{
    [Route("api/TowerResistance")]
    [ApiController]
    [Authorize(Roles = "user,admin")]
    public class FriendsController : ControllerBase
    {
        private readonly JogoContext _context;

        private IConnection _connection;

        public FriendsController(JogoContext context)
        {
            _connection = new Connection();
            _connection.Fetch();
            _context = context;
        }

        ///<summary>
        ///List of all friends of current user
        ///</summary>
        [HttpGet("Friends")]
        public IActionResult GetAll()
        {
             var sessionUsername = HttpContext.User.FindFirstValue(ClaimTypes.Name);
            Collection<Friend> friends = new Collection<Friend>();
            var userInv =
                _context.Users.SingleOrDefault(e => e.Username == sessionUsername);

            IDAO<Friend> friendDAO = new FriendDAO(_connection);
            friends = friendDAO.GetAll();
            List<string> names = new List<string>();
            var name = userInv;
            var listFriends =
                friends
                    .Where(e =>
                        e.UserId == userInv.Id || e.FriendUserId == userInv.Id)
                    .ToList();
            foreach (var item in listFriends)
            {
                if (item.UserId == userInv.Id)
                {
                    name =
                        _context
                            .Users
                            .SingleOrDefault(e => e.Id == item.FriendUserId);
                }
                else if (item.FriendUserId == userInv.Id)
                {
                    name =
                        _context
                            .Users
                            .SingleOrDefault(e => e.Id == item.UserId);
                }

                if (!names.Contains(name.Username))
                {
                    names.Add(name.Username);
                }
            }

            return Ok(new { Friends = names });
        }

        ///<summary>
        ///Only admin can see the list of some user friends
        ///</summary>
        ///<param name="username"></param>
        [Authorize(Roles = "admin")]
        [HttpGet("Friends/{username}", Name = "Get Friend")]
        public IActionResult GetUserFriends(string username)
        {

            Collection<Friend> friends = new Collection<Friend>();
            var userInv =
                _context.Users.SingleOrDefault(e => e.Username == username);

            IDAO<Friend> friendDAO = new FriendDAO(_connection);
            friends = friendDAO.GetAll();
            List<string> names = new List<string>();
            var name = userInv;
            var listFriends =
                friends
                    .Where(e =>
                        e.UserId == userInv.Id || e.FriendUserId == userInv.Id)
                    .ToList();
            foreach (var item in listFriends)
            {
                if (item.UserId == userInv.Id)
                {
                    name =
                        _context
                            .Users
                            .SingleOrDefault(e => e.Id == item.FriendUserId);

                    //names.Add(name.Username);
                }
                else if (item.FriendUserId == userInv.Id)
                {
                    name =
                        _context
                            .Users
                            .SingleOrDefault(e => e.Id == item.UserId);
                }

                if (!names.Contains(name.Username))
                {
                    names.Add(name.Username);
                }
            }

            return Ok(new { Friends = names });
        }

        ///<summary>
        /// Send friendship invite to another player
        /// </summary>
        [HttpPost("Friends/Send")]
        public IActionResult CreateInvite(SetFriendship friendship)
        {

            // ClaimsPrincipal currentUser = this.User;
            var sessionUsername = HttpContext.User.FindFirstValue(ClaimTypes.Name);
            var userInv =_context.Users.SingleOrDefault(e => e.Username == sessionUsername);
            var friendInv =_context.Users.SingleOrDefault(e =>e.Username == friendship.FriendUsername);
               IDAO<Friend> friendDAO = new FriendDAO(_connection);
            IDAO<Invite> inviteDAO = new InviteDAO(_connection);
            if (userInv.Id != friendInv.Id)
            {
                var found1= friendDAO.GetAllData().Where(e => e.UserId == userInv.Id && e.FriendUserId == friendInv.Id).ToList();
            var found2= friendDAO.GetAllData().Where(e => e.UserId == friendInv.Id && e.FriendUserId == userInv.Id).ToList();
            Friend friend = new Friend();
            if (found1.Count()== 0 && found2.Count()== 0)
            {
                friend.UserId = userInv.Id;
                friend.FriendUserId = friendInv.Id;
                var invite = new Invite { Date = DateTime.Now, Status = false };

                //_context.Invite.Add(invite);
                // _context.Invite.Add(invite);
                // _context.SaveChanges();
                // _context.Entry<Invite>(invite).State =EntityState.Detached;
                var inv = inviteDAO.Create(invite);
                friend.IdInvite = inv.Id;
                friendDAO.Create(friend);
                return Ok(new { Friendship = friend, Invite = invite });
            }
            else
            {
                return BadRequest();
                }
            
            
            } else
            {
                return BadRequest();
                }
         
            
        }



        ///<summary>
        ///Delete friendship
        ///</summary>
        ///<param name="username"></param>
        // DELETE: api/Leagues/5
        [HttpPut("Friends/Delete/{username}")]
        public  IActionResult DeleteFriends(string username)
        {
            var sessionUsername = HttpContext.User.FindFirstValue(ClaimTypes.Name);
            IDAO<Friend> friendDAO = new FriendDAO(_connection);
            IDAO<Invite> inviteDAO = new InviteDAO(_connection);
            var user = _context.Users.Single(u => u.Username == sessionUsername);
            var friend =_context.Users.Single(u => u.Username == username);
            var friendship1 = friendDAO.GetAllData().Where(f => f.UserId == friend.Id && f.FriendUserId == user.Id).SingleOrDefault();
           // var friendship2 = _context.Friend.Where(f => f.UserId == user.Id && f.FriendUserId == friend.Id).SingleOrDefault();

            if (friendship1 == null)
            {
                friendship1 = friendDAO.GetAllData().Where(f => f.UserId == user.Id && f.FriendUserId == friend.Id).SingleOrDefault();
               
                if(friendship1==null)
                {
                    return NotFound();
                }

            }

            var id = inviteDAO.GetAll().Where(i => i.Id == friendship1.IdInvite).SingleOrDefault().Id;
            var invite = inviteDAO.FindByID(id);
            invite.Status = false;
            inviteDAO.Update(invite);
           // _context.Friend.Remove(friendship);
             _context.SaveChangesAsync();

            return Ok(new
            {
                Status = "Friendship removed"
            });
        }
        private bool FriendExists(long id)
        {
            return _context.Friend.Any(e => e.Id == id);
        }
    }
}
