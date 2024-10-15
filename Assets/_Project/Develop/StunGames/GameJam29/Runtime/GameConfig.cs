using UnityEngine;

namespace _Project.Develop.StunGames.GameJam29.Runtime
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [SerializeField] private int startHp = 15;
        [Header("LEVEL SETTINGS")] 
        [SerializeField] private int roomsOnLevel = 10;
        [SerializeField] private int alarmsOnLevel = 1; 
        [SerializeField] private int lampsOnLevel = 1;
        [SerializeField] private int terminalsCount = 1; 
        [SerializeField] private int hpPacksCount =1;
        [SerializeField] private float roomConnectChance = 15f; // Вероятность соединения уже существующих комнат, от 0 до 100%
        [Header("ITEMS SETTINGS")] 
        [SerializeField] private int hpPacksHealthPoints =3;
        [Header("MONSTER SETTINGS")] 
        [SerializeField] private float monsterStayChance = 25f; // Вероятность стояния монстра на месте, от 0 до 100%
        [SerializeField] private int monsterMinSpawnDistance = 3; // Минимальное расстояние от спавна игрока до спавна монстра
        [Header("DEBUG")] 
        [SerializeField] public bool debugModeOn;
        [SerializeField] public bool drawLevelGizmos;
        [SerializeField] public bool alwaysShowMonster;
        [SerializeField] public bool alwaysShowItems;
        [SerializeField] public bool godMode;


        public int StartHp => startHp;

        public int RoomsOnLevel => roomsOnLevel;

        public int AlarmsOnLevel => alarmsOnLevel;

        public int LampsOnLevel => lampsOnLevel;

        public int TerminalsCount => terminalsCount;

        public int HpPacksCount => hpPacksCount;

        public int HpPacksHealthPoints => hpPacksHealthPoints;

        public float RoomConnectChance => roomConnectChance;

        public float MonsterStayChance => monsterStayChance;

        public int MonsterMinSpawnDistance => monsterMinSpawnDistance;
    }
}