using System;
using System.Collections.Generic;
using VContainer;

namespace _Project.Develop.StunGames.GameJam29.Runtime
{
    public class MatchController : IDisposable
    {
        private GameConfig _gameConfig;
        private LevelGenerator levelGenerator;
        private List<Room> rooms;
        private static int currentHealth;
        private bool _isCard;
        private bool _isInputActive;
        private Room previousRoom;
        private Room currentRoom;
        private Monster _monster;

        public static int CurrentHealth => currentHealth;

        public bool IsCard => _isCard;

        public List<Room> Rooms => rooms;

        [Inject]
        public MatchController(LevelGenerator levelGenerator,GameConfig gameConfig)
        {
            _gameConfig = gameConfig;
            this.levelGenerator = levelGenerator;
        }

        public void Dispose()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            EventHolder.OnPlayerRoomClick += RoomInteract;
            EventHolder.OnPlayerInteract += ItemInteract;
        }
        

        private void Unsubscribe()
        {
            EventHolder.OnPlayerRoomClick -= RoomInteract;
            EventHolder.OnPlayerInteract -= ItemInteract;
        }

        public void Initialize()
        {
            Subscribe();
            CreateLevel();
            _monster = new Monster(this,rooms);
            _monster.Initialize();
        }


        private void CreateLevel()
        {
            levelGenerator.GenerateLevel();
            rooms = levelGenerator.AllRooms;
        }
        
        private ItemType GetRandomItem()
        {
            var random = new System.Random();
            var values = Enum.GetValues(typeof(ItemType));
            var color = random.Next(values.Length);
            return (ItemType)values.GetValue(color);
        }
        
        private void PlaceMonster()
        {
            var random = new System.Random();
            Room startRoom = rooms[random.Next(rooms.Count)];
            while (startRoom == currentRoom)
            {
                startRoom = rooms[random.Next(rooms.Count)];
            }
            _monster.SetStartRoom(startRoom);
            startRoom.SetMonster();
        }

        public void StartMatch()
        {
            _isInputActive = true;
            currentHealth = _gameConfig.StartHp;
            currentRoom = rooms[0];
            currentRoom.MoveIn();
            PlaceMonster();
            EventHolder.RaiseMatchStarted();
            _isInputActive = true;
        }
        
        private void RoomInteract(Room room)
        {
            if (!_isInputActive) return;
            if (room == currentRoom)
            {
                if (room.State == RoomState.Hidden) TakeDamage();
                if (room.State == RoomState.Visible) TakeDamage();
                room.Interact();
                return;
            }
            if (currentRoom.ConnectedRooms.Contains(room))
            {
                previousRoom = currentRoom;
                currentRoom = room;
                previousRoom.RemovePlayer();
                currentRoom.MoveIn();
                TakeDamage();
            }
        }
        
        public void TakeDamage()
        {
            currentHealth--;
            EventHolder.RaiseHealthChanged(currentHealth);
            CheckHealth();
        }
        
        private void AddHealth()
        {
            currentHealth++;
            EventHolder.RaiseHealthChanged(currentHealth);
        }

        private void CheckHealth()
        {
            if (currentHealth<=0)
            {
                EndMatch();
            }
        }
        
        private void ItemInteract(ItemType item)
        {
            switch (item)
            {
                case ItemType.None:
                    return;
                case ItemType.Health:
                    AddHealth();
                    break;
                case ItemType.Key:
                    AddKey();
                    break;
                case ItemType.Lamp:
                    break;
                case ItemType.Terminal:
                    break;
            }
        }

        private void AddKey()
        {
            _isCard = true;
        }
        
        private void EndMatch()
        {
            _isInputActive = false;
            EventHolder.RaiseMatchEnded();
        }
    }
}