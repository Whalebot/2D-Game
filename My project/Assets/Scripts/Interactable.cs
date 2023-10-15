using UnityEngine;

public class Interactable : MonoBehaviour
{
    Highlight highlight;

    public virtual void Start()
    {

    }

    public enum Type
    {
        Enter, None
    }
    public Type _southType;
    public virtual Type southType
    {
        get { return _southType; }
        set { _southType = value; }
    }
    public Type westType = Type.None;
    public Type northType = Type.None;
    public Type eastType = Type.None;

    public virtual void OnHighlight()
    {
        if (highlight != null)
            highlight.ToggleHighlight(true);
    }
    public virtual void OnDeselect()
    {
        if (highlight != null)
            highlight.ToggleHighlight(false);
    }

    public virtual void Inspect()
    {

    }

    public virtual bool CanInteract()
    {
        return CanSouth() || CanEast() || CanNorth() || CanWest();
    }

    public virtual bool CanSouth()
    {
        return true;
    }

    public virtual void South()
    {

    }

    public virtual bool CanEast()
    {
        return false;
    }

    public virtual void East()
    {

    }

    public virtual bool CanNorth()
    {
        return false;
    }

    public virtual void North()
    {

    }

    public virtual bool CanWest()
    {
        return false;
    }

    public virtual void West()
    {

    }
}
