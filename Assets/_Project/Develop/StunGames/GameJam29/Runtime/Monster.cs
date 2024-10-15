using System.Collections.Generic;
using System.Linq;
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
            _isCooldown = true;
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
            }
            else
            {
                ChooseWay();
                MoveToRoom();
            }
            ScanRoom();
            if (!_isAlarmModeOn && _random.Next(1, 100) < _gameConfig.MonsterStayChance) _isCooldown = true;
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
            _shortestWay = _currentRoom.GetShortestPath(targetRoom);
            if (_currentRoom == _shortestWay.Peek()) _shortestWay.Dequeue();
        }
        
        private void ChooseWay()
        {
            if (_isAlarmModeOn)
            {
                if (_shortestWay.Any())
                {
                    _nextRoom = _shortestWay.Dequeue();
                }
                if (!_shortestWay.Any())
                {
                    _isAlarmModeOn = false;
                    _isCooldown = true;
                }
            }
            else
            {
                var nextRoomIndex = _random.Next(0, _currentRoom.ConnectedRooms.Count);
                _nextRoom = _currentRoom.ConnectedRooms[nextRoomIndex];
            }
        }
        
        private void HurtPlayer()
        {
            Debug.Log("MONSTER ATTACK");
            _matchController.TakeDamage();
        }
    }
}