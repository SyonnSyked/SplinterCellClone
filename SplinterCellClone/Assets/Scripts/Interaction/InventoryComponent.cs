using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu]

public class InventoryComponent : ScriptableObject
{
    public Dictionary<string, List<GameObject>> Inventory = new Dictionary<string, List<GameObject>>();

}
