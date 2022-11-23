using Services.Players.Domain;

namespace Game.Players
{
    public class PlayerModel
    {
        public PlayerInfo PlayerInfo { get; }

        public PlayerModel(PlayerInfo playerInfo)
        {
            PlayerInfo = playerInfo;
        }
    }
}