using UnityEngine;

[CreateAssetMenu(fileName = "New Moveset", menuName = "ScriptableObjects/Moveset")]
public class Moveset : ScriptableObject
{
    public Move dodge;
    public Move airDodge;
    public Move dashAttack;
    public Move airDashAttack;

    public Combo lightCombo;
    public Combo heavyCombo;
    public Combo airLightCombo;
    public Combo airHeavyCombo;
    public Combo[] combos;
    public Combo extra;
}

