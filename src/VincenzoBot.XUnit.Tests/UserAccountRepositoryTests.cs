//using AutoFixture;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;
//using VincenzoBot.Repositories;
//using Xunit;

//namespace VincenzoBot.XUnit.Tests
//{
//    class UserAccountRepositoryTests
//    {
//        [Fact]
//        public async Task GetOrCreateUserAsync_UserIsNull_ShouldCreateUserAccountAsync()
//        {
//            //Arrange
//            var _userRepo = new Mock<UserAccountRepository>();
//            //Act
//            //Assert
//            Assert.NotEqual(result, TimeSpan.Zero);
//        }
//        [Fact]
//        public async Task GiveDailyAsync_UserRecievedDaily24hAgo_TimeSpanEqualZero()
//        {
//            //Arrange
//            var userAccount = new Fixture().Create<UserAccount>();
//            userAccount.LastDaily = DateTime.Now.AddHours(-24);
//            var _userRepo = new Mock<IUserAccountRepository>();
//            var _userService = new UserAccountService(_userRepo);
//            //Act
//            var result = await _userService.GiveDailyAsync(userAccount);
//            //Assert
//            Assert.Equal(result, TimeSpan.Zero);
//        }
//    }
//}
