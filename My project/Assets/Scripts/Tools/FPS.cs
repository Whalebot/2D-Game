using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS : MonoBehaviour
{
    public int target = 60;
    public bool limitFrameRate;
    public static float frame = 0.0f;

    // Use this for initialization
    void Start()
    {
        QualitySettings.vSyncCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
   
            frame += (Time.unscaledDeltaTime - frame) * 0.1f;
        if (limitFrameRate)
        {
            if (Application.targetFrameRate != target)
            {
                Application.targetFrameRate = target;
            }
        }
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
        float msec = frame * 1000.0f;
        float fps = 1.0f / frame;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);
    }
}
