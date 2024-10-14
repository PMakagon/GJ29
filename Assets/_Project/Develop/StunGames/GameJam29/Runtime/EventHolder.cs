using System;

namespace _Project.Develop.StunGames.GameJam29.Runtime
{
    public static class EventHolder
    {
        public static event Action MatchStarted = delegate { };
        public static event Action OnLevelExit = delegate { };
        public static event Action MatchEnded = delegate { };
        public static event Action<Room> OnPlayerRoomClick = delegate { };
        public static event Action<ItemType> OnPlayerItemInteract = delegate { };
        public static event Action OnPlayerAction = delegate { };
        public static event Action<Room> OnPlayerEnterRoom = delegate { };
        public static event Action<Room> onPlayerExitRoom = delegate { };
        public static event Action<Room> OnAlarmSetOn = delegate { };
        public static event Action<int> OnHealthChanged = delegate { };
        public static event Action<Room> OnExitClicked = delegate { };
        

        public static void RaiseLevelExit()
        {
            OnLevelExit?.Invoke();
        }

        public static void RaiseExitClicked(Room room)
        {
            OnExitClicked?.Invoke(room);
        }

        public static void RaiseHealthChanged(int currentHealth)
        {
            OnHealthChanged?.Invoke(currentHealth);
        }

        public static void RaiseMatchStarted()
        {
            MatchStarted?.Invoke();
        }

        public static void RaiseMatchEnded()
        {
            MatchEnded?.Invoke();
        }

        public static void RaisePlayerEnterRoom(Room roomToEnter)
        {
            OnPlayerEnterRoom?.Invoke(roomToEnter);
        }

        public static void RaisePlayerExitRoom(Room roomToExit)
        {
            onPlayerExitRoom?.Invoke(roomToExit);
        }

        public static void RaisePlayerRoomClick(Room room)
        {
            OnPlayerRoomClick?.Invoke(room);
        }

        public static void RaisePlayerItemInteract(ItemType itemType)
        {
            OnPlayerItemInteract?.Invoke(itemType);
        }

        public static void RaisePlayerAction()
        {
            OnPlayerAction?.Invoke();
        }

        public static void RaiseAlarmSetOn(Room room)
        {
            OnAlarmSetOn?.Invoke(room);
        }
    }
}