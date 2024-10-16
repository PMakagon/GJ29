using System;
using _Project.Develop.StunGames.GameJam29.Runtime.Rooms;

namespace _Project.Develop.StunGames.GameJam29.Runtime
{
    public static class EventHolder
    {
        public static event Action onMatchStarted = delegate { };
        public static event Action onLevelExit = delegate { };
        public static event Action onMatchEnded = delegate { };
        public static event Action<Room> onPlayerRoomClick = delegate { };
        public static event Action<ItemType> onPlayerItemInteract = delegate { };
        public static event Action onPlayerAction = delegate { };
        public static event Action<Room> onPlayerEnterRoom = delegate { };
        public static event Action<Room> onPlayerExitRoom = delegate { };
        public static event Action<Room> onAlarmSetOn = delegate { };
        public static event Action<int> onHealthChanged = delegate { };
        public static event Action<Room> onExitClicked = delegate { };
        

        public static void RaiseLevelExit()
        {
            onLevelExit?.Invoke();
        }

        public static void RaiseExitClicked(Room room)
        {
            onExitClicked?.Invoke(room);
        }

        public static void RaiseHealthChanged(int currentHealth)
        {
            onHealthChanged?.Invoke(currentHealth);
        }

        public static void RaiseMatchStarted()
        {
            onMatchStarted?.Invoke();
        }

        public static void RaiseMatchEnded()
        {
            onMatchEnded?.Invoke();
        }

        public static void RaisePlayerEnterRoom(Room roomToEnter)
        {
            onPlayerEnterRoom?.Invoke(roomToEnter);
        }

        public static void RaisePlayerExitRoom(Room roomToExit)
        {
            onPlayerExitRoom?.Invoke(roomToExit);
        }

        public static void RaisePlayerRoomClick(Room room)
        {
            onPlayerRoomClick?.Invoke(room);
        }

        public static void RaisePlayerItemInteract(ItemType itemType)
        {
            onPlayerItemInteract?.Invoke(itemType);
        }

        public static void RaisePlayerAction()
        {
            onPlayerAction?.Invoke();
        }

        public static void RaiseAlarmSetOn(Room room)
        {
            onAlarmSetOn?.Invoke(room);
        }
    }
}