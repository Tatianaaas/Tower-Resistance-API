using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using JogoApi.DataAccess;
using JogoApi.Interfaces;
using JogoApi.Model;
using JogoApi.Model.Game;
using JogoApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace JogoApi.Controllers
{
    [Route("api/TowerResistance")]
    [ApiController]
    [Authorize(Roles = "user,admin")]
    public class MatchController : ControllerBase
    {
        private readonly JogoContext _context;

        private IConnection _connection;

        public MatchController(JogoContext context)
        {
            _connection = new Connection();
            _connection.Fetch();
            _context = context;
        }

        ///<summary>
        ///Get all data
        ///</summary>
        // GET: api/Match
        [HttpGet("Match")]
        public ActionResult<List<MatchHistory>> GetMatchHistory()
        {
            IDAO<MatchHistory> matchDAO = new MatchDAO(_connection);
            return matchDAO.GetAll().ToList();
        }

        ///<summary>
        ///Get user matches data
        ///</summary>
        ///<param name="username"></param>
        // GET: api/Match/5
        [HttpGet("Match/{username}")]
        public ActionResult<List<MatchHistory>> GetMatchHistory(string username)
        {
            var user =
                _context.Users.SingleOrDefault(e => e.Username == username);
            Collection<MatchHistory> matches = new Collection<MatchHistory>();

            IDAO<MatchHistory> matchDAO = new MatchDAO(_connection);
            matches = matchDAO.GetAll();
            return matches.Where(e => e.Id == user.Id).ToList();
        }

        ///<summary>
        /// Insert match results
        /// </summary>
        // POST: api/Match
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("Match/Insert")]
        public IActionResult PostMatchHistory(Results results)
        {
            IDAO<MatchHistory> matchDAO = new MatchDAO(_connection);
            IDAO<Leagues> leaguesDAO = new LeaguesDAO(_connection);
            IDAO<User> usersDAO = new UserDAO(_connection);
            List<Leagues> leagues1 = new List<Leagues>();
            List<MatchHistory> matchHistories = new List<MatchHistory>();
            var matches = matchDAO.GetAll().ToList();
            var leaguesList = leaguesDAO.GetAll().ToList();
            var matchId = 1;

            string[] leagueName =
            {
                "Bronze",
                "Silver",
                "Gold",
                "Platinum",
                "Diamond",
                "Master",
                "Challenger"
            };
            if (matches.Count() > 0)
            {
                matchId = matches.Last().Id + 1;
            }

            foreach (var player1 in results.Team1)
            {
                var user =
                    usersDAO
                        .GetAll()
                        .Single(u => u.Username == player1.Username);
                var win = false;
                if (results.Winner == "team1" || results.Winner == "Team1")
                {
                    win = true;
                }
                else
                {
                    win = false;
                }

                var player =
                    new MatchHistory {
                        Id = matchId,
                        IdUser = user.Id,
                        Date = results.Date,
                        Kills = player1.Kills,
                        Deaths = player1.Deaths,
                        TowersDestroyed = player1.TowersDestroyed,
                        GameTime = TimeSpan.Parse(results.GameTime),
                        Result = win
                    };

                var league =
                    leaguesDAO
                        .GetAll()
                        .Where(u => u.IdUser == player.IdUser)
                        .SingleOrDefault();

                //var p = 0;
                if (league == null)
                {
                    var leagues =
                        new Leagues {
                            Id = 1,
                            IdUser = player.IdUser,
                            Description = leagueName[0] + " 1"
                        };

                    if (player.Result == true)
                    {
                        leagues.Points += 3;
                    }
                    else
                    {
                        leagues.Points -= 2;
                    }
                    _context.Leagues.Add (leagues);
                    _context.SaveChanges();
                    _context.Entry<Leagues>(leagues).State =
                        EntityState.Detached;
                }
                else
                {
                    var oldLeague =
                        leaguesDAO.FindByID(league.Id, league.IdUser);

                    if (player.Result == true)
                    {
                        oldLeague.Points += 3;
                    }
                    else
                    {
                        oldLeague.Points -= 2;
                    }
                    if (oldLeague.Points >= 30 && oldLeague.Points % 3 == 0)
                    {
                        var pos = oldLeague.Points / 30;
                        oldLeague.Id += 1;
                        oldLeague.Description = leagueName[pos];

                        oldLeague.Description = leagueName[2];
                    }

                    if (oldLeague.Points % 10 == 0)
                    {
                        oldLeague.Description += (oldLeague.Points / 10);
                    }
                    leaguesDAO.Update (oldLeague);
                }
                _context.MatchHistory.Add (player);
                _context.SaveChanges();
                _context.Entry<MatchHistory>(player).State =
                    EntityState.Detached;
                //matchDAO.Create(player);
            }

            foreach (var player2 in results.Team2)
            {
                var user =
                    usersDAO
                        .GetAll()
                        .Single(u => u.Username == player2.Username);
                var win = false;
                if (results.Winner == "team2" || results.Winner == "Team2")
                {
                    win = true;
                }
                else
                {
                    win = false;
                }

                var player =
                    new MatchHistory {
                        Id = matchId,
                        IdUser = user.Id,
                        Date = results.Date,
                        Kills = player2.Kills,
                        Deaths = player2.Deaths,
                        TowersDestroyed = player2.TowersDestroyed,
                        GameTime = TimeSpan.Parse(results.GameTime),
                        Result = win
                    };
                var league =
                    leaguesDAO
                        .GetAll()
                        .Where(u => u.IdUser == player.IdUser)
                        .SingleOrDefault();

                if (league == null)
                {
                    Leagues leagues = new Leagues();
                    leagues.Id = 1;
                    leagues.IdUser = player.IdUser;
                    leagues.Description = "Bronze 1";

                    if (player.Result == true)
                    {
                        leagues.Points += 3;
                    }
                    else
                    {
                        if (leagues.Points != 0)
                        {
                            
                        }
                    }

                    _context.Leagues.Add (leagues);
                    _context.SaveChanges();
                    _context.Entry<Leagues>(leagues).State =
                        EntityState.Detached;
                }
                else
                {
                    var oldLeague =
                        leaguesDAO.FindByID(league.Id, league.IdUser);

                    if (player.Result == true)
                    {
                        oldLeague.Points += 3;
                    }
                    else
                    {
                        oldLeague.Points -= 2;
                    }
                    if (oldLeague.Points >= 30 && oldLeague.Points % 3 == 0)
                    {
                        var pos = oldLeague.Points / 30;
                        oldLeague.Id += 1;
                        oldLeague.Description = leagueName[pos];

                        oldLeague.Description = leagueName[2];
                    }

                    if (oldLeague.Points % 10 == 0)
                    {
                        oldLeague.Description += (oldLeague.Points / 10);
                    }
                    leaguesDAO.Update (oldLeague);
                }

                _context.MatchHistory.Add (player);
                _context.SaveChanges();
                _context.Entry<MatchHistory>(player).State =
                    EntityState.Detached;
            }

            var resultLeagues = leaguesDAO.GetAll().ToList();
            List<LeaguesList> result = new List<LeaguesList>();
            foreach (var item in resultLeagues)
            {
                    var user = usersDAO.GetAll().SingleOrDefault(e => e.Id == item.IdUser);
                 var values =
                    new LeaguesList {
                        Username = user.Username,
                        Description = item.Description,
                        Points = item.Points
                    };

                result.Add (values);
                }


            //  _context.SaveChanges();
            return Ok(new { GameData =  result});
        }

        ///<summary>
        ///Delete match data
        ///</summary>
        ///<param name="id"></param>
        [HttpDelete("Match/{id}")]
        public IActionResult DeleteMatchHistory(int id)
        {
            var matchHistory =
                _context.MatchHistory.Where(m => m.Id == id).ToArray();
            if (matchHistory == null)
            {
                return NotFound();
            }

            _context.MatchHistory.RemoveRange (matchHistory);
            _context.SaveChangesAsync();

            return NoContent();
        }

        ///<summary>
        /// List of user matches statistics
        ///</summary>
        ///<param name="username"></param>
        // GET: api/Match/5
        [HttpGet("Match/Stats/{username}")]
        public IActionResult GetUserHistory(string username)
        {
            var user =
                _context.Users.SingleOrDefault(e => e.Username == username);
            Collection<MatchHistory> matches = new Collection<MatchHistory>();

            IDAO<MatchHistory> matchDAO = new MatchDAO(_connection);
            matches = matchDAO.GetAll();

            var results = matches.Where(e => e.IdUser == user.Id).ToList();
            var lines = results.Count();
            var kills = 0;
            var deaths = 0;
            var assists = 0;
            var time_spans = new List<TimeSpan>();
            var win = 0;
            var lost = 0;
            foreach (var match in results)
            {
                time_spans.Add(match.GameTime);
                kills += match.Kills;
                deaths += match.Deaths;
                assists += match.TowersDestroyed;
                if (match.Result == true)
                {
                    win += 1;
                }
                else
                {
                    lost += 1;
                }
            }

            var average =
                new TimeSpan(Convert.ToInt64(time_spans.Average(t => t.Ticks)));
            return Ok(new {
                Media_Time = average.ToString(@"hh\:mm\:ss"),
                Matches = lines,
                Average_Kills = kills / lines,
                Average_Deaths = deaths / lines,
                Average_Assists = assists / lines,
                Winrate = win / lines,
                Total_Kills = kills,
                Total_Deaths = deaths,
                Total_Assists = assists,
                Total_Victories = win,
                Total_Defeats = lost
            });
        }

        ///<summary>
        ///Get last 10 matches statistics of user
        ///</summary>
        ///<param name="username"></param>
        [HttpGet("Match/Stats/History/{username}")]
        public IActionResult GetUser10MatchesHistory(string username)
        {
            // var dateTime = DateTime.Now.AddDays(-10);
            var user =
                _context.Users.SingleOrDefault(e => e.Username == username);
            Collection<MatchHistory> matches = new Collection<MatchHistory>();

            IDAO<MatchHistory> matchDAO = new MatchDAO(_connection);
            matches = matchDAO.GetAll();

            var results =
                matches
                    .Where(e => e.IdUser == user.Id)
                    .OrderByDescending(e => e.Date)
                    .Take(10)
                    .ToList();
            var lines = results.Count();
            var kills = 0;
            var deaths = 0;
            var assists = 0;
            var time_spans = new List<TimeSpan>();
            var win = 0;
            var lost = 0;
            foreach (var match in results)
            {
                time_spans.Add(match.GameTime);
                kills += match.Kills;
                deaths += match.Deaths;
                assists += match.TowersDestroyed;
                if (match.Result == true)
                {
                    win += 1;
                }
                else
                {
                    lost += 1;
                }
            }
            var average =
                new TimeSpan(Convert.ToInt64(time_spans.Average(t => t.Ticks)));
            return Ok(new {
                Media_Time = average.ToString(@"hh\:mm\:ss"),
                Matches = lines,
                Average_Kills = kills / lines,
                Average_Deaths = deaths / lines,
                Average_Assists = assists / lines,
                Winrate = win / lines
            });
        }

        private bool MatchHistoryExists(int id)
        {
            return _context.MatchHistory.Any(e => e.Id == id);
        }
    }
}
