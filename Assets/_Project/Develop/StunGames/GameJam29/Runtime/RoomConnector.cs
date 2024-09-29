using UnityEngine;

namespace _Project.Develop.StunGames.GameJam29.Runtime
{
    public class RoomConnector : MonoBehaviour
    {
        [SerializeField] private LineRenderer _lineRenderer;
        private bool isVisited;
        
        public void Connect(Room room1, Room room2)
        {
            _lineRenderer.SetPosition(0, room1.transform.position);
            _lineRenderer.SetPosition(1, room2.transform.position);
        }
        
        public void Visit()
        {
            isVisited = true;
        }
    }
}