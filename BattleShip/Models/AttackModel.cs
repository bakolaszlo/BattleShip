using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BattleShip.Models
{
    public class AttackModel
    {
        public int LobbyId { set; get; }
        public int GuestId { get; set; }
        public int HostId { get; set; }
        public int AttackIndex { get; set; }
    }
}
