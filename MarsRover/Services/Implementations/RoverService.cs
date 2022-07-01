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

        public string SetRoverCoordinates(int distance, Rover.Directions direction)
        {
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
            rover.Direction = direction;
            return "rover set";
        }

        public bool CheckDirections(string input, List<Command> commands, out string error)
        {
            string direction = "";
            if (commands.Count > 0)
            {
                direction = GetStringFromDirection(commands.Last().Direction);
            }
            else
            {
                direction = rover.GetDirection();
            }
            if (direction.Contains(GetStringFromDirection(Directions.South)) && input.Contains(GetStringFromDirection(Directions.North)))
            {
                error = "You may only may only make right or left hand turns!";
                return false;
            }
            if (direction.Contains(GetStringFromDirection(Directions.North)) && input.Contains(GetStringFromDirection(Directions.South)))
            {
                error = "You may only may only make right or left hand turns!";
                return false;
            }
            if (direction.Contains(GetStringFromDirection(Directions.East)) && input.Contains(GetStringFromDirection(Directions.West)))
            {
                error = "You may only may only make right or left hand turns!";
                return false;
            }
            if (direction.Contains(GetStringFromDirection(Directions.West)) && input.Contains(GetStringFromDirection(Directions.East)))
            {
                error = "You may only may only make right or left hand turns!";
                return false;
            }
            error = "";
            return true;
        }

        public bool CheckValidMovements(List<Command> commands, out string error)
        {
            Rover tempRover = GetRover();
            List<Command> removeCommands = new List<Command>();
            error = "";

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
                    rover.Coordinates = tempRover.Coordinates;
                    rover.Direction = command.Direction;
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
            }
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

        public Directions GetDirectionFromString(string input)
        {
            switch (input)
            {
                case "North":
                    return Directions.North;
                case "South":
                    return Directions.South;
                case "East":
                    return Directions.East;
                case "West":
                    return Directions.West;
                default: return Directions.South;
            }
        }
    }

}
