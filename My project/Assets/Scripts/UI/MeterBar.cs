using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MeterBar : MonoBehaviour
{
    Status status;
    public TextMeshProUGUI meterText;
    public Image meterBar;
    private void Start()
    {
        GameManager.Instance.advanceGameState += ExecuteFrame;

        if (status == null)
        {
            status = GameManager.Instance.player.GetComponent<Status>();
        }

        UpdateValues();
    }
    void UpdateValues()
    {
        meterText.text = "" + (int)(status.Meter / 100);

        if (status.currentStats.maxMeter > 0)
            meterBar.fillAmount = ((float)(status.currentStats.currentMeter % (status.currentStats.maxMeter + 1f)) / status.currentStats.maxMeter);

    }


    private void ExecuteFrame()
    {

        UpdateValues();
    }
}
