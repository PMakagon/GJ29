using System.Collections.Generic;

namespace _Project.Develop.StunGames.GameJam29.Runtime
{
    public class Monster
    {
        private List<int> _map;
        private List<int> _shortestWay;
        private int _damage;
        private int _nextRoom;
        private int _currentRoom;
        private bool _isPlayerInRoom;
        private bool _isAlarmOn;

        public int CurrentRoom => _currentRoom;

        private void SubscribeEvents()
        {
            
        }

        private void NextStep()
        {
            if (_isAlarmOn)
            {
                _nextRoom = _shortestWay[0];
            }
            else ChooseWay();
            MoveToRoom();
            ScanRoom();
        }

        private void MoveToRoom()
        {
            _currentRoom = _nextRoom;
        }
        
        private void ScanRoom()
        {
            if (_isPlayerInRoom)
            {
                HurtPlayer();
                if (_isAlarmOn) _isAlarmOn = false;
            }
            
        }
        
        private void OnAlarmTrigger()
        {
            _isAlarmOn = true;
            FindShortestWay();
        }
        
        private void ChooseWay()
        {
            //_nextRoom = _currentRoom.ConnectedRooms;
        }

        private void FindShortestWay()
        {
            //_shortestWay = map.FindShortest(targetRoom);
        }
        
        private void HurtPlayer()
        {
            
        }
    }
}