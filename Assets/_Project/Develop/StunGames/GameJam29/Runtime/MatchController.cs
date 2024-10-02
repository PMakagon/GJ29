using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Develop.StunGames.GameJam29.Runtime
{
    public class MatchController :PersistentSingleton<MatchController>
    {
        [SerializeField] private int startHealth = 17;
        [SerializeField] private LevelGenerator levelGenerator;
        [SerializeField] private List<Room> rooms;
        private int currentHealth;
        private bool _isCard;
        private bool _isInputActive;
        private Room previousRoom;
        private Room currentRoom;

        public int CurrentHealth => currentHealth;

        public bool IsCard => _isCard;

        public List<Room> Rooms => rooms;


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

        private void Start()
        {
            Subscribe();
            ConfigureRooms();
            StartMatch();
        }


        private void ConfigureRooms()
        {
            levelGenerator.GenerateLevel();
            rooms = levelGenerator.AllRooms;
            Debug.Log("rooms.Count = " + rooms.Count);
        }
        
        private ItemType GetRandomItem()
        {
            var random = new System.Random();
            var values = Enum.GetValues(typeof(ItemType));
            var color = random.Next(values.Length);
            return (ItemType)values.GetValue(color);
        }

        public void StartMatch()
        {
            Debug.Log("MATCH START");
            _isInputActive = true;
            currentHealth = startHealth;
            currentRoom = rooms[0];
            currentRoom.MoveIn();
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
        
        private void TakeDamage()
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