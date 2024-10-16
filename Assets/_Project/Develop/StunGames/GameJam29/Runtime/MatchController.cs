﻿using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Develop.StunGames.GameJam29.Runtime.Audio;
using _Project.Develop.StunGames.GameJam29.Runtime.Rooms;
using UnityEngine;
using VContainer;

namespace _Project.Develop.StunGames.GameJam29.Runtime
{
    public class MatchController : IDisposable
    {
        private GameConfig _gameConfig;
        private LevelGenerator _levelGenerator;
        private PlayerSpawner _playerSpawner;
        private List<Room> _rooms;
        private int _currentHealth;
        private bool _playerHasCard;
        private bool _isInputActive;
        private bool _isFirstMatch;
        private Room _previousRoom;
        private Room _currentRoom;
        private Monster _monster;
        private PlayerView _currentPlayerView;

        public int CurrentHealth => _currentHealth;

        public bool PlayerHasCard => _playerHasCard;

        public bool IsFirstMatch
        {
            get => _isFirstMatch;
            set => _isFirstMatch = value;
        }

        public List<Room> Rooms => _rooms;

        [Inject]
        public MatchController(LevelGenerator levelGenerator,GameConfig gameConfig, PlayerSpawner playerSpawner)
        {
            _gameConfig = gameConfig;
            _levelGenerator = levelGenerator;
            _playerSpawner = playerSpawner;
        }

        public void Dispose()
        {
            Unsubscribe();
            _monster.Dispose();
        }

        private void Subscribe()
        {
            EventHolder.onPlayerRoomClick += RoomInteract;
            EventHolder.onPlayerItemInteract += PlayerItemInteract;
            EventHolder.onPlayerAction += TakeDamage;
            EventHolder.onExitClicked += CheckExit;
        }

        private void CheckExit(Room room)
        {
            if (!_isInputActive) return;
            if (_playerHasCard)
            {
                EventHolder.RaiseLevelExit();
                SoundManager.Instance.CreateSoundBuilder().Play(SoundDataLibrary.Instance.ExitOpen);
            }
            else
            {
                Debug.Log("NO CARD");
                SoundManager.Instance.CreateSoundBuilder().Play(SoundDataLibrary.Instance.ExitClose);
            }
        }


        private void Unsubscribe()
        {
            EventHolder.onPlayerRoomClick -= RoomInteract;
            EventHolder.onPlayerItemInteract -= PlayerItemInteract;
            EventHolder.onPlayerAction -= TakeDamage;
            EventHolder.onExitClicked -= CheckExit;
        }

        public void Initialize()
        {
            Subscribe();
            _monster = new Monster(this,_rooms,_gameConfig);
            _monster.Initialize();
            _isFirstMatch = true;
        }


        private void CreateLevel()
        {
            ClearLevel();
            _levelGenerator.GenerateLevel();
            _rooms = _levelGenerator.AllRooms;
        }

        private void ClearLevel()
        {
            _levelGenerator.ClearLevel();
            _rooms?.Clear();
            _previousRoom = null;
            _currentRoom = null;
            _playerHasCard = false;
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
            List<Room> monsterSpawnRooms = _currentRoom.GetRoomsBeyondDistance(_gameConfig.MonsterMinSpawnDistance);
            if (!monsterSpawnRooms.Any()) monsterSpawnRooms = _currentRoom.GetRoomsAtMaxDistance();
            Room startRoom = monsterSpawnRooms[random.Next(monsterSpawnRooms.Count)];
            _monster.ResetMonster(_rooms, startRoom);
            startRoom.SetMonsterInRoom(_gameConfig.alwaysShowMonster);
        }

        public void StartMatch()
        {
            CreateLevel();
            _currentRoom = _rooms[0];
            PlaceMonster();
            PlacePlayer();
            EventHolder.RaiseMatchStarted();
            _isInputActive = true;
        }

        private void PlacePlayer()
        {
            // TODO: удалить если читаемость на тернарных операциях устраивает
            // if (_gameConfig.godMode) _currentHealth = 999;
            // if (_isFirstMatch) _currentHealth = _gameConfig.StartHp;
            // else _currentHealth += _gameConfig.StartHp;
            
            _currentHealth = _gameConfig.godMode ? 999 : _isFirstMatch ? _gameConfig.StartHp : _currentHealth + _gameConfig.StartHp;
            
            _currentPlayerView = _playerSpawner.SpawnPlayer();
            _currentPlayerView.SetHealth(_currentHealth);
            MovePlayer(_currentRoom);
            _currentPlayerView.Show();
        }

        private void MovePlayer(Room room)
        {
            _currentPlayerView.MoveToRoom(room);
        }

        private void RoomInteract(Room room)
        {
            if (!_isInputActive) return;
            if (room == _currentRoom)
            {
                room.Interact();
                return;
            }
            if (_currentRoom.ConnectedRooms.Contains(room))
            {
                _previousRoom = _currentRoom;
                _currentRoom = room;
                _previousRoom.VisitRoomConnector(room);
                _previousRoom.RemovePlayer();
                _currentPlayerView.MoveToRoom(_currentRoom);
            }
        }

        public void TakeDamage(int damage)
        {
            if (!_gameConfig.godMode) _currentHealth-=damage;
            EventHolder.RaiseHealthChanged(_currentHealth);
            CheckHealth();
        }
        
        public void TakeDamage()
        {
            if (!_gameConfig.godMode) _currentHealth--;
            EventHolder.RaiseHealthChanged(_currentHealth);
            // Debug.Log(_currentHealth);
            CheckHealth();
        }
        
        private void AddHealth()
        {
            _currentHealth+=_gameConfig.HpPacksHealthPoints;
            EventHolder.RaiseHealthChanged(_currentHealth);
        }

        private void CheckHealth()
        {
            if (_currentHealth<=0)
            {
                EndMatch();
            }
        }
        
        private void PlayerItemInteract(ItemType item)
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
            _playerHasCard = true;
        }
        
        private void EndMatch()
        {
            _isInputActive = false;
            EventHolder.RaiseMatchEnded();
        }
    }
}