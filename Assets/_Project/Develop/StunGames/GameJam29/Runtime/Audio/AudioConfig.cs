using UnityEngine;

namespace _Project.Develop.StunGames.GameJam29.Runtime.Audio
{
    [CreateAssetMenu(menuName = "AudioConfig", fileName = "AudioConfig")]
    public class AudioConfig : ScriptableObject
    {
        public bool muteUI;
        public bool muteGame;
        public bool muteAll;
        
        public float volumeUI = 1f;
        public float volumeGame = 1f;
        public float volumeMaster = 1f;
    }
}