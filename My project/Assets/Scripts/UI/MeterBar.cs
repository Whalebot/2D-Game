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
        meterText.text = "" + (status.Meter) + "/" + status.currentStats.maxMeter;

        if (status.currentStats.maxMeter > 0)
            meterBar.fillAmount = ((float)status.currentStats.currentMeter / (float)status.currentStats.maxMeter);

    }


    private void ExecuteFrame()
    {

        UpdateValues();
    }
}
