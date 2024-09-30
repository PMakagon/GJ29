using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

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
        [SerializeField] private SpriteRenderer roomSpriteRenderer;
        [SerializeField] private SpriteRenderer roomHiddenSpriteRenderer;
        [SerializeField] private Player player;
        

        [Header("Room Objects Renderers")] 
        [SerializeField] private SpriteRenderer monsterSpriteRenderer;
        
        [SerializeField] private SpriteRenderer itemSpriteRenderer;
        [SerializeField] private SpriteRenderer lampSpriteRenderer;
        [SerializeField] private SpriteRenderer alarmSpriteRenderer;
        [SerializeField] private SpriteRenderer terminalSpriteRenderer;
        [SerializeField] private SpriteRenderer exitSpriteRenderer;
        [Header("Sprites")] 
        [SerializeField] private Sprite roomDefaultSprite;
        [SerializeField] private Sprite roomSelectedSprite;

        [SerializeField] private Sprite healthSprite;
        [SerializeField] private Sprite keySprite;

        [SerializeField] private Sprite alarmOnSprite;
        [SerializeField] private Sprite alarmOffSprite;

        [SerializeField] private Sprite lightOnSprite;
        [SerializeField] private Sprite lightOffSprite;

        [SerializeField] private Sprite terminalOnSprite;
        [SerializeField] private Sprite terminalOffSprite;

        private RoomState _state = RoomState.Hidden;
        
        private bool _isAlarmable;
        private bool _isAlarmOn;
        private bool _isMonsterInRoom;
        private bool _isPlayerInRoom;
        private bool _hasExit;
        private bool _isReady;

        public RoomState State => _state;

        public List<Room> ConnectedRooms => connectedRooms;

        public ItemType ItemType => itemType;

        public bool IsAlarmable => _isAlarmable;

        public bool IsAlarmOn => _isAlarmOn;

        public bool IsMonsterInRoom => _isMonsterInRoom;

        public bool IsPlayerInRoom => _isPlayerInRoom;

        public bool HasExit => _hasExit;

        public bool IsReady => _isReady;


        public void Configure(List<Room> roomsToConnect, ItemType item, bool isAlarmable, bool hasExit)
        {
            connectedRooms = roomsToConnect;
            SetItem(item);
            this._isAlarmable = isAlarmable;
            this._hasExit = hasExit;
            ConnectTo(roomsToConnect);
            _isReady = true;
        }

        public void Interact()
        {
            Debug.Log("INTERACT");
            if (_state == RoomState.Hidden)
            {
                SetRoomVisible();
                return;
            }
            
            if (_state == RoomState.Visible && itemType != ItemType.None)
            {
                ActivateItem();
            }
        }

        public void MoveIn()
        {
            if (!_isPlayerInRoom)
            {
                SetPlayerInRoom();
            }
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


        #region Input

        public void OnPointerClick(PointerEventData eventData)
        {
            EventHolder.RaisePlayerRoomClick(this);
            // Debug.Log("ROOM CLICK");
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // Debug.Log("POINTER ENTER"); 
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // Debug.Log("POINTER EXIT");
        }

        #endregion

        public void SetMonster()
        {
            if (_isMonsterInRoom) return;
            _isMonsterInRoom = true;
            if (_isPlayerInRoom || _state == RoomState.Lighted)
            {
                monsterSpriteRenderer.enabled = true;
            }
        }

        public void RemoveMonster()
        {
            if (!_isMonsterInRoom) return;
            _isMonsterInRoom = false;
            monsterSpriteRenderer.enabled = false;
        }

        private void SetPlayerInRoom()
        {
            if (_isPlayerInRoom) return;
            Debug.Log("PLAYER INSIDE ROOM");
            _isPlayerInRoom = true;
            EventHolder.RaisePlayerEnterRoom(this); 
            player.Show();
            roomHiddenSpriteRenderer.enabled = false;
        }

        public void RemovePlayer()
        {
            Debug.Log("PLAYER REMOVED");
            if (!_isPlayerInRoom) return;
            _isPlayerInRoom = false;
            EventHolder.RaisePlayerExitRoom(this);
            player.Hide();
        }

        private void SetRoomVisible()
        {
            Debug.Log("SET VISIBLE");
            if (_state == RoomState.Hidden)
            {
                _state = RoomState.Visible;
                if (_isAlarmable) SetAlarmOn();
                roomSpriteRenderer.sprite = roomDefaultSprite;
                exitSpriteRenderer.enabled = _hasExit;
                switch (itemType)
                {
                    case ItemType.None:
                        break;
                    case ItemType.Health:
                        itemSpriteRenderer.sprite = healthSprite;
                        itemSpriteRenderer.enabled = true;
                        break;
                    case ItemType.Key:
                        itemSpriteRenderer.sprite = keySprite;
                        itemSpriteRenderer.enabled = true;
                        break;
                    case ItemType.Lamp:
                        lampSpriteRenderer.sprite = lightOffSprite;
                        lampSpriteRenderer.enabled = true;
                        break;
                    case ItemType.Terminal:
                        terminalSpriteRenderer.sprite = terminalOffSprite;
                        terminalSpriteRenderer.enabled = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void ActivateItem()
        {
            Debug.Log("ACTIVATE ITEM " + itemType);
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
                    lampSpriteRenderer.sprite = lightOnSprite;
                    break;
                case ItemType.Terminal:
                    UseTerminal();
                    break;
                default:
                    break;
            }

            EventHolder.RaisePlayerInteract(itemType);
        }

        private void UseTerminal()
        {
            EventHolder.RaisePlayerInteract(ItemType.Terminal);
            terminalSpriteRenderer.sprite = terminalOnSprite;
        }

        private void SetAlarmOn()
        {
            alarmSpriteRenderer.enabled = true;
            alarmSpriteRenderer.sprite = alarmOnSprite;
            _isAlarmOn = true;
            EventHolder.RaiseAlarmSetOn(this);
        }

        private void AddKey()
        {
            itemSpriteRenderer.enabled = false;
        }

        private void AddHealth()
        {
            itemSpriteRenderer.enabled = false;
        }

        private void OnDestroy()
        {
        }
    }
}