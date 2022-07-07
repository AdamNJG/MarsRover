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

        bool GetDirectionFromString(string input, Rover rover, out Directions direction);

        string GetStringFromDirection(Directions direction);


        public bool ValidateInput(string input, out string error);

        bool CheckValidMovements(List<string> commands, out string error);

        Rover GetTempRover(int number);

        List<string> AddCommand(List<string> commands, string input, out string error);
    }
}
