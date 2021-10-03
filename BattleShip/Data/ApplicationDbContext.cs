using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using BattleShip.Models;

namespace BattleShip.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<BattleShip.Models.Player> Player { get; set; }
        public DbSet<BattleShip.Models.Match> Match { get; set; }
    }
}
