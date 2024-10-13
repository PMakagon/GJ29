using UnityEngine;

namespace _Project.Develop.StunGames.GameJam29.Runtime
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [SerializeField] private int startHp = 17;
        [SerializeField] private int roomsOnLevel = 9;
        [SerializeField] private int alarmsOnLevel = 1; 
        [SerializeField] private int lampsOnLevel = 1;
        [SerializeField] private int terminalsCount = 1; 
        [SerializeField] private int hpPacksCount = 2;

        public int StartHp => startHp;

        public int RoomsOnLevel => roomsOnLevel;

        public int AlarmsOnLevel => alarmsOnLevel;

        public int LampsOnLevel => lampsOnLevel;

        public int TerminalsCount => terminalsCount;

        public int HpPacksCount => hpPacksCount;
    }
}