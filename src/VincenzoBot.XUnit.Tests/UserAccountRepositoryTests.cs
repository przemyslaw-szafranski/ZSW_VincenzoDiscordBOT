//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;
//using VincenzoBot.Storages;
//using VincenzoBot.Storages.Implementations;
//using VincenzoDiscordBot;
//using VincenzoDiscordBot.Discord;
//using VincenzoDiscordBot.Models;
//using VincenzoDiscordBot.Repositories;
//using Xunit;

//namespace VincenzoBot.XUnit.Tests
//{
//    public class UserAccountRepositoryTests
//    {
//        private readonly UserAccountRepository _userRepo;
//        private readonly DiscordLogger _logger;
//        private readonly Logger _iLogger;
//        private readonly JsonStorage _storage;
//        public UserAccountRepositoryTests()
//        {
//            _iLogger = new Mock<Logger>();
//            _logger = new Mock<DiscordLogger>(_iLogger);
//            _storage = new Mock<JsonStorage>();
//            _userRepo = new Mock<UserAccountRepository>(_logger, _storage);
//        }
//        [Fact]
//        public async Task Test()
//        {
//            //Arrange

//            //Act
//            List<UserAccount> list = _userRepo.LoadOrCreate();
//            //Assert
//            Assert.Null(list);
//        }
//    }
//}
