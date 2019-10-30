using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using VincenzoBot.Storages.Implementations;
using VincenzoBot;
using VincenzoBot.Discord;
using VincenzoBot.Models;
using VincenzoBot.Repositories;
using Xunit;
using System;
using AutoFixture;
using VincenzoBot.Services.Discord;
using Discord.WebSocket;

namespace VincenzoBot.XUnit.Tests
{
    public class UserAccountServiceTests
    {
        [Fact]
        public async Task GiveDailyAsync_UserRecievedDailyMilliSecondAgo_TimeSpanHigherThanZero()
        {
            //Arrange
            var userAccount = new Fixture().Create<UserAccount>();
            userAccount.LastDaily = DateTime.Now.AddMilliseconds(-1);
            var _userRepo = new Mock<IUserAccountRepository>();
            var _userService = new UserAccountService(_userRepo.Object);
            //Act
            var result = await _userService.GiveDailyAsync(userAccount);
            //Assert
            Assert.NotEqual(result,TimeSpan.Zero);
        }
        [Fact]
        public async Task GiveDailyAsync_UserRecievedDaily24hAgo_TimeSpanEqualZero()
        {
            //Arrange
            var userAccount = new Fixture().Create<UserAccount>();
            userAccount.LastDaily = DateTime.Now.AddHours(-24);
            var _userRepo = new Mock<IUserAccountRepository>();
            var _userService = new UserAccountService(_userRepo.Object);
            //Act
            var result = await _userService.GiveDailyAsync(userAccount);
            //Assert
            Assert.Equal(result, TimeSpan.Zero);
        }
    }
}
