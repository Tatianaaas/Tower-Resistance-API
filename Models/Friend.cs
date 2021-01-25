using System;

namespace JogoApi.Models
{
    public class Friend
    {
        public int Id { get; set; }

        public int IdInvite { get; set; }
        public int UserId { get; set; }
        public int FriendUserId { get; set; }

    }
}


namespace JogoApi.Models
{
    public class FriendsList
    {
        public string Username { get; set; }
        public string FriendUsername { get; set; }
        public DateTime Date { get; set; }

    }
}