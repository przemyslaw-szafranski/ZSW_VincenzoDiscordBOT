//using Discord.WebSocket;
//using Unity;
//using Unity.Injection;
//using VicenzoDiscordBot;
//using VicenzoDiscordBot.Modules;
//using VincenzoBot.Storages;
//using VincenzoBot.Storages.Implementations;
//using VincenzoDiscordBot;
//using VincenzoDiscordBot.Discord;
//using VincenzoDiscordBot.Discord.Entities;
//using VincenzoDiscordBot.Entities;
//using VincenzoDiscordBot.Repositories;

//namespace VincenzoBot
//{
//    public static class Unity
//    {
//        public static UnityContainer _container;

//        public static UnityContainer Container
//        {
//            get
//            {
//                if (_container == null)
//                    RegisterTypes();
//                return _container;
//            }
//        }

//        public static void RegisterTypes()
//        {
//            _container = new UnityContainer();
//            _container.RegisterSingleton<IDataStorage, JsonStorage>();
//            _container.RegisterSingleton<ILogger,Logger>();
//            _container.RegisterSingleton<DiscordLogger>();
//            _container.RegisterSingleton<BotConfigRepository>();
//            _container.RegisterSingleton<VincenzoDiscordBot.Repositories.UserAccountRepository>();
//            //_container.RegisterSingleton<User>(new InjectionConstructor(typeof(UserAccountRepository)));
//            _container.RegisterType<DiscordSocketConfig>(new InjectionFactory(i => SocketConfig.GetDefault()));
//            _container.RegisterType<DiscordSocketClient>(new InjectionConstructor(typeof(DiscordSocketConfig)));
//            _container.RegisterSingleton<Connection>(); //Discord connection
//            //_container.RegisterType<>();
//        }

//        internal static T Resolve<T>() => _container.Resolve<T>();  
//    }
//}
