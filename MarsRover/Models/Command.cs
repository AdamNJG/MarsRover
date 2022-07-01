using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MarsRover.Models.Rover;

namespace MarsRover.Models
{
    public class Command
    {
        public int Distance { get; set; }
        public Directions Direction { get; set; }

        public Command(int distance, Rover.Directions direction)
        {
            Distance = distance;
            Direction = direction;
        }
    }
}
