using System.Collections.Generic;
using _Project.Develop.StunGames.GameJam29.Runtime.Utils;
using UnityEngine;
using UnityEngine.Serialization;


namespace _Project.Develop.StunGames.GameJam29.Runtime.Audio
{
    public class SoundDataLibrary : PersistentSingleton<SoundDataLibrary>
    {
        [Header("UI")]
        [SerializeField] private SoundData buttonPress;
        [SerializeField] private SoundData errorBeep;
        [Header("NOISE")]
        [SerializeField] private List<SoundData> metalSounds = new List<SoundData>();
        [SerializeField] private List<SoundData> metalSoundsDistant = new List<SoundData>();
        [SerializeField] private List<SoundData> metalSoundsAll = new List<SoundData>();
        
        [Header("AMBIENCE")]
        [SerializeField] private SoundData ambience1;

        [Header("PLAYER")]
        [SerializeField] private SoundData heartBeat1;
        [SerializeField] private SoundData heartBeat2;
        [SerializeField] private SoundData heartBeat3;
        [SerializeField] private SoundData heartBeat4;

        [Header("ROOMS")]
        [SerializeField] private SoundData enter;
        [SerializeField] private SoundData alarm;
        [SerializeField] private SoundData lamp;
        [SerializeField] private SoundData terminal;
        [SerializeField] private SoundData hpPack;
        [SerializeField] private SoundData card;
        [SerializeField] private SoundData exitOpen;
        [SerializeField] private SoundData exitClose;

        [Header("MONSTER")]
        [SerializeField] private List<SoundData> monsterSounds = new List<SoundData>();
        [SerializeField] private SoundData monsterDistant1;
        [SerializeField] private SoundData monsterDistant2;
        [SerializeField] private SoundData monsterClose;
        
        [SerializeField] private SoundData earRing;

        public SoundData ButtonPress => buttonPress;

        public SoundData ErrorBeep => errorBeep;

        public List<SoundData> MetalSounds => metalSounds;

        public List<SoundData> MetalSoundsDistant => metalSoundsDistant;

        public List<SoundData> MetalSoundsAll => metalSoundsAll;

        public SoundData Ambience1 => ambience1;

        public SoundData HeartBeat1 => heartBeat1;

        public SoundData HeartBeat2 => heartBeat2;

        public SoundData HeartBeat3 => heartBeat3;

        public SoundData HeartBeat4 => heartBeat4;

        public SoundData Enter => enter;

        public SoundData Alarm => alarm;

        public SoundData Lamp => lamp;

        public SoundData Terminal => terminal;

        public SoundData HpPack => hpPack;

        public SoundData Card => card;

        public SoundData ExitOpen => exitOpen;

        public SoundData ExitClose => exitClose;

        public List<SoundData> MonsterSounds => monsterSounds;

        public SoundData MonsterDistant1 => monsterDistant1;

        public SoundData MonsterDistant2 => monsterDistant2;

        public SoundData MonsterClose => monsterClose;

        public SoundData EarRing => earRing;
    }
}