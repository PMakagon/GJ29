using UnityEngine;
using UnityEngine.EventSystems;

namespace _Project.Develop.StunGames.GameJam29.Runtime.Rooms
{
    public class LevelExit : MonoBehaviour,IPointerClickHandler
    {
        [SerializeField] private ExitPosition _exitPosition;
        private bool _isActive;
        private Room room;

        private void Awake()
        {
           EventHolder.MatchStarted += Setup;
        }

        private void Setup()
        {
            EventHolder.MatchStarted -= Setup;
            room = GetComponentInParent<Room>();
            if (room.HasExit && room.ExitPosition == _exitPosition)
            {
                _isActive = true;
                // Debug.Log($"SetExitActive" + room.gameObject.name +" "+ gameObject.name + room.IsReady+ room.ExitPosition+room.HasExit );
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_isActive) return;
            EventHolder.RaiseExitClicked(room);
        }
    }
}