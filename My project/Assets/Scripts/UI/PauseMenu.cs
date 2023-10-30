using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public MapGenerator mapGenerator;
    public StatDisplay statWindow;
    private void OnEnable()
    {
        mapGenerator.SetupNodes();
        MarkVisitedRooms();
    }
    void MarkVisitedRooms()
    {

        for (int i = 0; i < mapGenerator.mapNodes.Count; i++)
        {
            foreach (var item in SaveManager.Instance.saveData.currrentCharacter.visitedRooms)
            {
                if (i == item)
                    mapGenerator.mapNodes[i].Visited();
            }

        }

    }
}
