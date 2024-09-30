using TMPro;
using UnityEngine;

namespace _Project.Develop.StunGames.GameJam29.Runtime
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Player : MonoBehaviour
    {
        [SerializeField] private TextMeshPro healthLabel;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            Hide();
            EventHolder.OnHealthChanged += OnHealthChanged;
        }

        private void OnDestroy()
        {
            EventHolder.OnHealthChanged -= OnHealthChanged;
        }

        private void OnHealthChanged(int amount)
        {
            healthLabel.text = amount.ToString();
        }

        public void Show()
        {
            healthLabel.gameObject.SetActive(true);
            spriteRenderer.enabled = true;
            healthLabel.text = MatchController.Instance.CurrentHealth.ToString();
        }

        public void Hide()
        {
            healthLabel.gameObject.SetActive(false);
            spriteRenderer.enabled = false;
        }
    }
}