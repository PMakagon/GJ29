using TMPro;
using UnityEngine;
using VContainer;

namespace _Project.Develop.StunGames.GameJam29.Runtime
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] private TextMeshPro healthLabel;
        [SerializeField] private SpriteRenderer spriteRenderer;
        private MatchController _matchController;
        
        
        [Inject]
        private void Construct(MatchController matchController)
        {
            _matchController = matchController;
        }
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
            healthLabel.text = MatchController.CurrentHealth.ToString();
        }

        public void Hide()
        {
            healthLabel.gameObject.SetActive(false);
            spriteRenderer.enabled = false;
        }
    }
}