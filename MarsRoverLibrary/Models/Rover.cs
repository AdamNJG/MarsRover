using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarsRover.Models
{
    public class Rover
    {
        public int x;
        public int y;
        public Directions Direction { get; set; }

        public Rover()
        {
            x = 1;
            y = 0;
            Direction = Directions.South;
        }

        public enum Directions
        {
            North = 1,
            East = 2,
            South = 3,
            West = 4
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

        public void SetX(int x)
        {
            this.x = x;
        }

        public void SetY(int y)
        {
            this.y = y;
        }
    }
}

