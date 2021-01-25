using JogoApi.Models;
using Microsoft.EntityFrameworkCore;
using JogoApi.Model.Game;
namespace JogoApi.Model
{
    public class JogoContext : DbContext
    {
        public JogoContext(DbContextOptions<JogoContext> options)
            : base(options)
        {
            // this.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Friend> Friend { get; set; }

        public DbSet<Invite> Invite { get; set; }

        public DbSet<JogoApi.Model.Game.MatchHistory> MatchHistory { get; set; }

        public DbSet<JogoApi.Model.Game.Leagues> Leagues { get; set; }

    
    }
}