using UnityEngine;

namespace _Project.Develop.StunGames.GameJam29.Runtime
{
    public class PlayerSpawner : MonoBehaviour
    {
        [SerializeField] private PlayerView playerViewPrefab;
        private PlayerView _currentPlayerView;

        public PlayerView SpawnPlayer()
        {
            _currentPlayerView = Instantiate(playerViewPrefab);
            return _currentPlayerView;
        }
    }
}