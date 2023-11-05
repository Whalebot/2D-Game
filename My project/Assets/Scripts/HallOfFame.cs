using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallOfFame : MonoBehaviour
{
    public GameObject visualPrefab;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < SaveManager.Instance.saveData.oldCharacters.Count; i++)
        {
            GameObject GO = Instantiate(visualPrefab, transform.position + Vector3.right * i, Quaternion.Euler(0, 180, 0), transform);
            GO.GetComponent<CharacterVisuals>().visualData = SaveManager.Instance.saveData.oldCharacters[i].visualData;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
