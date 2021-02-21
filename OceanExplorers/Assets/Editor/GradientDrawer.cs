using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomPropertyDrawer(typeof(CustomGradient))]
public class GradientDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        Event guiEvent = Event.current;
        CustomGradient gradient = (CustomGradient)fieldInfo.GetValue(property.serializedObject.targetObject); 
        Rect textureRect = new Rect(position.x, position.y, position.width, position.height); 
        if (guiEvent.type == EventType.Repaint) {  
            GUIStyle gradientStyle = new GUIStyle();
            gradientStyle.normal.background = gradient.GetTexture((int)position.width);
            GUI.Label(textureRect, GUIContent.none, gradientStyle);
        } else {
            if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0) {
                if (textureRect.Contains(guiEvent.mousePosition)) {
                    GradientEditor window = EditorWindow.GetWindow<GradientEditor>();
                    window.SetGradient(gradient);

                }
            }
        }
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) { 
        return base.GetPropertyHeight(property, label) + 150;
    }
}
public class GradientEditor : EditorWindow { 
    CustomGradient gradient;
    const int borderSize = 10;
    const float keyWidth = 10;
    const float keyHeight = 20;
    Rect gradientPreviewRect;
    Rect[] keyRects;
    bool mouseIsDownOverKey;
    int selectedKeyIndex;
    bool needRepaint;
    private void OnGUI() {
        Draw();
        HandleInput();
        if (needRepaint) {
            needRepaint = false;
            Repaint();
        }
    }
    void Draw() {
        Event guiEvent = Event.current;

        gradientPreviewRect = new Rect(borderSize, borderSize, position.width - borderSize * 2, 25);
        GUI.DrawTexture(gradientPreviewRect, gradient.GetTexture((int)gradientPreviewRect.width));

        keyRects = new Rect[gradient.NumKeys];

        for (int i = 0; i < gradient.NumKeys; i++) {
            CustomGradient.ColourKey key = gradient.GetKey(i);
            Rect keyRect = new Rect(gradientPreviewRect.x + gradientPreviewRect.width * key.Time - keyWidth / 2f,
                gradientPreviewRect.yMax + borderSize, keyWidth, keyHeight);
            if (i == selectedKeyIndex) {
                EditorGUI.DrawRect(new Rect(keyRect.x - 2, keyRect.y - 2, keyRect.width + 4, keyRect.height + 4), Color.black);
            }
            EditorGUI.DrawRect(keyRect, key.Colour);
            keyRects[i] = keyRect;
        }
        Rect settingsRect = new Rect(borderSize, keyRects[0].yMax + borderSize, position.width - borderSize * 2, position.height);
        //EditorGUI.DrawRect(settingsRect, Color.white);
        GUILayout.BeginArea(settingsRect);
        EditorGUI.BeginChangeCheck();
        Color newColour = EditorGUILayout.ColorField(gradient.GetKey(selectedKeyIndex).Colour);
        if (EditorGUI.EndChangeCheck()) {
            gradient.UpdateKeyColour(selectedKeyIndex, newColour);
        }
        gradient.blendMode = (CustomGradient.BlendMode)EditorGUILayout.EnumPopup("Blend mode", gradient.blendMode);
        gradient.randomColour = EditorGUILayout.Toggle("Randomize Colour", gradient.randomColour);
        GUILayout.EndArea();
    }
    void HandleInput() {
        Event guiEvent = Event.current;

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0) {
            for (int i = 0; i < keyRects.Length; i++) {
                if (keyRects[i].Contains(guiEvent.mousePosition)) {
                    mouseIsDownOverKey = true;
                    selectedKeyIndex = i;
                    needRepaint = true;
                    break;
                }
            }
            if (!mouseIsDownOverKey) {
                Color randomColour = new Color(Random.value, Random.value, Random.value);
                float keyTime = Mathf.InverseLerp(gradientPreviewRect.x, gradientPreviewRect.xMax, guiEvent.mousePosition.x);
                Color interpolatedColour = gradient.Evaluate(keyTime);
                selectedKeyIndex = gradient.AddKey((gradient.randomColour)? randomColour : interpolatedColour, keyTime);
                mouseIsDownOverKey = true;
                needRepaint = true;
            }
        }
        if (guiEvent.type == EventType.MouseUp && guiEvent.button == 0) {
            mouseIsDownOverKey = false;
        }
        if (mouseIsDownOverKey && guiEvent.type == EventType.MouseDrag && guiEvent.button == 0) {
            float keyTime = Mathf.InverseLerp(gradientPreviewRect.x, gradientPreviewRect.xMax, guiEvent.mousePosition.x);
            selectedKeyIndex = gradient.UpdateKeyTime(selectedKeyIndex, keyTime);
            needRepaint = true;
        }
        if (guiEvent.keyCode == KeyCode.Backspace && guiEvent.type == EventType.KeyDown) {
            gradient.RemoveKey(selectedKeyIndex);
            if (selectedKeyIndex >= gradient.NumKeys) {
                selectedKeyIndex--;
            }
            needRepaint = true;
        }
    }
    public void SetGradient(CustomGradient gradient) {
        this.gradient = gradient;
    }
    private void OnEnable() {
        titleContent.text = "Gradient Editor";
        position.Set(position.x, position.y, 400, 150);
        minSize = new Vector2(200,150);
        maxSize = new Vector2(1920, 150);
    }
    private void OnDisable() {
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
    }
}
