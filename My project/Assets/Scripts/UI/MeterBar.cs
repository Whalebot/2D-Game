using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MeterBar : MonoBehaviour
{
    Status status;
    RectTransform rect;
    public TextMeshProUGUI meterText;
    public Image meterBar;
    float startWidth;
    private void Start()
    {
        rect = GetComponent<RectTransform>();
        startWidth = rect.rect.width;
        GameManager.Instance.advanceGameState += ExecuteFrame;

        if (status == null)
        {
            status = GameManager.Instance.player.GetComponent<Status>();
        }

        UpdateValues();
    }
    void UpdateValues()
    {
        float size = Mathf.Clamp(
          (startWidth * 0.75f) + (startWidth * 0.25f * (status.currentStats.maxMeter / 100f))
          , 250f, 900f);
        rect.sizeDelta = new Vector2(size, rect.sizeDelta.y);

        meterText.text = "" + (status.Meter) + "/" + status.currentStats.maxMeter;

        if (status.currentStats.maxMeter > 0)
            meterBar.fillAmount = ((float)status.currentStats.currentMeter / (float)status.currentStats.maxMeter);

    }


    private void ExecuteFrame()
    {

        UpdateValues();
    }
}
