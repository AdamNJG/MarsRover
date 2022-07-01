using MarsRover.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MarsRover.Models.Rover;

namespace MarsRover.Services
{
    public interface IRoverService
    {
        void Send(string[] instructions);

        Rover GetRover();

        string SetRoverCoordinates(int distance, Rover.Directions direction);

        bool CheckDirections(string input, List<Command> commands, out string error);

        string GetStringFromDirection(Directions direction);

        Directions GetDirectionFromString(string input);

        bool CheckValidMovements(List<Command> commands, out string error);
    }
}
