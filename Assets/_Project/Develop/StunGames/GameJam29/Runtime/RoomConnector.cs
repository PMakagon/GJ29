using System;
using UnityEngine;

namespace _Project.Develop.StunGames.GameJam29.Runtime
{
    [RequireComponent(typeof(LineRenderer))]
    public class RoomConnector : MonoBehaviour
    {
        [SerializeField] private Room roomToConnectTest;
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private Material lineMaterial;
        [SerializeField] private Material dottedMaterial;
        private bool isVisited;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.material = lineMaterial;
            _lineRenderer.enabled = false;
        }
        
        public void Connect(Room room1, Room room2)
        {
            _lineRenderer.enabled = true;
            _lineRenderer.SetPosition(0, room1.transform.position);
            _lineRenderer.SetPosition(1, room2.transform.position);
        }
        [ContextMenu("CONNECT")]
        public void ConnectTest()
        {
            _lineRenderer.SetPosition(0, transform.position);
            _lineRenderer.SetPosition(1, roomToConnectTest.transform.position);
        }

        [ContextMenu("VISIT")]
        public void Visit()
        {
            _lineRenderer.material = dottedMaterial;
            isVisited = true;
        }
        
}
}