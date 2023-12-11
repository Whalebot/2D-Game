using UnityEngine;

[CreateAssetMenu(fileName = "New Moveset", menuName = "ScriptableObjects/Moveset")]
public class Moveset : ScriptableObject
{
    [Header("Dodge Button")]
    public Combo dodgeCombo;
    public Combo airDodgeCombo;
    public Combo dashAttackCombo;
    public Combo airDashAttackCombo;

    [Header("A Button")]
    public Combo lightCombo; 
    public Combo upLightCombo;
    public Combo downLightCombo;
    public Combo airLightCombo;
    public Combo downAirLightCombo;

    [Header("B Button")]
    public Combo heavyCombo;
    public Combo upHeavyCombo;
    public Combo downHeavyCombo;
    public Combo airHeavyCombo;
    public Combo downAirHeavyCombo;

    [Header("C Button")]
    public Combo skillCombo;
    public Combo upSkillCombo;
    public Combo downSkillCombo;
    public Combo sideSkillCombo;
    public Combo airSkillCombo;
    public Combo downAirSkillCombo;
}

