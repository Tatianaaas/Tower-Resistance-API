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
    public class InvitesController : ControllerBase
    {
        private readonly JogoContext _context;

        private IConnection _connection;

        public InvitesController(JogoContext context)
        {
            _connection = new Connection();
            _connection.Fetch();
            _context = context;
        }

        ///<summary>
        /// List of all users invites
        ///</summary>
        [Authorize(Roles = "admin")]
        [HttpGet("Invites")]
        public ActionResult<List<Invite>> GetAll()
        {
            IDAO<Invite> inviteDAO = new InviteDAO(_connection);
            return inviteDAO.GetAll().ToList();
        }

        ///<summary>
        ///List of invites sent to user
        ///</summary>
        ///<param name="username"></param>
        [Authorize(Roles = "user,admin")]
        [HttpGet("Invites/To/{username}")]
        public IActionResult GetInviteTo(string username)
        {

            Collection<Invite> invites = new Collection<Invite>();
            IDAO<Invite> inviteDAO = new InviteDAO(_connection);
            invites = inviteDAO.GetAll();

            var userInv =
                _context.Users.SingleOrDefault(e => e.Username == username);

            IDAO<Friend> friendDAO = new FriendDAO(_connection);
            var friends = friendDAO.GetAllData().Where(e => e.FriendUserId == userInv.Id);
            var QSOuterJoin =
                from friendship in friends
                join inv
                in invites
                on friendship.IdInvite
                equals inv.Id
                into InvitesList
                from invite in InvitesList.DefaultIfEmpty()
                select new { friendship, invite };

            var queryList = QSOuterJoin.ToList();

            List<FriendsList> result = new List<FriendsList>();
            foreach (var item in queryList)
            {
                if (item.invite.Status == false)
                {
                    var user =
                    _context
                        .Users
                        .SingleOrDefault(e => e.Id == item.friendship.UserId);
                var friend =
                    _context
                        .Users
                        .SingleOrDefault(e =>
                            e.Id == item.friendship.FriendUserId);

                var values =
                    new FriendsList {
                        Username = user.Username,
                        FriendUsername = friend.Username,
                        Date = item.invite.Date
                    };

                result.Add (values);
                }
                
            }
            return Ok(new { List = result });
        }


        
        ///<summary>
        ///List of invites sent from user
        ///</summary>
        [Authorize(Roles = "user,admin")]
        [HttpGet("Invites/From")]
        public IActionResult GetInviteFrom()
        {
            var sessionUsername = HttpContext.User.FindFirstValue(ClaimTypes.Name);
            Collection<Invite> invites = new Collection<Invite>();
            IDAO<Invite> inviteDAO = new InviteDAO(_connection);
            invites = inviteDAO.GetAll();

            var userInv =
                _context.Users.SingleOrDefault(e => e.Username == sessionUsername);

            IDAO<Friend> friendDAO = new FriendDAO(_connection);
            var friends = friendDAO.GetAllData().Where(e => e.UserId == userInv.Id);
            var QSOuterJoin =
                from friendship in friends
                join inv
                in invites
                on friendship.IdInvite
                equals inv.Id
                into InvitesList
                from invite in InvitesList.DefaultIfEmpty()
                select new { friendship, invite };

            var queryList = QSOuterJoin.ToList();

            List<FriendsList> result = new List<FriendsList>();
            foreach (var item in queryList)
            {
                if (item.invite.Status == false)
                {
                    var user =
                    _context
                        .Users
                        .SingleOrDefault(e => e.Id == item.friendship.UserId);
                var friend =
                    _context
                        .Users
                        .SingleOrDefault(e =>
                            e.Id == item.friendship.FriendUserId);

                var values =
                    new FriendsList {
                        Username = user.Username,
                        FriendUsername = friend.Username,
                        Date = item.invite.Date
                    };

                result.Add (values);
                }
                
            }
            return Ok(new { List = result });
        }

        ///<summary>
        /// Accept invite
        ///</summary>
        ///<param name="setFriendship"></param>
        [Authorize(Roles = "user,admin")]
        [HttpPut("Invites/Confirm")]
        public IActionResult Update(SetFriendship setFriendship)
        {
            //ClaimsPrincipal currentUser = this.User;
            var sessionUsername = HttpContext.User.FindFirstValue(ClaimTypes.Name);
            var user = _context.Users.Single(u => u.Username == sessionUsername);
            if (sessionUsername != setFriendship.FriendUsername)
            {
                var friend =_context.Users.Single(u => u.Username == setFriendship.FriendUsername);
            IDAO<Invite> inviteDAO = new InviteDAO(_connection);
            IDAO<Friend> friendDAO = new FriendDAO(_connection);

            var invite =friendDAO.GetAllData().SingleOrDefault(f =>f.UserId == friend.Id && f.FriendUserId == user.Id);

                if (invite != null)
                {
                var oldInvite = inviteDAO.FindByID(invite.IdInvite);

            if (oldInvite == null)
            {
                return NotFound();
            }
            if (oldInvite.Status == false)
            {
                oldInvite.Status = true;
            }

            inviteDAO.Update (oldInvite);
                return Ok();
            }
            else
            {
                return BadRequest();
            }
            }
            else
            {
                return BadRequest();
            }
            
            
        }

        // ///<summary>
        // /// Invite status
        // /// </summary>
        // [Authorize(Roles = "user,admin")]
        // [HttpPost("Invites/Send")]
        // public IActionResult CreateInvite(Invite invite)
        // {
        //     DateTime localDate = DateTime.Now;
        //     invite.Date = localDate;
        //     invite.Status = false;
        //     IDAO<Invite> inviteDAO = new InviteDAO(_connection);
        //     inviteDAO.Create (invite);
        //     return CreatedAtRoute("Get Invite", new { id = invite.Id }, invite);
        // }

        private bool InviteExists(long id)
        {
            return _context.Invite.Any(e => e.Id == id);
        }
    }
}
