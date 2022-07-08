using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MarsRover.Models;
using Microsoft.Extensions.Configuration;
using static MarsRover.Models.Rover;

namespace MarsRover.Services
{
    public class RoverService : IRoverService
    {
        public Rover rover;
        private readonly IConfiguration _config;
        public RoverService(IConfiguration config)
        {
            _config = config;
            int size;
            if (Int32.TryParse(config["gridSize"], out size) != true)
            {
                size = 10;
            }
            rover = new Rover();
        }
        public Rover GetRover()
        {
            return rover;
        }

        public List<string> AddCommand(List<string> commands, string input, out string error)
        {
            error = "";
            string pattern = @"d*m";
            Match match = Regex.Match(input, pattern);
            if (commands.Count < 5)
            {

                if (int.TryParse(RemoveMFromCommand(input), out int number) == false)
                {
                    commands.Add(input.ToLower());
                }
                else if (match.Success && number <= 100 && number > 0)
                {
                    commands.Add(number.ToString());
                }
                else
                {
                    error = "Command not added, use positive numbers up to 100";
                }
            }
            else
            {
                error = "You may only enter a maximum of 5 commands";
            }

            return commands;
        }

        public string RemoveMFromCommand(string input)
        {
            return input.Split('m')[0];
        }

        public Rover GetTempRover()
        {
            Rover tempRover = GetRover().DeepCopy();
            if (tempRover != null)
            {
                return tempRover;
            }
            return new Rover();
        }

        public bool ValidateInput(string input, out string error)
        {
            string pattern = @"d*m";
            Match match = Regex.Match(input, pattern);
            error = "";

            if (input.ToLower().Contains(" ") == false && (match.Success && Int32.TryParse(input.Split('m')[0], out int distance) || input.ToLower().Contains("right") || input.ToLower().Contains("left")))
            {
                return true;
            }
            error = "Value entered is not a command, only 'right', 'left' and positive integers followed by m bellow 100 allowed, no spaces";
            return false;
        }

        public void SetRoverCoordinates(Rover rover, string command)
        {
            if (int.TryParse(command, out int distance))
            {
                switch (rover.Direction)
                {
                    case Directions.North:
                        rover.SetY(rover.y - distance);
                        break;
                    case Directions.South:
                        rover.SetY(rover.y + distance);
                        break;
                    case Directions.East:
                        rover.SetX(rover.x + distance);
                        break;
                    case Directions.West:
                        rover.SetX(rover.x - distance);
                        break;
                }
            }
        }

        public bool GetDirectionFromString(string input, Rover rover, out Directions direction)
        {
            direction = rover.Direction;
            if (Int32.TryParse(input, out int distance))
            {
                return false;
            }

            if (input.ToLower().Contains("right"))
            {
                direction = (int)rover.Direction == 4 ? (Directions)1 : rover.Direction + 1;
                return true;
            }
            else if (input.ToLower().Contains("left"))
            {
                direction = (int)rover.Direction == 1 ? (Directions)4 : rover.Direction - 1;
                return true;
            }

            return false;
        }

        public bool CheckValidMovements(string command, Rover rover, out string error)
        {
            error = "";
            SetRoverCoordinates(rover, command);

            if ((rover.x > 0 && rover.x < 100) && (rover.y > 0 && rover.y < 100))
            {
                return true;
            }
            error = "A command would take the rover out of bounds, aborting";
            return false;
        }
    }



}
