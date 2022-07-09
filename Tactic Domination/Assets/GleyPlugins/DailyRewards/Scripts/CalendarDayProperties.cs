namespace GleyDailyRewards
{
    using UnityEngine;


    public enum RewardType {Coin, Gem, Minion, Chest_Common, Chest_Rare, Chest_Epic, Chest_Legendary}
    /// <summary>
    /// Properties of the day prefab
    /// </summary>
    [System.Serializable]
    public class CalendarDayProperties
    {
        public RewardType rewardType;
        public string key;
        public int rewardValue;
    }
}