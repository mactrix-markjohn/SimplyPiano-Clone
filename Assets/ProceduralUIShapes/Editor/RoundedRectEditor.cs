using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoundedRect))]
public class RoundedRectEditor : Editor
{
    private static bool showBorder = false;
    private static bool showShadow = false;
    
    public override void OnInspectorGUI()
    {
        var roundedRect = (RoundedRect)target;
        
        if (!roundedRect.canvas.additionalShaderChannels.HasFlag(AdditionalCanvasShaderChannels.TexCoord1) ||
            !roundedRect.canvas.additionalShaderChannels.HasFlag(AdditionalCanvasShaderChannels.TexCoord2) ||
            !roundedRect.canvas.additionalShaderChannels.HasFlag(AdditionalCanvasShaderChannels.TexCoord3))
        {
            EditorGUILayout.HelpBox("Make sure that \"TexCoord1\", \"TexCoord2\" and \"TexCoord3\" is enabled in \"Additional Shader Channels\" on the Canvas", MessageType.Error, true);
        }


        roundedRect.color = EditorGUILayout.ColorField("Tint (Fill and border)", roundedRect.color);
        roundedRect.fillColor = EditorGUILayout.ColorField("Fill", roundedRect.fillColor);
        EditorGUIUtility.wideMode = true;
        //EditorStyles.objectField
        roundedRect.sprite = (Sprite)EditorGUILayout.ObjectField("Sprite", roundedRect.sprite, typeof(Sprite), false, GUILayout.Height(EditorGUIUtility.singleLineHeight));
        roundedRect.roundedValue = Mathf.Max(EditorGUILayout.FloatField("Corner radius", roundedRect.roundedValue), 0);
        roundedRect.fillStyle = (RoundedRect.FillStyle)EditorGUILayout.EnumPopup("Fill method", roundedRect.fillStyle);

        GUI.enabled = roundedRect.fillStyle != RoundedRect.FillStyle.None;
        roundedRect.fillAmount = EditorGUILayout.Slider("Fill amount", roundedRect.fillAmount, 0f, 1f);
        GUI.enabled = true;

        GUILayout.Space(10);

        BeginFoldOutGroup("Border", ref roundedRect.enableBorder, ref showBorder);
        if (showBorder)
        {
            roundedRect.borderColor = EditorGUILayout.ColorField("Border", roundedRect.borderColor);
            roundedRect.applySpriteToBorder = EditorGUILayout.Toggle("Apply sprite to border", roundedRect.applySpriteToBorder);
            roundedRect.borderThickness = Mathf.Max(EditorGUILayout.FloatField("Border thickness", roundedRect.borderThickness), 0);
        }
        EndFoldOutGroup();

        GUILayout.Space(5);

        BeginFoldOutGroup("Shadow/Glow", ref roundedRect.enableShadow, ref showShadow);
        if (showShadow)
        {
            roundedRect.shadowColor = EditorGUILayout.ColorField("Shadow", roundedRect.shadowColor);
            roundedRect.shadowBlur = Mathf.Max(EditorGUILayout.FloatField("Shadow blur", roundedRect.shadowBlur), 0);
            roundedRect.shadowOffset = EditorGUILayout.Vector2Field("Shadow offset", roundedRect.shadowOffset);
            roundedRect.manualShadowScale = EditorGUILayout.Toggle("Manual shadow scale", roundedRect.manualShadowScale);
            GUI.enabled = roundedRect.enableShadow && roundedRect.manualShadowScale;
            roundedRect.shadowScale = EditorGUILayout.FloatField("Shadow scale", roundedRect.shadowScale);
        }
        EndFoldOutGroup();

        roundedRect.SetAllDirty();
    }

    private void BeginFoldOutGroup(string name, ref bool enabled, ref bool foldOut)
    {
        var foldOutBodyStyle = EditorStyles.helpBox;
        foldOutBodyStyle.padding = new RectOffset(1, 1, 1, 0);
        GUILayout.BeginVertical(foldOutBodyStyle);
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUILayout.Space(10);
        foldOut = EditorGUILayout.Foldout(foldOut, name, true);
        GUILayout.FlexibleSpace();
        GUILayout.Label("Enable");
        enabled = EditorGUILayout.Toggle(enabled, GUILayout.Width(15));
        GUILayout.EndHorizontal();
        GUI.enabled = enabled;
    }

    private void EndFoldOutGroup()
    {
        GUI.enabled = true;
        GUILayout.EndVertical();
    }

    [MenuItem("GameObject/UI/RoundedRect", false, 10)]
    static void CreateRoundedRect(MenuCommand menuCommand)
    {
        // Create a custom game object
        GameObject go = new GameObject("RoundedRect");
        go.AddComponent<RoundedRect>();
        // Ensure it gets reparented if this was a context click (otherwise does nothing)
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }
}
