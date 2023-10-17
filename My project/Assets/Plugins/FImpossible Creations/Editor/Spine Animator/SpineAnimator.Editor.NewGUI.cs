using FIMSpace.FEditor;
using UnityEditor;
using UnityEngine;

public partial class FSpineAnimator_Editor
{
    void DrawNewGUI()
    {
        #region Preparations for unity versions and skin

        c = Color.Lerp(GUI.color * new Color(0.8f, 0.8f, 0.8f, 0.7f), GUI.color, Mathf.InverseLerp(0f, 0.15f, Get.SpineAnimatorAmount));

        RectOffset zeroOff = new RectOffset(0, 0, 0, 0);
        float bgAlpha = 0.05f; if (EditorGUIUtility.isProSkin) bgAlpha = 0.1f;

#if UNITY_2019_3_OR_NEWER
        int headerHeight = 22;
#else
        int headerHeight = 25;
#endif

        if (Get.SpineBones == null) Get.SpineBones = new System.Collections.Generic.List<FIMSpace.FSpine.FSpineAnimator.SpineBone>();

        #endregion

        GUILayout.BeginVertical(FGUI_Resources.BGBoxStyle); GUILayout.Space(1f);


        // ------------------------------------------------------------------------

        // If spine setup is not finished, then not drawing rest of the inspector
        if (Get.SpineBones.Count <= 1)
        {
            GUILayout.BeginVertical(FGUI_Inspector.Style(zeroOff, zeroOff, new Color(.7f, .7f, 0.7f, bgAlpha), Vector4.one * 3, 3));

            EditorGUILayout.BeginHorizontal(FGUI_Resources.HeaderBoxStyle);

            GUILayout.Label(new GUIContent(" "), GUILayout.Width(1));
            if (GUILayout.Button(new GUIContent(FGUI_Resources.Tex_GearSetup), EditorStyles.label, new GUILayoutOption[2] { GUILayout.Width(headerHeight), GUILayout.Height(headerHeight) })) { }
            if (GUILayout.Button(Lang("Prepare Spine Chain"), LangBig() ? FGUI_Resources.HeaderStyleBig : FGUI_Resources.HeaderStyle, GUILayout.Height(headerHeight))) { }
            if (GUILayout.Button(new GUIContent(FGUI_Resources.Tex_Repair), EditorStyles.label, new GUILayoutOption[2] { GUILayout.Width(headerHeight), GUILayout.Height(headerHeight) })) { }
            GUILayout.Label(new GUIContent(" "), GUILayout.Width(1));

            EditorGUILayout.EndHorizontal();

            Tab_DrawPreSetup();

            GUILayout.EndVertical();
            GUILayout.EndVertical();
            return;
        }


        GUILayout.BeginVertical(FGUI_Inspector.Style(zeroOff, zeroOff, new Color(.7f, .7f, 0.7f, bgAlpha), Vector4.one * 3, 3));

        FGUI_Inspector.HeaderBox(ref drawSetup, Lang("Character Setup"), true, FGUI_Resources.Tex_GearSetup, headerHeight, headerHeight - 1, LangBig());
        Get._editorSetupDrawing = drawSetup;
        if (drawSetup) Tab_DrawSetup();

        GUILayout.EndVertical();


        // ------------------------------------------------------------------------

        GUILayout.BeginVertical(FGUI_Inspector.Style(zeroOff, zeroOff, new Color(.3f, .4f, 1f, bgAlpha), Vector4.one * 3, 3));
        FGUI_Inspector.HeaderBox(ref drawTweaking, Lang("Tweak Animation"), true, FGUI_Resources.Tex_Sliders, headerHeight, headerHeight - 1, LangBig());

        if (drawTweaking) Tab_DrawTweaking();

        GUILayout.EndVertical();


        // ------------------------------------------------------------------------

        GUILayout.BeginVertical(FGUI_Inspector.Style(zeroOff, zeroOff, new Color(.825f, .825f, 0.225f, bgAlpha * 0.8f), Vector4.one * 3, 3));
        FGUI_Inspector.HeaderBox(ref drawCorrecting, Lang("Adjustements"), true, FGUI_Resources.Tex_Repair, headerHeight, headerHeight - 1, LangBig());

        if (drawCorrecting) Tab_DrawCorrections();

        GUILayout.EndVertical();


        // ------------------------------------------------------------------------

        GUILayout.BeginVertical(FGUI_Inspector.Style(zeroOff, zeroOff, new Color(.4f, 1f, .7f, bgAlpha), Vector4.one * 3, 3));
        FGUI_Inspector.HeaderBox(ref drawPhysics, Lang("Physical Parameters"), true, FGUI_Resources.Tex_Physics, headerHeight, headerHeight - 1, LangBig());

        Get._editorPhysicsDrawing = drawPhysics;
        if (drawPhysics) Tab_DrawPhysics();

        GUILayout.EndVertical();

        // ------------------------------------------------------------------------


        GUILayout.EndVertical();
    }
}