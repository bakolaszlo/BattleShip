using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BattleShip.Models
{
    public class Lobby
    {
        public int Id { set; get; }
        public Player Host { set; get; }
        public Player? Guest { set; get; }
    }
}
