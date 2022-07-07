using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public void Send(string[] instructions)
        {
            Console.WriteLine("Send");
        }

        public Rover GetRover()
        {
            return rover;
        }

        public List<string> AddCommand(List<string> commands, string input, out string error)
        {
            error = "";
            if (commands.Count < 5)
            {
                if (int.TryParse(input, out int number) == false)
                {
                    commands.Add(input.ToLower());
                }
                else if (number <= 100 && number > 0)
                {
                    commands.Add(input);
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

        public Rover GetTempRover(int number)
        {
            Rover tempRover = GetRover(number).DeepCopy();
            if (tempRover != null)
            {
                return tempRover;
            }
            return new Rover();
        }

        public bool ValidateInput(string input, out string error)
        {
            error = "";
            if (Int32.TryParse(input, out int distance) || input.ToLower().Contains("right") || input.ToLower().Contains("left"))
            {
                return true;
            }
            error = "Value entered is not a command";
            return false;
        }

        public string SetRoverCoordinates(int distance, Rover.Directions direction)
        {
            /*
            //spec says to reverse x and y from standard.
            switch (direction)
            {
                case Rover.Directions.North:
                    rover.Coordinates[0] -= distance;
                    break;
                case Rover.Directions.South:
                    rover.Coordinates[0] += distance;
                    break;
                case Rover.Directions.East:
                    rover.Coordinates[1] -= distance;
                    break;
                case Rover.Directions.West:
                    rover.Coordinates[1] += distance;
                    break;
                default:
                    SetRoverCoordinates(distance, rover.Direction);
                    break;
            }
            rover.Direction = direction;*/
            return "rover set";
        }

        public bool GetDirectionFromString(string input, Rover rover, out Directions direction)
        {
            direction = Directions.North;
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

        public bool CheckValidMovements(List<string> commands, out string error)
        {
            error = "";
            /*Rover tempRover = GetRover(0);
            List<Command> removeCommands = new List<Command>();


            foreach (Command command in commands)
            {
                switch (command.Direction)
                {
                    case Directions.North:
                        tempRover.Coordinates[0] -= command.Distance;
                        break;
                    case Directions.South:
                        tempRover.Coordinates[0] += command.Distance;
                        break;
                    case Directions.East:
                        tempRover.Coordinates[1] += command.Distance;
                        break;
                    case Directions.West:
                        tempRover.Coordinates[1] -= command.Distance;
                        break;
                }

                if (tempRover.Coordinates[0] > 0 && tempRover.Coordinates[0] <= 100 && tempRover.Coordinates[1] > 0 && tempRover.Coordinates[1] <= 100)
                {
                    //rover.Coordinates = tempRover.Coordinates;
                    //rover.Direction = command.Direction;
                    removeCommands.Add(command);
                }
                else
                {
                    removeCommands.Add(command);
                    error = "command would take rover out of bounds";
                    break;
                }
            }
            foreach (Command command in removeCommands)
            {
                commands.Remove(command);
            }
            if (String.IsNullOrEmpty(error))
            {
                return true;
            }*/
            return false;
        }

        public string GetStringFromDirection(Directions direction)
        {
            switch (direction)
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
