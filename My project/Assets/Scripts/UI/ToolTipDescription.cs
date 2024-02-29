using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ToolTipDescription : MonoBehaviour
{
    public TextMeshProUGUI tagTitle;
    public TextMeshProUGUI tagDescription;

    public void SetupToolTip(string title, string description)
    {
        tagTitle.text = title;
        tagDescription.text = description;
    }
}
