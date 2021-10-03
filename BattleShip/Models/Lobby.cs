using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BattleShip.Models
{
    public class Lobby
    {
        public int Id { set;get; }
        public int Host { set; get; }
        public int? Guest { set; get; }
    }
}
