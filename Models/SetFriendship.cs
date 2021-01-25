using System;
using System.ComponentModel.DataAnnotations;

namespace JogoApi.Models
{
    public class SetFriendship
    {
        //  [Required]
        //  public int IdInvite { get; set; }
        
        // [Required]
        // public string Username { get; set; }

        [Required]
        public string FriendUsername { get; set; }

    }
}