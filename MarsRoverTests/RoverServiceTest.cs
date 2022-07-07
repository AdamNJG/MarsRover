using Microsoft.VisualStudio.TestTools.UnitTesting;
using MarsRover.Models;
using MarsRover.Services;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using static MarsRover.Models.Rover;

namespace MarsRoverTests
{
    [TestClass]
    public class RoverServiceTest
    {
        IConfiguration _config;
        IRoverService _roverService;

        public RoverServiceTest()
        {
            _config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
            _roverService = new RoverService(_config);
        }

        [TestMethod]
        public void GetTempRoverTest()
        {
            //Given a Copy of a rover object
            Rover originalRover = _roverService.GetRover(0);
            Rover tempRover = _roverService.GetTempRover(0);

            //When I change the coordinates of the copy Rover
            int y = 10;
            int x = 20;
            tempRover.SetX(x);
            tempRover.SetY(y);
            tempRover.Direction = Directions.North;

            //Then the coordinates of the original and copy should not match
            Assert.AreNotEqual(originalRover.Coordinates[0], tempRover.Coordinates[0]);
            Assert.AreNotEqual(originalRover.Coordinates[1], tempRover.Coordinates[1]);
            Assert.AreNotEqual(originalRover.Direction, tempRover.Direction);
        }

        [TestMethod]
        public void ValidateCommandInputTest()
        {
            //give some unvalidated commands
            Rover rover = new Rover(); // standard rover input 
            string right = "Right";
            string left = "left";
            string distance = "20";
            string turn = "turn around";

            //When I check if they are validated, if not an error is generated
            bool isRight = _roverService.ValidateInput(right, out string rightError);
            bool isLeft = _roverService.ValidateInput(left, out string leftError);
            bool isNumber = _roverService.ValidateInput(distance, out string distanceError);
            bool isTurn = _roverService.ValidateInput(turn, out string turnError);

            //Then only valid commands should pass the check, and errors should be passed up
            Assert.IsTrue(isRight);
            Assert.IsTrue(isLeft);
            Assert.IsTrue(isNumber);
            Assert.IsFalse(isTurn);
            Assert.IsTrue("".Contains(rightError));
            Assert.IsTrue("".Contains(leftError));
            Assert.IsTrue("".Contains(distanceError));
            Assert.IsFalse("".Contains(turnError));
        }

        [TestMethod]
        public void AddCommandTest()
        {
            //Given some prevalidated commands
            List<string> commands = new List<string>();
            string right = "Right";
            string left = "Left";
            string distance = "20";
            string longDistance = "120";

            //When added to the command list
            commands = _roverService.AddCommand(commands, right, out string rightError);
            commands = _roverService.AddCommand(commands, left, out string leftError);
            commands = _roverService.AddCommand(commands, distance, out string distanceError);
            commands = _roverService.AddCommand(commands, longDistance, out string longDistanceError);

            //Then they are contained within the list
            Assert.IsTrue(commands.Exists(e => e.Contains(right.ToLower())));
            Assert.IsTrue(commands.Exists(e => e.Contains(left.ToLower())));
            Assert.IsTrue(commands.Exists(e => e.Contains(distance)));

            //And if an integer is over 100, this will produce an error and not be added to the list
            Assert.IsFalse(commands.Exists(e => e.Contains(longDistance)));
            Assert.IsTrue(rightError.Contains(""));
            Assert.IsTrue(leftError.Contains(""));
            Assert.IsTrue(distanceError.Contains(""));
            Assert.IsTrue(longDistanceError.Contains("Command not added, use positive numbers up to 100"));
        }

        [TestMethod]
        public void AddCommandCountTest()
        {
            //Given more than 5 commands
            List<string> commands = new List<string>();
            string right = "Right";
            string two = "2";
            string left = "left";
            string ten = "10";
            string twelve = "12";
            string five = "5";

            //When added to the command list
            commands = _roverService.AddCommand(commands, right, out string rightError);
            commands = _roverService.AddCommand(commands, two, out string twoError);
            commands = _roverService.AddCommand(commands, left, out string leftError);
            commands = _roverService.AddCommand(commands, ten, out string tenError);
            commands = _roverService.AddCommand(commands, twelve, out string twelveError);
            commands = _roverService.AddCommand(commands, five, out string fiveError);

            //Then there will only be the first 5 commands in the list
            Assert.IsTrue(commands.Count == 5);
            Assert.IsTrue(commands.Exists(e => e.Contains(right.ToLower())));
            Assert.IsTrue(commands.Exists(e => e.Contains(two)));
            Assert.IsTrue(commands.Exists(e => e.Contains(left.ToLower())));
            Assert.IsTrue(commands.Exists(e => e.Contains(ten)));
            Assert.IsTrue(commands.Exists(e => e.Contains(twelve)));
            Assert.IsFalse(commands.Exists(e => e.Contains(five)));

            //And if they are not added they will produce an error
            Assert.IsTrue(rightError.Contains(""));
            Assert.IsTrue(twoError.Contains(""));
            Assert.IsTrue(leftError.Contains(""));
            Assert.IsTrue(tenError.Contains(""));
            Assert.IsTrue(twelveError.Contains(""));
            Assert.IsTrue(fiveError.Contains("You may only enter a maximum of 5 commands"));
        }

        [TestMethod]
        public void StringDirectionToEnumTest()
        {
            //Given a command and a Rover (default south direction)
            Rover rover = new Rover();
            string right = "Right";
            string left = "Left";
            string distance = "20";

            //When I check to see if they are valid directional commands
            bool isRight = _roverService.GetDirectionFromString(right, rover, out Directions rightDirection);
            rover.Direction = Directions.South;
            bool isLeft = _roverService.GetDirectionFromString(left, rover, out Directions leftDirection);
            bool isDistance = _roverService.GetDirectionFromString(distance, rover, out Directions distanceDirection);
            rover.Direction = Directions.West;
            bool isRightFromWest = _roverService.GetDirectionFromString(right, rover, out Directions rightFromWestDirection);
            rover.Direction = Directions.North;
            bool isLeftFromNorth = _roverService.GetDirectionFromString(left, rover, out Directions leftFromNorthDirection);

            //Then Valid commands will be converted to the enum, if not valid then returns an error
            Assert.IsTrue(isRight);
            Assert.IsTrue(isLeft);
            Assert.IsTrue(isRightFromWest);
            Assert.IsTrue(isLeftFromNorth);
            Assert.IsFalse(isDistance);

            Assert.AreEqual(Directions.West, rightDirection);
            Assert.AreEqual(Directions.East, leftDirection);
            Assert.AreEqual(Directions.North, rightFromWestDirection);
            Assert.AreEqual(Directions.West, leftFromNorthDirection);
        }


    }
}