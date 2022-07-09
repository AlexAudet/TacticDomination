using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GleyDailyRewards;

public class LootChestSelector : MonoBehaviour
{
    public Button lootButton;
    public Transform lootChestHandler;
    [Space(20)]
    public Button commonLootButton;
    public Button rareLootButton;
    public Button epicLootButton;
    public Button legendaryLootButton;

    public void OpenLootSelector(List<LootChestProperty> allLootChest, TimerButtonScript lootScript)
    {
        foreach (LootChestProperty item in allLootChest)
        {
            Button newButton = null;
            if (item.chestAmount > 0)
            {
                newButton = Instantiate(lootButton, lootChestHandler);
                newButton.GetComponentInChildren<TextMeshProUGUI>().text = item.chestAmount.ToString();
                newButton.transform.gameObject.GetComponent<Image>().sprite = item.chestSprite;

                StartLoot(lootScript, item.chestType);
            }
        }
    }

    void StartLoot(TimerButtonScript lootButtonScript, ChestType type)
    {
        foreach (LootChestProperty item in ChestLootManager.instance.ChestData)
        {
            if(item.chestType == type)
            {
                item.chestAmount--;
                ChestLootManager.instance.AddChestToLootButton(lootButtonScript, item);
                break;
            }
        }
    }
}
