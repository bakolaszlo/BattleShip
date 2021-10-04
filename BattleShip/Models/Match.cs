using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BattleShip.Models
{
    public class Match
    {
        public int Id { get; set; }

        public int LobbyId { set; get; }
        public int HostHp { set; get; }
        public int GuestHp { set; get; }
        public int HostId { get; set; }

        public int GuestId { get; set; }

        public string HostBoard { get; set; }

        public string GuestBoard { get; set; }

        public bool IsHostTurn { get; set; }

    }
}
