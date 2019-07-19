namespace VincenzoBot.Discord
{
    public interface ISlotMachine
    {
        string[,] Slots { get; set; }
        void Prepare();
        uint Play(uint bet);
    }
}