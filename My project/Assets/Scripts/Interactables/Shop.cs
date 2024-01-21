using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : Interactable
{
    public Rank rank;
    public GameObject shopContainer;

    public override void Start()
    {
        base.Start();
    }


    public override void South()
    {
        base.South();
        OpenShop();
    }
    public void OpenShop()
    {
  
        GameManager.Instance.OpenShopWindow();
        SkillManager.Instance.RollShop(rank);
        Destroy(gameObject);
    }
}
