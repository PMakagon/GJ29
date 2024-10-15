using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace _Project.Develop.StunGames.GameJam29.Runtime.Audio
{
    [RequireComponent(typeof(UnityEngine.UI.Button))] 
    [DisallowMultipleComponent]
    public class UIButtonSoundEmitter : MonoBehaviour
    {
        [SerializeField] private List<SoundData> soundData = new List<SoundData>();
        private Button button;

        private void Awake()
        {
            if (soundData.Count == 0)
            {
                Debug.LogError("SoundData missing on " + gameObject.name);
            }
            button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            button.onClick.AddListener(Play);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(Play);
        }

        private void Play() => SoundManager.Instance.PlayUI(soundData[Random.Range(0, soundData.Count)]);
    }
}