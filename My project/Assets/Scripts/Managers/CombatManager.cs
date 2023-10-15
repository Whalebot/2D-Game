using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }
    public Attack lvl1;
    public Attack lvl2;
    public Attack lvl3;

    private void Awake()
    {
        Instance = this;
    }
}
