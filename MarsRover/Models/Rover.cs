using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarsRover.Models
{
    public class Rover
    {
        public int[] Coordinates { get; set; }
        public Directions Direction { get; set; }

        public Rover()
        {
            Coordinates = new int[2] { 0, 1 };
            Direction = Directions.South;
        }

        public enum Directions
        {
            North,
            South,
            East,
            West
        }

        public string GetDirection()
        {
            switch (Direction)
            {
                case Directions.North:
                    return "North";
                case Directions.South:
                    return "South";
                case Directions.East:
                    return "East";
                case Directions.West:
                    return "West";
                default: return "ERROR: no direction set";
            }
        }

    }
}

