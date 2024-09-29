using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Project.Develop.StunGames.GameJam29.Runtime
{
    [Serializable]
    public enum RoomState
    {
        Hidden,
        Visible,
        Lighted
    }

    public enum ItemType
    {
        None,
        Health,
        Key,
        Lamp,
        Terminal
    }

    [RequireComponent(typeof(SpriteRenderer))]
    public class Room : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private List<Room> connectedRooms = new List<Room>();
        [SerializeField] private ItemType itemType;
        [SerializeField] private RoomConnector connectorPrefab;
        [SerializeField] private List<RoomConnector> roomConnectors = new List<RoomConnector>();
        [SerializeField] private SpriteRenderer roomViewSpriteRenderer;
        [Header("Room Objects Renderers")] 
        [SerializeField] private SpriteRenderer monsterSpriteRenderer;
        [SerializeField] private SpriteRenderer playerSpriteRenderer;
        [SerializeField] private SpriteRenderer itemSpriteRenderer;
        [SerializeField] private SpriteRenderer lampSpriteRenderer;
        [SerializeField] private SpriteRenderer alarmSpriteRenderer;
        [SerializeField] private SpriteRenderer exitSpriteRenderer;
        [Header("Sprites")]
        [SerializeField] private Sprite roomHiddenSprite;
        [SerializeField] private Sprite roomVisibleSprite;
        
        [SerializeField] private Sprite mosterSprite;
        [SerializeField] private Sprite playerSprite;
        [SerializeField] private Sprite healthSprite;
        [SerializeField] private Sprite keySprite;
        
        [SerializeField] private Sprite alarmOnSprite;
        [SerializeField] private Sprite alarmOffSprite;

        [SerializeField] private Sprite lightOnSprite;
        [SerializeField] private Sprite lightOffSprite;

        [SerializeField] private Sprite terminalOnSprite;
        [SerializeField] private Sprite terminalOffSprite;
        
        private RoomState _state = RoomState.Hidden;
        private bool isAlarmable;
        private bool isAlarmOn;
        private bool isMonsterInRoom;
        private bool isPlayerInRoom;
        private bool hasExit;
        private bool isReady;

        public List<Room> ConnectedRooms => connectedRooms;

        public ItemType ItemType => itemType;

        public bool IsAlarmable => isAlarmable;

        public bool IsAlarmOn => isAlarmOn;

        public bool IsMonsterInRoom => isMonsterInRoom;

        public bool IsPlayerInRoom => isPlayerInRoom;

        public bool HasExit => hasExit;

        public bool IsReady => isReady;


        private void Awake()
        {
            Subscribe();
        }

        public void Configure(List<Room> roomsToConnect,ItemType item, bool isAlarmable, bool hasExit)
        {
            connectedRooms = roomsToConnect;
            SetItem(item);
            this.isAlarmable = isAlarmable;
            this.hasExit = hasExit;
            ConnectTo(roomsToConnect);
            isReady = true;
        }

        private void SetItem(ItemType item)
        {
            itemType = item;
            switch (itemType)
            {
                case ItemType.None:
                    break;
                case ItemType.Health:
                    itemSpriteRenderer.sprite = healthSprite;
                    break;
                case ItemType.Key:
                    itemSpriteRenderer.sprite = keySprite;
                    break;
                case ItemType.Lamp:
                    lampSpriteRenderer.sprite = lightOffSprite;
                    break;
                case ItemType.Terminal:
                    itemSpriteRenderer.sprite = terminalOffSprite;
                    break;
                default:
                    break;
            }
        }

        private void ConnectTo(List<Room> roomsToConnect)
        {
            foreach (var connectedRoom in roomsToConnect)
            {
                var newConnector = Instantiate(connectorPrefab, transform);
                roomConnectors.Add(newConnector);
                newConnector.Connect(this, connectedRoom);
            }
            
        }

        private void Subscribe()
        {
            EventHolder.OnPlayerEnterRoom += OnPlayerEnterRoom;
            EventHolder.onPlayerExitRoom += OnPlayerExitRoom;
            EventHolder.OnPlayerUseItem += OnPlayerUseItem;
        }

        private void Unsubscribe()
        {
            EventHolder.OnPlayerEnterRoom -= OnPlayerEnterRoom;
            EventHolder.onPlayerExitRoom -= OnPlayerExitRoom;
            EventHolder.OnPlayerUseItem -= OnPlayerUseItem;
        }

        #region Events

        private void OnPlayerEnterRoom(Room room)
        {
            isPlayerInRoom = true;
        }

        private void OnPlayerExitRoom(Room room)
        {
            isPlayerInRoom = false;
        }

        private void OnPlayerUseItem(ItemType itemType)
        {
            
        }

        #endregion
        

        #region Input

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!isPlayerInRoom)
            {
                TryMovePlayerToRoom();
            }
            else
            {
                SetRoomVisible();
                if (_state== RoomState.Visible && itemType != ItemType.None)
                {
                    ActivateItem();
                }
            }
           
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
        }

        public void OnPointerExit(PointerEventData eventData)
        {
        }

        #endregion

        

        private void TryMovePlayerToRoom()
        {
            if (isPlayerInRoom) return;
            isPlayerInRoom = true;
            EventHolder.RaisePlayerEnterRoom(this);
            playerSpriteRenderer.enabled = true;
        }

        private void SetRoomVisible()
        {
            if (_state == RoomState.Hidden)
            {
                _state = RoomState.Visible;
                if (isAlarmable) SetAlarmOn();
                roomViewSpriteRenderer.sprite = roomVisibleSprite;
            }
           
        }

        private void ActivateItem()
        {
            switch (itemType)
            {
                case ItemType.None:
                    break;
                case ItemType.Health:
                    AddHealth();
                    break;
                case ItemType.Key:
                    AddKey();
                    break;
                case ItemType.Lamp:
                    _state = RoomState.Lighted;
                    break;
                case ItemType.Terminal:
                    UseTerminal();
                    break;
                default:
                    break;
            }
            EventHolder.RaisePlayerUseItem(itemType);
        }

        private void UseTerminal()
        {
        }

        private void SetAlarmOn()
        {
            isAlarmOn = true;
        }

        private void AddKey()
        {
        }

        private void AddHealth()
        {
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }
    }
}