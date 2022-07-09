using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DataHolder : MonoBehaviour
{
    public static DataHolder Instance;
    DataHolder()
    {
        Instance = this;
    }

    public List<Minion> allMinions = new List<Minion>();
    public Dictionary<string, Minion> minonPrefabDictionary;


    public List<MinionUpdateCost> minionAmountToLevelUp = new List<MinionUpdateCost>();

    public List<LootChestProperty> lootChestData = new List<LootChestProperty>();
    public Dictionary<string, LootChestProperty> lootChestDictionary;

    void Awake()
    {   
        minonPrefabDictionary = new Dictionary<string, Minion>();
        foreach (var item in allMinions)
        {
            minonPrefabDictionary.Add(item.minionKey, item);
        }

        lootChestDictionary = new Dictionary<string, LootChestProperty>();
        foreach (LootChestProperty item in lootChestData)
        {
            lootChestDictionary.Add(item.chestType.ToString(), item);
        }
    }

}

[System.Serializable]
public class MinionUpdateCost
{
    public int coinCost = 0;
    public int minionAmount = 0;
}