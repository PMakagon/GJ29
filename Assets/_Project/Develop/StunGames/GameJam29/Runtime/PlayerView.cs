﻿using _Project.Develop.StunGames.GameJam29.Runtime.Rooms;
using TMPro;
using UnityEngine;

namespace _Project.Develop.StunGames.GameJam29.Runtime
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] private TextMeshPro healthLabel;
        [SerializeField] private SpriteRenderer spriteRenderer;
        private void Awake()
        {
            Hide();
            EventHolder.onHealthChanged += HealthChanged;
        }
        
        public void SetHealth(int currentHp)
        {
            healthLabel.text = currentHp.ToString();
        }

        private void OnDestroy()
        {
            EventHolder.onHealthChanged -= HealthChanged;
        }

        private void HealthChanged(int amount)
        {
            healthLabel.text = amount<0 ? "0" : amount.ToString();
        }

        public void Show()
        {
            healthLabel.gameObject.SetActive(true);
            spriteRenderer.enabled = true;
        }

        public void Hide()
        {
            healthLabel.gameObject.SetActive(false);
            spriteRenderer.enabled = false;
        }

        public void MoveToRoom(Room newRoom)
        {
            transform.SetParent(newRoom.transform);
            transform.position = newRoom.PlayerPoint.position;
            newRoom.SetPlayerInRoom();
        }
    }
}