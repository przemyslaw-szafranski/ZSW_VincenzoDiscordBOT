using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using VincenzoBot.Discord;
using Xunit;

namespace VincenzoBot.XUnit.Tests
{
    public class SlotMachineTests
    {
        private ISlotMachine _machine;

        [Fact]
        public void Play_Bet1GrapeMiddleRowWon_UserGetReward()
        {
            //Arrange
            _machine = new SlotMachine(40);
            string[,] slots = new string[3, 3]
            {
                { "🍒","🍏","💰" },
                { "🍇","🍇","🍇" },
                { "⚜️","🍒","🔔" }
            };
            _machine.Slots = slots;
            //Act
            uint reward = _machine.Play(9);
            //Assert
            Assert.Equal(12, (int)reward);
        }
        [Fact]
        public void CheckWin_NotEnoughCoinsInMachine_ThrowException()
        {
            //Arrange
            _machine = new SlotMachine(39);
            //Act
            _machine.Prepare();
            //Assert
            Assert.Throws<ArgumentException>(() => _machine.Play(1));
        }
        [Fact]
        public void Play_Bet1NotWon_UserGetReward()
        {
            //Arrange
            _machine = new SlotMachine(40);
            string[,] slots = new string[3, 3]
            {
                { "🍒","🍏","💰" },
                { "🍏","🍇","🍇" },
                { "⚜️","🍒","🔔" }
            };
            _machine.Slots = slots;
            //Act
            uint reward = _machine.Play(9);
            //Assert
            Assert.Equal(0, (int)reward);

            slots = new string[3, 3]
            {
                { "🍒","🍏","💰" },
                { "🍇","🍏","🍇" },
                { "⚜️","🍒","🔔" }
            };
            _machine.Slots = slots;
            //Act
            reward = _machine.Play(9);
            //Assert
            Assert.Equal(0, (int)reward);
            slots = new string[3, 3]
            {
                { "🍒","🍏","💰" },
                { "🍇","🍇","🍏" },
                { "⚜️","🍒","🔔" }
            };
            _machine.Slots = slots;
            //Act
            reward = _machine.Play(9);
            //Assert
            Assert.Equal(0, (int)reward);
        }
        //[Fact]
        //public void CheckWin_Bet1AppleMiddleRowWon_UserGetReward()
        //{
        //    //Arrange
        //    _machine = new Mock<ISlotMachine>().Object;
        //    //Act
        //    uint reward = _machine.Play(9);
        //    //Assert
        //    Assert.Equal(12, (int)reward);
        //}
        //[Fact]
        //public void CheckWin_Bet1CherryMiddleRowWon_UserGetReward()
        //{
        //    //Arrange
        //    _machine = new Mock<ISlotMachine>().Object;
        //    //Act
        //    uint reward = _machine.Play(9);
        //    //Assert
        //    Assert.Equal(12, (int)reward);
        //}
        //[Fact]
        //public void CheckWin_Bet1BellMiddleRowWon_UserGetReward()
        //{
        //    //Arrange
        //    _machine = new Mock<ISlotMachine>().Object;
        //    //Act
        //    uint reward = _machine.Play(9);
        //    //Assert
        //    Assert.Equal(12, (int)reward);
        //}
        //[Fact]
        //public void CheckWin_Bet1JackMiddleRowWon_UserGetReward()
        //{
        //    //Arrange
        //    _machine = new Mock<ISlotMachine>().Object;
        //    //Act
        //    uint reward = _machine.Play(9);
        //    //Assert
        //    Assert.Equal(12, (int)reward);
        //}
    }
}
