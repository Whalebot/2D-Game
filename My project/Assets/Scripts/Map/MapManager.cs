using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public MapGenerator firstAreaMap;
    public MapGenerator secondAreaMap;
    public MapGenerator thirdAreaMap;

    // Start is called before the first frame update
    void Start()
    {
        firstAreaMap.gameObject.SetActive(false);
        secondAreaMap.gameObject.SetActive(false);
        thirdAreaMap.gameObject.SetActive(false);

        switch (LevelManager.Instance.area)
        {
            case 1: firstAreaMap.gameObject.SetActive(true); break;
            case 2: secondAreaMap.gameObject.SetActive(true); break;
            case 3: thirdAreaMap.gameObject.SetActive(true); break;
            default:
                break;
        }
    }

    private void OnEnable()
    {
        
    }
}
