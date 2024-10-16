﻿using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Develop.StunGames.GameJam29.Runtime.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace _Project.Develop.StunGames.GameJam29.Runtime.Rooms
{
    [Serializable]
    public enum RoomState
    {
        Hidden,
        Visible,
        Lighted
    }
    [Serializable]
    public enum RoomType
    {
        SERV,
        MTN,
        RLY,
        DCK,
        CMD,
        MED,
        M,
        L,
        CR,
        CTR,
        AIR,
        FUEL,
        SRC
    }
    
    
    [Serializable]
    public enum ItemType
    {
        None,
        Health,
        Key,
        Lamp,
        Terminal
    }
    [Serializable]
    public enum ExitPosition
    {
        None,
        Up,
        Down,
        Left,
        Right
    }

    [RequireComponent(typeof(SpriteRenderer))]
    public class Room : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private List<Room> connectedRooms = new List<Room>();
        [SerializeField] private ItemType itemType;
        [SerializeField] private RoomConnector connectorPrefab;
        [SerializeField] private List<RoomConnector> roomConnectors = new List<RoomConnector>();
        [SerializeField] private SpriteRenderer roomSpriteRenderer;
        [SerializeField] private SpriteRenderer itemIndicatorSpriteRenderer;
        [SerializeField] private SpriteRenderer roomHiddenSpriteRenderer;
        [SerializeField] private Transform playerPoint;
        [SerializeField] private TextMeshPro roomNameLabel;
        

        [Header("Rooms Objects Renderers")] 
        [SerializeField] private SpriteRenderer monsterSpriteRenderer;
        [SerializeField] private SpriteRenderer itemSpriteRenderer;
        [SerializeField] private SpriteRenderer lampSpriteRenderer;
        [SerializeField] private SpriteRenderer alarmSpriteRenderer;
        [SerializeField] private SpriteRenderer terminalSpriteRenderer;
        [SerializeField] private SpriteRenderer exitUpSpriteRenderer;
        [SerializeField] private SpriteRenderer exitDownSpriteRenderer;
        [SerializeField] private SpriteRenderer exitLeftSpriteRenderer;
        [SerializeField] private SpriteRenderer exitRightSpriteRenderer;
        
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
        private ExitPosition _exitPosition;
        private bool _isReady;
        private bool _isItemUsed;

        public bool IsItemUsed => _isItemUsed;

        public RoomState State => _state;

        public List<Room> ConnectedRooms => connectedRooms;

        public List<RoomConnector> RoomConnectors => roomConnectors;

        public ItemType ItemType => itemType;

        public bool IsAlarmable => _isAlarmable;

        public bool IsAlarmOn => _isAlarmOn;

        public bool IsMonsterInRoom => _isMonsterInRoom;

        public bool IsPlayerInRoom => _isPlayerInRoom;

        public bool HasExit => _hasExit;

        public ExitPosition ExitPosition => _exitPosition;

        public bool IsReady => _isReady;

        public Transform PlayerPoint => playerPoint;


        public void Configure(ItemType item, bool isAlarmable, ExitPosition exitPosition)
        {
            SetItem(item);
            SetRoomName();
            _isAlarmable = isAlarmable;
            _exitPosition = exitPosition;
            _hasExit = exitPosition != ExitPosition.None;
            if (_hasExit)
            {
            }
            _isReady = true;
        }

        private void SetRoomName()
        {
            var randomNumber = UnityEngine.Random.Range(1, 10);
            var randomName = (RoomType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(RoomType)).Length);
            roomNameLabel.text = randomName.ToString() + randomNumber.ToString();
        }

        public void Interact()
        {
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

        private void SetItem(ItemType item)
        {
            itemType = item;
            switch (itemType)
            {
                case ItemType.None:
                    break;
                case ItemType.Health:
                    itemSpriteRenderer.sprite = healthSprite;
                    itemIndicatorSpriteRenderer.enabled = true;
                    break;
                case ItemType.Key:
                    itemSpriteRenderer.sprite = keySprite;
                    itemIndicatorSpriteRenderer.enabled = true;
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

        public void SetMonsterInRoom(bool isAlwaysVisible = false)
        {
            if (_isMonsterInRoom) return;
            _isMonsterInRoom = true;
            monsterSpriteRenderer.enabled =  isAlwaysVisible;
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

        public void SetPlayerInRoom()
        {
            if (_isPlayerInRoom) return;
            _isPlayerInRoom = true;
            EventHolder.RaisePlayerEnterRoom(this); 
            EventHolder.RaisePlayerAction();
            playerPoint.gameObject.SetActive(true);
            // roomHiddenSpriteRenderer.enabled = false;
            itemIndicatorSpriteRenderer.enabled = false; //TODO: REMOVE
        }

        public void RemovePlayer()
        {
            if (!_isPlayerInRoom) return;
            _isPlayerInRoom = false;
            EventHolder.RaisePlayerExitRoom(this);
            //EventHolder.RaisePlayerAction();
            playerPoint.gameObject.SetActive(false);
        }

        public void VisitRoomConnector(Room targetRoom)
        {
            // ищем коридор участвующий в переходе и помечаем его 
            foreach (RoomConnector roomConnector in roomConnectors.Where(roomConnector => roomConnector.TargetRoom == targetRoom || roomConnector.OriginRoom == targetRoom))
                roomConnector.Visit();
        }

        private void SetRoomVisible()
        {
            if (_state == RoomState.Hidden)
            {
                _state = RoomState.Visible;
                EventHolder.RaisePlayerAction();
                if (_isAlarmable) SetAlarmOn();
                roomSpriteRenderer.sprite = roomDefaultSprite;
                itemIndicatorSpriteRenderer.enabled = false;
                if (_hasExit)
                    switch (_exitPosition)
                    {
                        case ExitPosition.None: 
                            break;
                        case ExitPosition.Up: 
                            exitUpSpriteRenderer.enabled = true; 
                            break;
                        case ExitPosition.Down: 
                            exitDownSpriteRenderer.enabled = true; 
                            break;
                        case ExitPosition.Left: 
                            exitLeftSpriteRenderer.enabled = true; 
                            break;
                        case ExitPosition.Right: 
                            exitRightSpriteRenderer.enabled = true; 
                            break;
                        default: 
                            throw new ArgumentOutOfRangeException();
                    }
                
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
            if (_isItemUsed) return;
            Debug.Log("USED " + itemType);
            switch (itemType)
            {
                case ItemType.None:
                    break;
                case ItemType.Health:
                    UseHealth();
                    break;
                case ItemType.Key:
                    AddKey();
                    break;
                case ItemType.Lamp:
                    UseLamp();
                    break;
                case ItemType.Terminal:
                    UseTerminal();
                    break;
                default:
                    break;
            }
            _isItemUsed = true;
            EventHolder.RaisePlayerItemInteract(itemType);
        }

        private void UseLamp()
        {
            _state = RoomState.Lighted;
            lampSpriteRenderer.sprite = lightOnSprite;
            SoundManager.Instance.CreateSoundBuilder().Play(SoundDataLibrary.Instance.Lamp);
            EventHolder.RaisePlayerAction();
        }

        private void UseTerminal()
        {
            SoundManager.Instance.CreateSoundBuilder().Play(SoundDataLibrary.Instance.Terminal);
            EventHolder.RaisePlayerItemInteract(ItemType.Terminal);
            EventHolder.RaisePlayerAction();
            terminalSpriteRenderer.sprite = terminalOnSprite;
        }

        private void SetAlarmOn()
        {
            alarmSpriteRenderer.enabled = true;
            alarmSpriteRenderer.sprite = alarmOnSprite;
            _isAlarmOn = true;
            EventHolder.RaiseAlarmSetOn(this);
            SoundManager.Instance.CreateSoundBuilder().Play(SoundDataLibrary.Instance.Alarm);
        }

        private void AddKey()
        {
            SoundManager.Instance.CreateSoundBuilder().Play(SoundDataLibrary.Instance.Card);
            itemSpriteRenderer.enabled = false;
        }

        private void UseHealth()
        {
            SoundManager.Instance.CreateSoundBuilder().Play(SoundDataLibrary.Instance.HpPack);
            itemSpriteRenderer.enabled = false;
        }
        
    }
}