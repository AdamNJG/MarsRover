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

        Rover GetRover();

        bool GetDirectionFromString(string input, Rover rover, out Directions direction);

        bool ValidateInput(string input, out string error);

        bool CheckValidMovements(string command, Rover rover, out string error);

        Rover GetTempRover();

        List<string> AddCommand(List<string> commands, string input, out string error);

        void SetRoverCoordinates(Rover rover, string command);
        string RemoveMFromCommand(string input);

    }
}
