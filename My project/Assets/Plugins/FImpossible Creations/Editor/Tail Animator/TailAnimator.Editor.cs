using FIMSpace.FEditor;
using UnityEditor;
using UnityEngine;

namespace FIMSpace.FTail
{
    [CustomEditor(typeof(TailAnimator2))]
    [CanEditMultipleObjects]
    public partial class FTailAnimator2_Editor : Editor
    {

        public override void OnInspectorGUI()
        {
            Undo.RecordObject(target, "Spine Animator Inspector");

            serializedObject.Update();

            TailAnimator2 Get = (TailAnimator2)target;
            string title = drawDefaultInspector ? " Default Inspector" : (" " + Get._editor_Title);

            HeaderBoxMain(title, ref Get.DrawGizmos, ref drawDefaultInspector, _TexTailAnimIcon, Get, 27);

            if (drawDefaultInspector)
                DrawDefaultInspector();
            else
                DrawNewGUI();

            serializedObject.ApplyModifiedProperties();
        }


        void DrawNewGUI()
        {
            #region Preparations for unity versions and skin

            c = Color.Lerp(GUI.color * new Color(0.8f, 0.8f, 0.8f, 0.7f), GUI.color, Mathf.InverseLerp(0f, 0.15f, Get.TailAnimatorAmount));

            RectOffset zeroOff = new RectOffset(0, 0, 0, 0);
            float substr = .18f; float bgAlpha = 0.08f; if (EditorGUIUtility.isProSkin) { bgAlpha = 0.1f; substr = 0f; }

#if UNITY_2019_3_OR_NEWER
            int headerHeight = 22;
#else
            int headerHeight = 25;
#endif

            Get._editor_IsInspectorViewingSetup = drawSetup;
            Get._editor_IsInspectorViewingColliders = drawCollisions;
            Get._editor_IsInspectorViewingModules = drawAdditional;
            Get._editor_IsInspectorViewingShaping = drawShaping;
            Get._editor_IsInspectorViewingIncludedColliders = drawInclud;

            Get.RefreshTransformsList();

            #endregion


            GUILayout.BeginVertical(FGUI_Resources.BGBoxStyle); GUILayout.Space(1f);
            GUILayout.BeginVertical(FGUI_Inspector.Style(zeroOff, zeroOff, new Color(.7f - substr, .7f - substr, 0.7f - substr, bgAlpha), Vector4.one * 3, 3));

            FGUI_Inspector.HeaderBox(ref drawSetup, Lang("Main Setup"), true, FGUI_Resources.Tex_GearSetup, headerHeight, headerHeight - 1, LangBig());
            if (drawSetup) Tab_DrawSetup();

            GUILayout.EndVertical();

            // ------------------------------------------------------------------------

            GUILayout.BeginVertical(FGUI_Inspector.Style(zeroOff, zeroOff, new Color(.3f - substr, .4f - substr, 1f - substr, bgAlpha), Vector4.one * 3, 3));
            FGUI_Inspector.HeaderBox(ref drawTweaking, Lang("Tweak Animation"), true, FGUI_Resources.Tex_Sliders, headerHeight, headerHeight - 1, LangBig());

            if (drawTweaking) Tab_DrawTweaking();

            GUILayout.EndVertical();


            // ------------------------------------------------------------------------


            GUILayout.BeginVertical(FGUI_Inspector.Style(zeroOff, zeroOff, new Color(.4f - substr, 1f - substr, .8f - substr, bgAlpha * 0.6f), Vector4.one * 3, 3));
            FGUI_Inspector.HeaderBox(ref drawAdditional, Lang("Additional Modules"), true, FGUI_Resources.Tex_Module, headerHeight, headerHeight - 1, LangBig());

            if (drawAdditional) Tab_DrawAdditionalFeatures();

            GUILayout.EndVertical();

            // ------------------------------------------------------------------------

            GUILayout.BeginVertical(FGUI_Inspector.Style(zeroOff, zeroOff, new Color(1f - substr, .55f - substr, .55f - substr, bgAlpha * 0.5f), Vector4.one * 3, 3));
            FGUI_Inspector.HeaderBox(ref drawShaping, Lang("Additional Shaping"), true, FGUI_Resources.Tex_Repair, headerHeight, headerHeight - 1, LangBig());

            if (drawShaping) Tab_DrawShaping();

            GUILayout.EndVertical();

            GUILayout.Space(2f);
            GUILayout.EndVertical();
        }

    }
}
