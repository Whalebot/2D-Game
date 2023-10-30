using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class MapNode : MonoBehaviour
{
    public int x, y;
    public RoomTypes roomType;
    public Image img;
    public GameObject visitedIcon;
    public TextMeshProUGUI roomName;
    public List<MapNode> entryRooms;
    public List<MapNode> exitRooms;
    public Sprite normalSprite, eliteSprite, bossSprite, treasureSprite, shopSprite, eventSprite;
    public void SetupNode(RoomTypes room)
    {
        roomName.text = "" + room;
        roomType = room;
        switch (room)
        {
            case RoomTypes.Normal:
                img.sprite = normalSprite;
                break;
            case RoomTypes.Elite:
                img.sprite = eliteSprite;
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
            case RoomTypes.Disabled:
                break;
            default:
                break;
        }
    }
    public void Visited() {
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
