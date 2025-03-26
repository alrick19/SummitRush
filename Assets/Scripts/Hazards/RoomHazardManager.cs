using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Subscriber model
/// </summary>
public class RoomHazardManager : MonoBehaviour
{
    private List<IResettableHazard> hazardsInRoom = new List<IResettableHazard>();

    public void Register(IResettableHazard hazard)
    {
        if (!hazardsInRoom.Contains(hazard))
            hazardsInRoom.Add(hazard);
    }

    public void ResetAllHazards()
    {
        foreach (var hazard in hazardsInRoom)
        {
            hazard.ResetHazard();
        }
    }
}
