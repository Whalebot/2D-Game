using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MapGenerator : MonoBehaviour
{
    public int inEditorSeed;

    [TabGroup("Map Generation")] public Vector2 nodeSpacing;
    [TabGroup("Map Generation")] public Vector2 nodeNoise;
    [TabGroup("Map Generation")] public int roomsPerFloor = 5;
    [TabGroup("Map Generation")] public int roomChoices = 3;
    [TabGroup("Map Generation")] public int treasureChance = 10;
    [TabGroup("Map Generation")] public int restChance = 10;
    [TabGroup("Map Generation")] public int shopChance = 20;
    [TabGroup("Map Generation")] public int eliteChance = 20;
    [TabGroup("Map Generation")] public int eventChance = 40;

    [TabGroup("Reward Generation")] public int potentialChance = 25;
    [TabGroup("Reward Generation")] public int goldChance = 25;
    [TabGroup("Reward Generation")] public int itemChance = 25;

    RectTransform rectTransform;
    [TabGroup("Settings & Components")] public Vector3 scrollPosition;
    [TabGroup("Settings & Components")] public Vector3 clampPosition;
    [TabGroup("Settings & Components")] public Transform mapContainer;
    Vector2 startPosition;
    [TabGroup("Settings & Components")] public GameObject mapNodePrefab;
    [TabGroup("Settings & Components")] public GameObject mapLinePrefab;
    [TabGroup("Settings & Components")] public MapNode startNode;
    [TabGroup("Settings & Components")] public List<MapNode> mapNodes;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        if (LevelManager.Instance != null)
        {
            scrollPosition = -mapContainer.GetComponent<RectTransform>().anchoredPosition - LevelManager.Instance.currentMapNode.GetComponent<RectTransform>().anchoredPosition;
            rectTransform.anchoredPosition = mapContainer.GetComponent<RectTransform>().anchoredPosition - LevelManager.Instance.currentMapNode.GetComponent<RectTransform>().anchoredPosition;

            scrollPosition.y = Mathf.Clamp(scrollPosition.y, -clampPosition.y, clampPosition.y);
            rectTransform.anchoredPosition = new Vector3(0, scrollPosition.y, 0);
        }

    }

    private void FixedUpdate()
    {
        scrollPosition.y = Mathf.Clamp(scrollPosition.y, -clampPosition.y, clampPosition.y);
        rectTransform.anchoredPosition = new Vector3(0, scrollPosition.y, 0);
    }

    [Button]
    public void RandomizeMap()
    {
        inEditorSeed = Random.Range(0, 1000000);

        SetupNodes();
    }

    [Button]
    public void SetupNodes()
    {
        mapNodes.Clear();

        for (var i = mapContainer.transform.childCount - 1; i >= 0; i--)
        {
            if (!Application.isPlaying)
                DestroyImmediate(mapContainer.transform.GetChild(i).gameObject);
            else
                Destroy(mapContainer.transform.GetChild(i).gameObject);

        }

        if (Application.isEditor && !Application.isPlaying)
            Random.InitState(inEditorSeed);
        else
            Random.InitState(SaveManager.Instance.seed);

        //Entry Node
        GameObject entryGO = Instantiate(mapNodePrefab, mapContainer.transform.position, Quaternion.identity, mapContainer.transform);
        entryGO.transform.localPosition = Vector3.zero;
        entryGO.name = "0-1 Entrance";
        startNode = entryGO.GetComponent<MapNode>();
        mapNodes.Add(startNode);
        startNode.roomType = RoomTypes.Staircase;
        startNode.Visited();

        GameObject bossGO = Instantiate(mapNodePrefab, mapContainer.transform.position + new Vector3(0, nodeSpacing.y * roomsPerFloor), Quaternion.identity, mapContainer.transform);
        bossGO.transform.localPosition = new Vector3(0, nodeSpacing.y * roomsPerFloor);
        bossGO.name = $"{roomsPerFloor}-{1} Boss";
        MapNode bossNode = bossGO.GetComponent<MapNode>();
        bossNode.SetupNode(RoomTypes.Boss);
        mapNodes.Add(bossNode);

        int previousFloorCount = roomsPerFloor;

        for (int i = roomsPerFloor - 1; i > 0; i--)
        {
            int randomRoomChoices = Random.Range(3, Mathf.Clamp(roomChoices + 1, 3, previousFloorCount + 2));
            if (i == 1
                //|| i == roomsPerFloor - 1
                )
            {
                randomRoomChoices = Random.Range(2, 4);
            }

            for (int j = 0; j < randomRoomChoices; j++)
            {
                float pos = j - (float)randomRoomChoices / 2;
                if (randomRoomChoices % 2 == 0) pos += 0.5F;
                else pos += 0.5F;

                GameObject tempNode = Instantiate(mapNodePrefab, mapContainer.transform.position + new Vector3(nodeSpacing.x * pos, nodeSpacing.y * i), Quaternion.identity, mapContainer.transform);
                float x = Random.Range(-nodeNoise.x, nodeNoise.x);
                float y = Random.Range(-nodeNoise.y, nodeNoise.y);

                tempNode.transform.localPosition = new Vector3(nodeSpacing.x * pos + x, nodeSpacing.y * i + y);
                tempNode.name = $"{i}-{j}";
                MapNode node = tempNode.GetComponent<MapNode>();

                node.x = j;
                node.y = i;

                //Connect to nodes above
                foreach (var item in mapNodes)
                {
                    if (item.y == node.y + 1)
                    {
                        if (node.x == 0)
                        {
                            if (item.x == 0)
                                ConnectNodes(node, item);
                        }
                        else if (node.x == randomRoomChoices - 1)
                        {
                            if (item.x == previousFloorCount - 1)
                                ConnectNodes(node, item);
                        }
                        else
                        {

                        }
                    }
                }
                //Connect to nodes above
                foreach (var item in mapNodes)
                {
                    if (item.y == node.y + 1)
                    {
                        int highestX = 0;

                        foreach (var exit in item.exitRooms)
                        {
                            if (exit.y == node.y)
                                if (exit.x > highestX)
                                    highestX = exit.x;
                        }
                        //Auto connect straight nodes if RNG
                        if (node.x == item.x)
                        {
                            int multi = Random.Range(0, 2);
                            if (multi == 0)
                                ConnectNodes(node, item);
                        }
                    }
                }
                foreach (var item in mapNodes)
                {
                    if (item.y == node.y + 1)
                    {
                        float tempX = j - (float)randomRoomChoices / 2;
                        float oldX = item.x - (float)previousFloorCount / 2;
                        //If the node hasn't found anything and the range is smaller than 2
                        if (node.exitRooms.Count == 0 && Mathf.Abs(tempX - oldX) < 1)
                        {
                            int multi = Random.Range(0, 4);
                            if (multi == 0)
                                ConnectNodes(node, item);
                        }
                    }
                }
                foreach (var item in mapNodes)
                {
                    if (item.y == node.y + 1)
                    {
                        float tempX = j - (float)randomRoomChoices / 2;
                        float oldX = item.x - (float)previousFloorCount / 2;

                        // When reaching the last node, will auto connect to everything without an entry
                        if (node.x == randomRoomChoices - 1 && Mathf.Abs(tempX - oldX) < 1f)
                        {
                            if (item.entryRooms.Count == 0)
                                ConnectNodes(node, item);
                        }

                        if (node.exitRooms.Count == 0 && Mathf.Abs(tempX - oldX) < 1f)
                            ConnectNodes(node, item);
                    }
                }
                foreach (var item in mapNodes)
                {
                    if (item.y == node.y + 1)
                    {
                        float tempX = j - (float)randomRoomChoices / 2;
                        float oldX = item.x - (float)previousFloorCount / 2;

                        if (item.entryRooms.Count == 0 && Mathf.Abs(tempX - oldX) < 1f)
                            ConnectNodes(node, item);
                    }
                }

                mapNodes.Add(node);

                if (i == 1)
                    ConnectNodes(startNode, node);

                if (i == roomsPerFloor - 1)
                    ConnectNodes(node, bossNode);

                node.SetupNode(RollRoomType(node));
            }

            previousFloorCount = randomRoomChoices;
        }
        foreach (var item in mapNodes)
        {
            RollRewardType(item);
        }
    }

    public void ConnectNodes(MapNode from, MapNode to)
    {
        if (from.exitRooms.Contains(to)) return;
        from.exitRooms.Add(to);
        to.entryRooms.Add(from);

        GameObject tempNode = Instantiate(mapLinePrefab, Vector3.zero, Quaternion.identity, mapContainer.transform);
        tempNode.transform.localPosition = from.transform.localPosition + (to.transform.localPosition - from.transform.localPosition) / 2;
        tempNode.transform.localScale = new Vector3(1, Vector2.Distance(to.transform.localPosition, from.transform.localPosition) * 0.008f, 1);
        tempNode.transform.localRotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, (to.transform.localPosition - from.transform.localPosition).normalized));
    }
    void RollRewardType(MapNode node)
    {
        bool foundGold = false;
        foreach (var item in node.exitRooms)
        {
            if (item.rewardType == RewardType.Gold)
                foundGold = true;

            int RNG = UnityEngine.Random.Range(0, 100);

            if (item.roomType == RoomTypes.Elite)
            {
                if (RNG <= 50)
                {
                    item.SetupReward(RewardType.Skill);
                    continue;
                }

                item.SetupReward(RewardType.Item);
                continue;
            }

            if (RNG <= itemChance)
            {
                item.SetupReward(RewardType.Item);
                continue;
            }

            if (RNG <= potentialChance)
            {
                RNG = UnityEngine.Random.Range(0, 100);
                item.SetupReward(RewardType.Skill);
                continue;
            }

            RNG = UnityEngine.Random.Range(0, 100);
            if (RNG <= goldChance && !foundGold)
            {
                item.SetupReward(RewardType.Gold);
                continue;
            }

            item.SetupReward(RewardType.Blessing);

        }
    }

    RoomTypes RollRoomType(MapNode node)
    {
        bool foundShop = false;
        bool foundElite = false;
        bool foundRest = false;
        foreach (var item in node.exitRooms)
        {
            if (item.roomType == RoomTypes.Shop)
                foundShop = true;
            if (item.roomType == RoomTypes.Elite)
                foundElite = true;
            if (item.roomType == RoomTypes.Rest)
                foundRest = true;
        }

        //if (node.y == 9)
        //    return RoomTypes.Treasure;

        int RNG = UnityEngine.Random.Range(0, 100);
        if (RNG <= treasureChance)
            return RoomTypes.Treasure;

        if (node.y != 1 && !foundElite)
        {
            RNG = UnityEngine.Random.Range(0, 100);
            if (RNG <= eliteChance)
                return RoomTypes.Elite;
        }

        if (!foundShop)
        {
            RNG = UnityEngine.Random.Range(0, 100);
            if (RNG <= shopChance)
                return RoomTypes.Shop;
        }

        RNG = UnityEngine.Random.Range(0, 100);
        if (RNG <= eventChance)
            return RoomTypes.Event;

        if (node.y != 1 && !foundRest)
        {
            RNG = UnityEngine.Random.Range(0, 100);
            if (RNG <= restChance)
                return RoomTypes.Rest;
        }

        return RoomTypes.Normal;
    }
    RoomTypes RollRoomType()
    {
        int RNG = UnityEngine.Random.Range(0, 100);
        if (RNG <= treasureChance)
            return RoomTypes.Treasure;

        RNG = UnityEngine.Random.Range(0, 100);
        if (RNG <= eliteChance)
            return RoomTypes.Elite;

        RNG = UnityEngine.Random.Range(0, 100);
        if (RNG <= shopChance)
            return RoomTypes.Shop;

        RNG = UnityEngine.Random.Range(0, 100);
        if (RNG <= eventChance)
            return RoomTypes.Event;

        RNG = UnityEngine.Random.Range(0, 100);
        if (RNG <= restChance)
            return RoomTypes.Rest;

        return RoomTypes.Normal;
    }
}
