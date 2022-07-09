using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "PlayerData")]
public class Player : ScriptableObject
{
    public MinionData[] minionsDeck = new MinionData[3];

    public List<MinionData> allMinions = new List<MinionData>();

}

