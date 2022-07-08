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
            Rover originalRover = _roverService.GetRover();
            Rover tempRover = _roverService.GetTempRover();

            //When I change the coordinates of the copy Rover
            tempRover.SetX(20);
            tempRover.SetY(10);
            tempRover.Direction = Directions.North;

            //Then the coordinates of the original and copy should not match
            Assert.AreNotEqual(originalRover.x, tempRover.x);
            Assert.AreNotEqual(originalRover.y, tempRover.y);
            Assert.AreNotEqual(originalRover.Direction, tempRover.Direction);
        }

        [TestMethod]
        public void ValidateCommandInputTest()
        {
            //give some unvalidated commands
            Rover rover = new Rover(); // standard rover input 
            string right = "Right";
            string left = "left";
            string distance = "20m";
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
            string distance = "20m";
            string longDistance = "120";

            //When added to the command list
            commands = _roverService.AddCommand(commands, right, out string rightError);
            commands = _roverService.AddCommand(commands, left, out string leftError);
            commands = _roverService.AddCommand(commands, distance, out string distanceError);
            commands = _roverService.AddCommand(commands, longDistance, out string longDistanceError);

            //Then they are contained within the list (distance commands have m stripped away)
            Assert.IsTrue(commands.Exists(e => e.Contains(right.ToLower())));
            Assert.IsTrue(commands.Exists(e => e.Contains(left.ToLower())));
            Assert.IsTrue(commands.Exists(e => e.Contains(distance.Split('m')[0])));

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
            string two = "2m";
            string left = "left";
            string ten = "10m";
            string twelve = "12m";
            string five = "5m";

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
            Assert.IsTrue(commands.Exists(e => e.Contains(_roverService.RemoveMFromCommand(two))));
            Assert.IsTrue(commands.Exists(e => e.Contains(left.ToLower())));
            Assert.IsTrue(commands.Exists(e => e.Contains(_roverService.RemoveMFromCommand(ten))));
            Assert.IsTrue(commands.Exists(e => e.Contains(_roverService.RemoveMFromCommand(twelve))));
            Assert.IsFalse(commands.Exists(e => e.Contains(_roverService.RemoveMFromCommand(five))));

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

            //Then Valid commands will be converted to the enum, if not valid then returns an error, ensure that rover is not mutated
            Assert.IsTrue(isRight);
            Assert.IsTrue(isLeft);
            Assert.IsTrue(isRightFromWest);
            Assert.IsTrue(isLeftFromNorth);
            Assert.IsFalse(isDistance);

            Assert.AreEqual(Directions.West, rightDirection);
            Assert.AreEqual(Directions.East, leftDirection);
            Assert.AreEqual(Directions.North, rightFromWestDirection);
            Assert.AreEqual(Directions.West, leftFromNorthDirection);
            Assert.AreEqual(rover.Direction, Directions.North);
        }

        [TestMethod]
        public void CheckDistanceOutOfBoundsTest()
        {
            //Given a rover and a distance to move
            Rover rover = new Rover();
            string distance = "1";
            string distance2 = "5";
            string distance3 = "99";
            string notDistance = "test";

            //When the rover is moved for the distance specified
            bool dis1 = _roverService.CheckValidMovements(distance, rover, out string dis1Error);
            bool dis2 = _roverService.CheckValidMovements(distance2, rover, out string dis2Error);
            bool dis3 = _roverService.CheckValidMovements(distance3, rover, out string dis3Error);
            bool notDis = _roverService.CheckValidMovements(notDistance, rover, out string notDisError);

            //Then The rover should not go out of bounds or accept non integer strings
            Assert.IsTrue(dis1);
            Assert.IsTrue(dis2);
            Assert.IsFalse(dis3);
            Assert.IsFalse(notDis);

            Assert.IsTrue(string.IsNullOrEmpty(dis1Error));
            Assert.IsTrue(string.IsNullOrEmpty(dis2Error));
            Assert.IsTrue(string.IsNullOrEmpty(dis3Error) == false);
            Assert.IsTrue(string.IsNullOrEmpty(notDisError) == false);
        }

        [TestMethod]
        public void SetRoverXYTest()
        {
            //Given a distance command and four rovers facing along tow different planes
            Rover rover = new Rover();
            Rover rover2 = new Rover();
            rover2.Direction = Directions.North;
            rover2.SetY(20);
            Rover rover3 = new Rover();
            rover3.Direction = Directions.East;
            Rover rover4 = new Rover();
            rover4.Direction = Directions.West;
            rover4.SetX(20);
            string distance = "5";


            //Whem the command is parsed
            _roverService.SetRoverCoordinates(rover, distance);
            _roverService.SetRoverCoordinates(rover2, distance);
            _roverService.SetRoverCoordinates(rover3, distance);
            _roverService.SetRoverCoordinates(rover4, distance);


            //Then the rover moves in that direction
            Assert.AreEqual(5, rover.y);
            Assert.AreEqual(15, rover2.y);
            Assert.AreEqual(6, rover3.x);
            Assert.AreEqual(15, rover4.x);
        }

        [TestMethod]
        public void RemoveMFromCommandTest()
        {
            //Given an integer command
            string command = "20m";

            //When stripping the m from the command
            string result = _roverService.RemoveMFromCommand(command);

            //Then I am left with the integer as a string
            Assert.AreEqual("20", result);
        }
    }
}