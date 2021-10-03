using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BattleShip.Models
{
    public class Match
    {
        public int Id { get; set; }

        public Player Host { get; set; }

        public Player Guest { get; set; }

        public string HostBoard { get; set; }

        public string GuestBoard { get; set; }

        public bool isHostTurn { get; set; }
    }
}
