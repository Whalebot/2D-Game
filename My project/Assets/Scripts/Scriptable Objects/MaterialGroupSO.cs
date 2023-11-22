using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Material Group", menuName = "Material Group")]
public class MaterialGroupSO : ScriptableObject
{

    public List<Material> materials;
}
