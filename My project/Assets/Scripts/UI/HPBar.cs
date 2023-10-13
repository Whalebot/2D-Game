using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    public bool skyrimBar;
    public bool alwaysShowName;
    public bool alwaysShowHPBar;
    public bool isBoss = false;
    public bool disabled;
    [SerializeField] Status status;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI subTitleText;
    public TextMeshProUGUI HpText;

    public bool activated;


    public GameObject container;
    public GameObject nameContainer;

    public Image bar;
    public Image poiseBar;
    public Color poiseColor;
    public Color poiseBreakColor;

    AI AI;



    private void Start()
    {
        if (status == null)
        {
            status = GetComponentInParent<Status>();
            AI = GetComponentInParent<AI>();
        }
        if (AI != null) AI.detectEvent += DisplayInfo;


        if (container != null && !alwaysShowHPBar)
            container.SetActive(false);

        if (poiseBar != null && status.baseStats.poise <= 0)
            poiseBar.gameObject.SetActive(false);

        if (nameContainer != null && !alwaysShowName)
            nameContainer.SetActive(false);

        status.healthEvent += UpdateBar;
        status.deathEvent += DisableHPBar;
        UpdateBar();
        SetName();
    }

    private void Update()
    {
        if (disabled) return;

        if (!alwaysShowHPBar && GameManager.Instance.showHPBar)
            if (status.currentStats.currentHealth < status.currentStats.maxHealth)
                container.SetActive(true);

        if (nameContainer != null && alwaysShowName)
            nameContainer.SetActive(true);
        UpdateBar();
        // SetName();
    }

    void DisplayInfo()
    {
        if (!activated || !isBoss) return;
        activated = true;
        container.SetActive(true);
        nameContainer.SetActive(true);
    }

    void DisableHPBar()
    {
        disabled = true;
        if (container != null)
        {
            container.SetActive(false);
            nameContainer.SetActive(false);
            //ou to if (subTitleText != null) subTitleText.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if (status != null)
            status.healthEvent -= UpdateBar;
    }

    void SetName()
    {
        if (nameText == null || status.character == null) return;
        nameText.text = status.character.characterName;
        if (subTitleText != null) subTitleText.text = status.character.subTitle;
    }

    // Update is called once per frame
    void UpdateBar()
    {
        HpText.text = "" + status.currentStats.currentHealth + "/" + status.currentStats.maxHealth;
        if (!skyrimBar)
            bar.fillAmount = (float)status.currentStats.currentHealth / status.currentStats.maxHealth;
        else
        {
            bar.transform.localScale = new Vector3((float)status.currentStats.currentHealth / status.currentStats.maxHealth, 1, 1);

        }
        if (poiseBar != null && status.baseStats.poise > 0)
        {
            if (status.poiseBroken)
            {
                poiseBar.color = poiseBreakColor;
            }
            else
                poiseBar.color = poiseColor;
            poiseBar.transform.localScale = new Vector3((float)status.currentStats.poise / status.baseStats.poise, 1, 1);
        }
    }
}
