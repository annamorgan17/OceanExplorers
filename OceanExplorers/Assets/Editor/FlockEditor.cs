using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(FlockData))]
public class FlockEditor : Editor {
    
    FlockData instance;
    Editor gameObjectEditor;
    bool RGBMODE = false;
    public override void OnInspectorGUI() { 
        RGBMODE = EditorGUILayout.Toggle("PARTY MODE -> ", RGBMODE);
        base.OnInspectorGUI(); 

        EditorGUI.BeginChangeCheck();

        GameObject gameObject = ((FlockData)target).fishprefab;
        
        GUIStyle bgColour = new GUIStyle();
        Texture2D texture = EditorGUIUtility.whiteTexture;
        Color[] fillColorArray = texture.GetPixels();

        if (RGBMODE) { //rgb mode
            for (int i = 0; i < fillColorArray.Length; i++) 
                fillColorArray[i] = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)); 
            texture.SetPixels(fillColorArray);
            texture.Apply(); 
            bgColour.normal.background = texture;
        } else { // normal white texture
            for (int i = 0; i < fillColorArray.Length; i++) 
                fillColorArray[i] = Color.white; 
            texture.SetPixels(fillColorArray);
            texture.Apply(); 
            bgColour.normal.background = texture;
        }
         
        if (gameObject != null) { // stuff to display the model preview
            if (gameObjectEditor == null)
                gameObjectEditor = Editor.CreateEditor(gameObject); 
            gameObjectEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(256, 256), bgColour);
        }

    } 
}
