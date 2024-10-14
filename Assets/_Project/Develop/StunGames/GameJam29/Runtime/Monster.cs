using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace _Project.Develop.StunGames.GameJam29.Runtime
{
    public class Monster
    {
        private MatchController _matchController;
        private List<Room> _map;
        private GameConfig _gameConfig;
        private Queue<Room> _shortestWay;
        private int _damage;
        private Room _nextRoom;
        private Room _currentRoom;
        private bool _isPlayerInRoom;
        private bool _isAlarmModeOn;
        private bool _isCooldown;
        
        private Random _random = new Random();
        public Room CurrentRoom => _currentRoom;
        
        public Monster(MatchController matchController, List<Room> map,GameConfig gameConfig)
        {
            _matchController = matchController;
            _map = map;
            _gameConfig = gameConfig;
        }

        public void Initialize()
        {
            SubscribeEvents();
        }
        
        public void Dispose()
        {
            UnSubscribeEvents();
        }

        public void ResetMonster(List<Room> map, Room startRoom)
        {
            _map = map;
            _shortestWay?.Clear();
            _nextRoom = startRoom;
            _currentRoom = startRoom;
            _isPlayerInRoom = false;
            _isCooldown = false;
            _isAlarmModeOn = false;
        }
        
        private void SubscribeEvents()
        {
            EventHolder.OnAlarmSetOn += OnAlarmTrigger;
            EventHolder.OnPlayerAction += NextStep;
        }
        
        private void UnSubscribeEvents()
        {
            EventHolder.OnAlarmSetOn -= OnAlarmTrigger;
            EventHolder.OnPlayerAction -= NextStep;
        }

        private void NextStep()
        {
            if (_isCooldown)
            {
                _isCooldown = false;
                return;
            }
            if (_isAlarmModeOn)
            {
                // _nextRoom = _shortestWay.Peek();
            }
            else ChooseWay();
            MoveToRoom();
            ScanRoom();
        }

        private void MoveToRoom()
        {
            _currentRoom.RemoveMonster();
            _currentRoom = _nextRoom;
            _currentRoom.SetMonsterInRoom(_gameConfig.alwaysShowMonster);
        }
        
        private void ScanRoom()
        {
            if (_currentRoom.IsPlayerInRoom)
            {
                HurtPlayer();
                _isCooldown = true;
                if (_isAlarmModeOn) _isAlarmModeOn = false;
            }
        }
        
        private void OnAlarmTrigger(Room targetRoom)
        {
            _isAlarmModeOn = true;
            FindShortestWay(targetRoom);
        }
        
        private void ChooseWay()
        {
            var nextRoomIndex = _random.Next(0, _currentRoom.ConnectedRooms.Count);
            _nextRoom = _currentRoom.ConnectedRooms[nextRoomIndex];
        }

        private void FindShortestWay(Room targetRoom)
        {
            // int roomCount = _map.Count;
            // var dist = new Dictionary<Room, int>(roomCount); // Массив расстояний
            // var parent = new List<Room>(roomCount); // Массив для восстановления пути
            //
            // for (int i = 0; i < roomCount; i++)
            // {
            //     dist[_map[i]] = int.MaxValue; // Инициализация всех расстояний как бесконечность
            //     parent[i] = null;         // Инициализация всех родителей как -1
            // }
            //
            // dist[_currentRoom] = 0; // Расстояние до стартовой вершины = 0
            // Queue<Room> queue = new Queue<Room>();
            // queue.Enqueue(_currentRoom);
            //
            // while (queue.Count > 0)
            // {
            //     Room curr = queue.Dequeue();
            //
            //     foreach (var connectedRoom in curr.ConnectedRooms)
            //     {
            //         if (dist[connectedRoom] == int.MaxValue)
            //         {
            //             dist[connectedRoom] = dist[curr] + 1;
            //         }
            //     }
            // }
        }
        
        private void HurtPlayer()
        {
            Debug.Log("MONSTER ATTACK");
            _matchController.TakeDamage();
        }
    }
}