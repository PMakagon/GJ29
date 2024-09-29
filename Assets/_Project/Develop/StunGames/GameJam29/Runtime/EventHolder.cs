using System;

namespace _Project.Develop.StunGames.GameJam29.Runtime
{
    public static class EventHolder
    {
        public static event Action<Room> OnPlayerEnterRoom = delegate { };
        public static event Action<Room> onPlayerExitRoom = delegate { };
        public static event Action<ItemType> OnPlayerUseItem = delegate { };
        
        public static void RaisePlayerEnterRoom(Room roomToEnter)
        {
            OnPlayerEnterRoom?.Invoke(roomToEnter);
        }
        public static void RaisePlayerExitRoom(Room roomToExit)
        {
            onPlayerExitRoom?.Invoke(roomToExit);
        }
        
        public static void RaisePlayerUseItem(ItemType itemType)
        {
            OnPlayerUseItem?.Invoke(itemType);
        }
        
    }
}