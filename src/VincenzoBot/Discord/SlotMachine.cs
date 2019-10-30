using System;

namespace VincenzoBot.Discord
{
    public class SlotMachine : ISlotMachine
    {
        public uint Coins { get; set; } //Coins in the machine
        public uint Bet { get; set; }
        private const uint _minimumCoins = 40;
        //From lowest to highest win
        private const string Grape = "🍇";
        private const string Apple = "🍏";
        private const string Cherry = "🍒";
        private const string Bell = "🔔";
        private const string Bar = "⚜️";
        private const string Jack = "💰";  //Jackpot
        private readonly Random _randNumGen;
        //3x3 fields
        public string[,] Slots { get; set; } = new string[3, 3];
        private int[,] randNum = new int[3, 3];
        private enum Row
        {
            top = 0,
            middle = 1,
            bottom = 2
        }
        public SlotMachine(uint coins)
        {
            Coins = coins;
            _randNumGen = new Random();
        }
        public void Prepare()
        {
            RandNumbers();
            InitializeSlots();
        }
        public uint Play(uint bet)
        {
            Bet = bet;
            Coins += Bet;
            if (Coins <= _minimumCoins)
                throw new ArgumentException("Not enough coins in machine to provide all possible rewards!", "Coins");    //not sure
            return CheckWin();
        }
        private uint CheckWin()
        {
            uint reward = 0;
            if (Bet < 10)
            {
                reward = CheckRow((int)Row.middle);
            }
            else if (Bet >= 10 && Bet < 30)
            {
                reward = CheckRow((int)Row.top);
                reward += CheckRow((int)Row.middle);
            }
            else if (Bet >= 30 && Bet < 59)
            {
                reward = CheckRow((int)Row.top);
                reward += CheckRow((int)Row.middle);
                reward += CheckRow((int)Row.bottom);
            }
            return reward;
        }
        //    Example
        //    🍒🍏💰
        //    🍇🍇🍇
        //    ⚜️🍒🔔
        private uint CheckRow(int row)
        {
            if (Slots[row, 0] == Slots[row, 1] && Slots[row, 1] == Slots[row, 2])
            {
                if (Slots[row, 0] == Grape)
                    return Bet + 3;
                else if (Slots[row, 0] == Apple)
                    return Bet + 6;
                else if (Slots[row, 0] == Cherry)
                    return Bet + 10;
                else if (Slots[row, 0] == Bell)
                    return Bet + 20;
                else if (Slots[row, 0] == Bar)
                    return Bet + 40;
                //JACKPOT!!!
                else if (row == 1 && Slots[row, 0] == Jack)
                {
                    return Coins;
                }
            }
            return 0;
        }
        private void InitializeSlots()
        {
            string[] slotsToRand = new string[32];
            for (int i = 0; i < 8; i++)
                slotsToRand[i] = Grape;
            for (int i = 8; i < 15; i++)
                slotsToRand[i] = Apple;
            for (int i = 15; i < 21; i++)
                slotsToRand[i] = Cherry;
            for (int i = 21; i < 26; i++)
                slotsToRand[i] = Bell;
            for (int i = 26; i < 30; i++)
                slotsToRand[i] = Bar;
            for (int i = 30; i < 32; i++)
                slotsToRand[i] = Jack;

            for (int row = 0; row < 3; row++)
                for (int col = 0; col < 3; col++)
                    Slots[row, col] = slotsToRand[randNum[row, col]];
        }
        private void RandNumbers()
        {
            randNum[0, 0] = _randNumGen.Next(10000, 20000);
            randNum[0, 1] = _randNumGen.Next(30000, 40000);
            randNum[0, 2] = _randNumGen.Next(50000, 60000);

            randNum[1, 0] = _randNumGen.Next(70000, 80000);
            randNum[1, 1] = _randNumGen.Next(80000, 90000);
            randNum[1, 2] = _randNumGen.Next(90000, 100000);

            randNum[2, 0] = _randNumGen.Next(110000, 120000);
            randNum[2, 1] = _randNumGen.Next(120000, 130000);
            randNum[2, 2] = _randNumGen.Next(140000, 150000);

            for (int row = 0; row < 3; row++)
                for (int col = 0; col < 3; col++)
                    randNum[row, col] = randNum[row, col] % 32;
        }
    }
}
