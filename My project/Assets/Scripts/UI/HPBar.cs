using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    public int frameCounter;
    public int delayTime;
    public bool skyrimBar;
    public bool alwaysShowName;
    public bool alwaysShowHPBar;
    public bool isBoss = false;
    public bool disabled;
    public bool player;
    [SerializeField] public Status status;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI subTitleText;
    public TextMeshProUGUI HpText;

    public bool activated;


    public GameObject container;
    public GameObject nameContainer;

    public Image healthBar;
    public Image delayHealthBar;
    public Image poiseBar;
    public Color poiseColor;
    public Color poiseBreakColor;

    RectTransform rect;
    float startWidth;

    private void Start()
    {
        GameManager.Instance.advanceGameState += ExecuteFrame;

        rect = GetComponent<RectTransform>();
        startWidth = rect.rect.width;

        if (status == null)
        {
            status = GetComponentInParent<Status>();
        }

        if (container != null && !alwaysShowHPBar)
        {
            container.SetActive(false);
            poiseBar.gameObject.SetActive(false);
        }

        if (poiseBar != null && status.baseStats.poise <= 0)
            poiseBar.gameObject.SetActive(false);

        if (nameContainer != null && !alwaysShowName)
            nameContainer.SetActive(false);

        status.healthEvent += UpdateBar;
        status.deathEvent += DisableHPBar;
        UpdateValues();
        delayHealthBar.transform.localScale = new Vector3((float)status.currentStats.currentHealth / status.currentStats.maxHealth, 1, 1);
        SetSize();
        SetName();
    }

    private void ExecuteFrame()
    {
        if (disabled) return;
        SetSize();

        if (!alwaysShowHPBar && GameManager.Instance.showHPBar)
        {
            if (status.currentStats.currentHealth < status.currentStats.maxHealth)
                container.SetActive(true);

            if (poiseBar != null && status.baseStats.poise > status.currentStats.poise)
                poiseBar.gameObject.SetActive(true);
        }

        if (nameContainer != null && alwaysShowName)
            nameContainer.SetActive(true);

        UpdateValues();

        if (frameCounter > 0)
        {
            frameCounter--;
            if (frameCounter <= 0) UpdateDelayBar();
        }
    }
    void SetSize()
    {
        if (player)
        {
            float size = Mathf.Clamp(
                (startWidth * 0.75f) + (startWidth * 0.25f * (status.currentStats.maxHealth / 50f))
                , 250f, 900f);
            rect.sizeDelta = new Vector2(size, rect.sizeDelta.y);
        }
    }
    void DisplayInfo()
    {
        if (!activated || !isBoss) return;
        activated = true;
        container.SetActive(true);
        nameContainer.SetActive(true);

        if (poiseBar != null && status.baseStats.poise > 0)
            poiseBar.gameObject.SetActive(true);
    }

    void DisableHPBar()
    {
        disabled = true;
        if (container != null)
        {
            container.SetActive(false);
        }
        if (nameContainer != null)
            nameContainer.SetActive(false);
    }

    private void OnDisable()
    {
        GameManager.Instance.advanceGameState -= ExecuteFrame;
        if (status != null)
            status.healthEvent -= UpdateBar;
    }

    void SetName()
    {
        if (nameText == null || status.character == null) return;
        nameText.text = status.character.characterName;
        if (subTitleText != null) subTitleText.text = status.character.subTitle;
    }

    void UpdateDelayBar()
    {
        if (!skyrimBar)
            delayHealthBar.fillAmount = (float)status.currentStats.currentHealth / status.currentStats.maxHealth;
        else
        {
            delayHealthBar.transform.localScale = new Vector3((float)status.currentStats.currentHealth / status.currentStats.maxHealth, 1, 1);

        }
    }

    void UpdateValues()
    {
        HpText.text = "" + status.currentStats.currentHealth + "/" + status.currentStats.maxHealth;

        if (!skyrimBar)
            healthBar.fillAmount = (float)status.currentStats.currentHealth / status.currentStats.maxHealth;
        else
        {
            healthBar.transform.localScale = new Vector3((float)status.currentStats.currentHealth / status.currentStats.maxHealth, 1, 1);
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

    void UpdateBar()
    {
        frameCounter = delayTime;
        UpdateValues();
    }

}
