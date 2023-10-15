using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class InteractScript : MonoBehaviour
{
    public static InteractScript Instance { get; private set; }

    public Vector3 boxSize;
    public Vector3 offset;

    [TabGroup("Debug")] public InteractionType interactionType;
    [TabGroup("Debug")] public Interactable lastInteractable;

    [TabGroup("Components")] public Status status;
    [TabGroup("Components")] public LayerMask interactMask;
    [TabGroup("Components")] public Transform player;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        GameManager.Instance.advanceGameState += ExecuteFrame;
    }

    private void OnDisable()
    {
        GameManager.Instance.advanceGameState -= ExecuteFrame;
    }


    void ExecuteFrame()
    {
        if (status.currentState == Status.State.Neutral)
        {
            DetectCollisions();
        }
    }

    public bool CanInteract()
    {
        if (lastInteractable != null)
        {
            return lastInteractable.CanInteract();
        }
        return false;
    }
    void DetectCollisions()
    {
        //appraisalWindow.gameObject.SetActive(false);

        DefaultCollisionDetection();
    }
    void DefaultCollisionDetection()
    {
        if (lastInteractable != null)
        {
            lastInteractable.OnDeselect();
            lastInteractable = null;
        }

        UIManager.Instance.DisableButtonPrompts();
        Collider[] col = Physics.OverlapBox(transform.position + offset, boxSize * 0.5F, transform.rotation, interactMask);
        foreach (Collider item in col)
        {
            if (item.transform.IsChildOf(status.transform))
            {
                //Debug.Log("Found child of player");
                continue;
            }

            Interactable temp = item.GetComponentInParent<Interactable>();

            if (temp == null) continue;
            lastInteractable = temp;
            //appraisalWindow.transform.parent.position = new Vector3(lastInteractable.transform.position.x, appraisalWindow.transform.parent.transform.position.y, lastInteractable.transform.position.z);

            if (CanInteract())
            {
               UIManager.Instance.SetupButtonPrompt(temp);
            }
            else
            {
                interactionType = InteractionType.Generic;
            }
        }

        if (lastInteractable != null)
        {
            lastInteractable.OnHighlight();
        }
    }

    public bool CanNorth()
    {
        return lastInteractable && lastInteractable.CanNorth();
    }
    public void North()
    {
        lastInteractable.North();
    }
    public bool CanSouth()
    {
        return lastInteractable && lastInteractable.CanSouth();
    }
    public void South()
    {
        lastInteractable.South();
    }
}

public enum InteractionType
{
    None,
    Generic
}