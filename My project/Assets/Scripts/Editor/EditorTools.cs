using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using Sirenix.OdinInspector.Editor;

public class EditorTools : OdinEditor
{
    private static Vector3 position;
    private static bool toUpdate;

    [MenuItem("Tools/MovePlayerToMouse %t")]
    static void MovePlayerToMousehortcutKey()
    {
        toUpdate = true;
    }

    static EditorTools()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }
    private static void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;
        if (toUpdate)
        {
            toUpdate = false;
            MovePlayerHere();
        }

        switch (e.type)
        {
            case EventType.KeyDown:
                {

                    if (e.keyCode == (KeyCode.T) && e.control)
                    {
                        MovePlayerHere();
                    }
                    break;
                }
        }
    }

    public static void MovePlayerHere()
    {
        toUpdate = false;
        RaycastHit hit;
        Vector2 mousePos = Event.current.mousePosition;
        mousePos.y = SceneView.currentDrawingSceneView.camera.pixelHeight - mousePos.y;
        Ray ray = SceneView.currentDrawingSceneView.camera.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out hit))
        {
            position = hit.point;
        }
        var newPos = new Vector3(position.x, position.y, 0);
        GameManager thisComp = GameObject.FindObjectOfType<GameManager>();
        thisComp.player.position = newPos;
    }

    // Add a menu item named "Do Something with a Shortcut Key" to MyMenu in the menu bar
    // and give it a shortcut (ctrl-g on Windows, cmd-g on macOS).
    [MenuItem("Tools/GroupObjects %g")]
    static void GroupObjects()
    {
        if (!Selection.activeTransform) return;
        var go = new GameObject(Selection.activeTransform.name + " Group");

        Undo.RegisterCreatedObjectUndo(go, "Group Selected");
        go.transform.SetParent(Selection.activeTransform.parent, false);
        go.transform.position = Selection.transforms[0].position;
        foreach (var transform in Selection.transforms) Undo.SetTransformParent(transform, go.transform, "Group Selected");
        Selection.activeGameObject = go;
    }

    new void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    new void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }
}
#endif