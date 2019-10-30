﻿using System;
using Newtonsoft.Json;
using System.IO;
using VincenzoDiscordBot;
using VincenzoBot.Storages;
using VincenzoDiscordBot.Discord;
using VincenzoDiscordBot.Entities;

namespace VicenzoDiscordBot
{
    public class BotConfigRepository
    {
        private readonly IDataStorage _dataStorage;
        private readonly DiscordLogger _logger;
        public DiscordBotConfig _config { get; set; }

        public BotConfigRepository(DiscordLogger logger ,IDataStorage dataStorage, DiscordBotConfig config)
        {
            _logger = logger;
            _dataStorage = dataStorage;
            _config = config;
            LoadOrCreate();
        }
        public void LoadOrCreate()
        {
            _logger.Log($"Reading the configuration ({Constants.DISCORD_CONFIG_PATH})");
            var discordBotConfig = _dataStorage.RestoreObject<DiscordBotConfig>(Constants.DISCORD_CONFIG_PATH);
            if (discordBotConfig == null)
                _dataStorage.StoreObject(discordBotConfig, Constants.DISCORD_CONFIG_PATH);
            _config = discordBotConfig;
        }

        public void Save()
        {
            _logger.Log($"Saving the configuration ({Constants.DISCORD_CONFIG_PATH})");
            _dataStorage.StoreObject(_config, Constants.DISCORD_CONFIG_PATH);
        }
    }
}
