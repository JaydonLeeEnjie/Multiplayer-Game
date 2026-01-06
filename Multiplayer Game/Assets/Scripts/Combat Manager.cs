using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public List<CombatEntity> Entities;
    public List<CombatEntity> TurnOrder;

    public void CalculateTurnOrder()
    {
        TurnOrder.Clear();
        TurnOrder = new List<CombatEntity>(Entities);

        TurnOrder.Sort((a, b) =>
        {
            float speedA = a?.stats?.CurrentSpeed ?? 0f;
            float speedB = b?.stats?.CurrentSpeed ?? 0f;
            return speedB.CompareTo(speedA);
        });
    }
}
