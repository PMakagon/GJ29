using System.Collections.Generic;
using Random = System.Random;

namespace _Project.Develop.StunGames.GameJam29.Runtime
{
    public class Monster
    {
        private MatchController _matchController;
        private List<Room> _map;
        private Queue<Room> _shortestWay;
        private int _damage;
        private Room _nextRoom;
        private Room _currentRoom;
        private bool _isPlayerInRoom;
        private bool _isAlarmModeOn;
        private bool _isCooldown;
        
        private Random _random = new Random();
        public Room CurrentRoom => _currentRoom;

        public Monster(MatchController matchController, List<Room> map)
        {
            _matchController = matchController;
            _map = map;
        }

        public void Initialize()
        {
            _isCooldown = false;
            _isAlarmModeOn = false;
            SubscribeEvents();
        }
        
        private void SubscribeEvents()
        {
            EventHolder.OnAlarmSetOn += OnAlarmTrigger;
            EventHolder.OnPlayerRoomClick += NextStep;
        }
        
        private void UnSubscribeEvents()
        {
            EventHolder.OnAlarmSetOn -= OnAlarmTrigger;
            EventHolder.OnPlayerRoomClick -= NextStep;
        }

        public void SetStartRoom(Room room)
        {
            _currentRoom = room;
        }

        private void NextStep(Room itemType)
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
            _currentRoom.SetMonster();
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
        
        public void HurtPlayer()
        {
            _matchController.TakeDamage();
        }

        public void Dispose()
        {
            UnSubscribeEvents();
        }
    }
}