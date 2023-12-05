using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class MapNode : MonoBehaviour
{
    public int x, y;
    public RoomTypes roomType;
    public RewardType rewardType;

    public Image img;
    public Image rewardImg;
    public GameObject visitedIcon;
    public TextMeshProUGUI roomName;
    public List<MapNode> entryRooms;
    public List<MapNode> exitRooms;
    public Sprite normalSprite, eliteSprite, bossSprite, treasureSprite, shopSprite, eventSprite, restSprite;
    public Sprite goldRewardSprite, itemRewardSprite, skillRewardSprite, potentialRewardSprite;
    public void SetupNode(RoomTypes room)
    {
        roomName.text = "" + room;
        roomType = room;
        rewardImg.gameObject.SetActive(false);
        switch (room)
        {
            case RoomTypes.Normal:
                img.sprite = normalSprite;
                rewardImg.gameObject.SetActive(true);
                break;
            case RoomTypes.Elite:
                img.sprite = eliteSprite;
                rewardImg.gameObject.SetActive(true);
                break;
            case RoomTypes.Boss:
                img.sprite = bossSprite;
                break;
            case RoomTypes.Treasure:
                img.sprite = treasureSprite;
                break;
            case RoomTypes.Shop:
                img.sprite = shopSprite;
                break;
            case RoomTypes.Event:
                img.sprite = eventSprite;
                break;
            case RoomTypes.Rest:
                img.sprite = restSprite;
                break;
            case RoomTypes.Disabled:
                break;
            default:
                break;
        }
    }

    public void SetupReward(RewardType reward)
    {
        rewardType = reward;

        switch (rewardType)
        {
            case RewardType.Blessing:
                rewardImg.sprite = skillRewardSprite;
                break;
            case RewardType.Item:
                rewardImg.sprite = itemRewardSprite;
                break;
            case RewardType.Skill:
                rewardImg.sprite = potentialRewardSprite;
                break;
            case RewardType.Gold:
                rewardImg.sprite = goldRewardSprite;
                break;
            default:
                break;
        }
    }
    public void Visited()
    {
        visitedIcon.SetActive(true);
    }
    public MapNode GetExitNode(int i)
    {
        //Looping boss room until something new is made
        if (roomType == RoomTypes.Boss)
            return this;

        if (i > exitRooms.Count)
            return null;
        else return exitRooms[i - 1];
    }
}
