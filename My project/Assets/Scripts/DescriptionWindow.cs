using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionWindow : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI quantityText;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI statText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI capacityText;
    public GameObject equipWindow;
    public GameObject freshnessWindow;
    public GameObject capacityWindow;

    public TextMeshProUGUI attackText;
    public Slider expSlider;
    // Start is called before the first frame update
    void Start()
    {

    }

    string CheckStringTags(SkillSO skill)
    {
        string s = skill.description;

        List<string> words = new List<string>();
        string test = "";

        //Divide words
        foreach (char letter in s.ToCharArray())
        {
            test += letter;

            if (letter == ' ')
            {
                words.Add(test);
                test = "";
            }
        }

        //Add final word
        words.Add(test);

        //Paint text
        for (int i = 0; i < words.Count; i++)
        {
            //foreach (string tag in blueTag.tags)
            {
                if (words[i].ToLower().Contains("value"))
                {
                    words[i] = "<color=#00ffffff>" + words[i] + "</color>";
                }
            }
        }
        string final = "";

        foreach (string word in words)
        {
            final += word;
        }
        return final;
    }


    string CheckStringTags(string s)
    {
        List<string> words = new List<string>();
        string test = "";

        //Divide words
        foreach (char letter in s.ToCharArray())
        {
            test += letter;

            if (letter == ' ' || letter == '\n')
            {
                words.Add(test);
                test = "";
            }
        }

        //Add final word
        words.Add(test);

        //Paint text
        for (int i = 0; i < words.Count; i++)
        {
            if (words[i].ToLower().Contains("stats"))
            {
                words[i] = "<color=#00ffffff>";

                words[i] += "</color>";
            }
        }
        string final = "";

        foreach (string word in words)
        {
            final += word;
        }
        return final;
    }


    string CheckStringTags( )
    {
        //string s = item.SO.description;
        //List<string> words = new List<string>();
        //string test = "";

        ////Divide words
        //foreach (char letter in s.ToCharArray())
        //{
        //    test += letter;

        //    if (letter == ' ' || letter == '\n')
        //    {
        //        words.Add(test);
        //        test = "";
        //    }
        //}

        ////Add final word
        //words.Add(test);

        ////Paint text
        //for (int i = 0; i < words.Count; i++)
        //{
        //    if (words[i].ToLower().Contains("stats"))
        //    {
        //        words[i] = "<color=#00ffffff>";
        //        foreach (var e in item.SO.eventScripts)
        //        {
        //            if (e.eventType == EventScript.EventType.Stats)
        //            {
        //                for (int j = 0; j < e.statTypes.Length; j++)
        //                {
        //                    words[i] += "" + e.statTypes[j].type + " +" + (int)(e.statTypes[j].value + e.statTypes[j].value * item.quality / 100F) + "\n";
        //                }
        //            }

        //        }

        //        words[i] += "</color>";
        //    }
        //}
        //string final = "";

        //foreach (string word in words)
        //{
        //    final += word;
        //}
        return "final";
    }

    public void DisableDisplay()
    {

    }
}
