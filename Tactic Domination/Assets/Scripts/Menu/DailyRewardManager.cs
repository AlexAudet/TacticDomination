using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DailyRewardManager : MonoBehaviour
{
    int currentRewardValue;

    private void Start()
    {
        GleyDailyRewards.Calendar.AddClickListener(CalendarButtonClicked);
    }

    public void CalendarButtonClicked(int dayNumber, int rewardValue, GleyDailyRewards.RewardType type, string Key)
    {
        Debug.Log("Click : Day " + dayNumber + " / " + type.ToString() + " " + rewardValue);
        currentRewardValue = rewardValue;

        switch (type)
        {
            case GleyDailyRewards.RewardType.Coin:
                PlayFabManager.Instance.AddCoin(rewardValue);
                break;
            case GleyDailyRewards.RewardType.Gem:
                PlayFabManager.Instance.AddGem(rewardValue);
                break;
            case GleyDailyRewards.RewardType.Minion:

                break;
            default:
                break;
        }
     
    }

    [Button]
    public void ShowCalendar(bool reset = false)
    {
        GleyDailyRewards.Calendar.Show(reset);
    }
}
