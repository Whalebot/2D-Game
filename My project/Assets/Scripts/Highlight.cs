using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Highlight : MonoBehaviour
{
    public bool toggle;
    //we assign all the renderers here through the inspector
    [SerializeField]
    private List<Renderer> renderers;
    public float intensity = 4;

    //helper list to cache all the materials ofd this object
    private List<Material> materials;

    //Gets all the materials from each renderer
    private void Awake()
    {
        Renderer[] temp = GetComponentsInChildren<Renderer>(true);
        foreach (var item in temp)
        {
            if (item.enabled) renderers.Add(item);

        }
        //renderers = GetComponentsInChildren<Renderer>(true).ToList();
        materials = new List<Material>();
        foreach (var renderer in renderers)
        {
            //A single child-object might have mutliple materials on it
            //that is why we need to all materials with "s"
            materials.AddRange(new List<Material>(renderer.materials));
        }
    }

    private void Start()
    {
        toggle = false;
    }

    [Button]
    public void ToggleHighlight()
    {
        toggle = !toggle;
        ToggleHighlight(toggle);
    }

    public void ToggleHighlight(bool val)
    {
        float factor = Mathf.Pow(2, intensity);

        if (val)
        {
            foreach (var material in materials)
            {
                Color c = material.GetColor("_MainColor");
                //We need to enable the EMISSION
                material.SetColor("_MainColor", new Color(c.r * factor, c.g * factor, c.b * factor));
            }
        }
        else
        {
            foreach (var material in materials)
            {
                Color c = material.GetColor("_MainColor");
                material.SetColor("_MainColor", new Color(c.r / factor, c.g / factor, c.b / factor));
            }
        }
    }
}