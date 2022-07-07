using Microsoft.VisualStudio.TestTools.UnitTesting;
using MarsRover.Models;
using MarsRover.Services;
using Microsoft.Extensions.Configuration;

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
            Rover originalRover = _roverService.GetRover(0);
            Rover tempRover = _roverService.GetTempRover(0);
            int y = 10;
            int x = 20;

            tempRover.SetX(x);
            tempRover.SetY(y);

            Assert.AreNotEqual(originalRover.Coordinates[0], tempRover.Coordinates[0]);
            Assert.AreNotEqual(originalRover.Coordinates[1], tempRover.Coordinates[1]);
        }

        [TestMethod]
        public void ValidateCommandInputTest()
        {
            Rover rover = new Rover(); // standard rover input 
            string right = "Right";
            string left = "Left";
            string distance = "20";
            string turn = "turn around";

            bool isRight = _roverService.ValidateInput(right);
            bool isLeft = _roverService.ValidateInput(left);
            bool isNumber = _roverService.ValidateInput(distance);
            bool isTurn = _roverService.ValidateInput(turn);

            Assert.IsTrue(isRight);
            Assert.IsTrue(isLeft);
            Assert.IsTrue(isNumber);
            Assert.IsFalse(isTurn);
        }
    }
}