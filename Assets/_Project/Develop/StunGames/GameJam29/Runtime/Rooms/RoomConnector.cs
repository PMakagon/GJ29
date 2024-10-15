using UnityEngine;

namespace _Project.Develop.StunGames.GameJam29.Runtime.Rooms
{
    [RequireComponent(typeof(LineRenderer))]
    public class RoomConnector : MonoBehaviour
    {
        [SerializeField] private Room originRoom;
        [SerializeField] private Room targetRoom;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private Material lineMaterial;
        [SerializeField] private Material dottedMaterial;
        private bool _isVisited;
        
        public Room OriginRoom => originRoom;

        public Room TargetRoom => targetRoom;

        public bool IsVisited => _isVisited;
        
        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.material = dottedMaterial;
            lineRenderer.enabled = false;
        }
        
        public void Connect(Room originRoomToConnect, Room targetRoomToConnect)
        {
            lineRenderer.enabled = true;
            originRoom = originRoomToConnect;
            targetRoom = targetRoomToConnect;
            lineRenderer.SetPosition(0, originRoom.transform.position);
            lineRenderer.SetPosition(1, targetRoom.transform.position);
        }
        [ContextMenu("CONNECT")]
        public void ConnectTest()
        {
            lineRenderer.SetPosition(0, originRoom.transform.position);
            lineRenderer.SetPosition(1, targetRoom.transform.position);
        }

        [ContextMenu("VISIT")]
        public void Visit()
        {
            lineRenderer.material = lineMaterial;
            _isVisited = true;
        }
        
}
}