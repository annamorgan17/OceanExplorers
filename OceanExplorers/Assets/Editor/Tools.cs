using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
public class Tools{ 
    [MenuItem("GameObject/CreatePlant")] private static void CreatePlant() {
        if (EditorUtility.DisplayDialog("Plant Generator", "Do you want to create a plant?", "Create", "Cancel")) {
            int Index = 1;
            foreach (GameObject gameObj in GameObject.FindObjectsOfType<GameObject>()) {
                if (gameObj.name.Contains("Plant")) 
                    Index++; 
            } 
            GameObject gm = new GameObject("Plant " + Index);
            gm.AddComponent<Generation>();
        }
    }  
} 
[CustomEditor(typeof(Generation))] public class Interface : Editor {
    static Generation myScript;
    int[] _choiceIndex = new int[10]; 
    int _prevRotation = 0;
    int PreviousRuleLength = 1;
    int RuleLength = 1;
    string[] _choices = new[] { 

        //tree branching types
        "F+[+F-F-F]-[--F+F+F]", 
        "FF-[-F+F+F]+[+F-F-F]",
        "FF-[<-F+F+F>]+[<+F-F-F>]",
        "F[+FF][-FF]F[-F][+F]F",
        "F[+F]F[-F][F]",
         
        //traingle
        //120
        //F+F+F
        "F-F+F",

        //square sierpinski
        //90
        //F+XF+F+XF
        //X:
        "XF-F+F-XF+F+XF-F+F-X",

        //quadratic snowflake
        //F
        //90
        "F-F+F+F-F", 

        //board
        //F+F+F+F
        //90
        "FF+F+F+F+FF",

        //tiles
        //start F+F+F+F
        //90
        "FF+F-F+F+FF",

        //rings
        //start F+F+F+F
        // 90
        "FF+F+F+F+F+F-F",

        //levey
        //start F
        //45
        "-F++F-",

        //island curve
        //Start F-F-F-F
        //Angle 90
        //Rule F
        "F+f-FF+F+FF+Ff-f+FF-F-FF-Ff-FFF",
        //Rule f
        "ffffff",

        //Use same settings as island curve but onle rule with Rule F
        "F+FF-FF-F-F+F+FF-F-F+F+FF+FF-F",

        //Wiliam McWorters Pentadendrite
        //Start F-F-F-F-F
        //Angle 72
        //Iterations 4
        "F-F-F++F+F-F",

        //Dragon Curve
        //Start FA
        //Rule A
        "A+BF+",
        //Rule B
        "-FA-B",

        //Gosper Curve
        //Angle 60
        //Rule F
        "F-G--G+F++FF+G-",
        //Rule G
        "+F-GG--G-F++F+G",

        //Round Star
        //Angle 77
        //Iterations 7
        "F++F",

        //Ice fractal
        //Angle 90
        //Start F+F+F+F 
        "FF+F++F+F",

        //Koch curve
        //angle 80
        "F+F--F+F", 

        //Koch Snowflake
        //Angle 60
        //Start F--F--F
        "F+F--F+F",

        //Hilburt Curve
        //Angle 90
        //Rule L
        "+RF-LFL-FR+",
        //Rule R
        "-LF+RFR+FL-",

        "F-[[X]+X]+F[+FX]-X",
        "FF",

        "F(L,G)+[+F(L,G)-F(L,G)-F(L,G)]-[--F(L,G)+F(L,G)+F(L,G)]",
        "Custom String"}; // custom

void OnEnable() {
//EditorVariables stores data like choice index and previous rotation as this is lost after clicking on something else
//asign target is there is nothing
if (myScript == null) {
    myScript = (Generation)target;
}

//load values
_choiceIndex = myScript.EditorVariables._choiceIndex;
_prevRotation = myScript.EditorVariables._prevRotation;

//intiilise values if null
if (_choiceIndex == null) {
    _choiceIndex = new int[10];
    _prevRotation = 0;
}
}
private void OnDisable() {
//set the values when focus is lost
myScript.EditorVariables._choiceIndex = _choiceIndex;
myScript.EditorVariables._prevRotation = _prevRotation;
}
public override void OnInspectorGUI() { 
//Check if control was changed inside a block of code
EditorGUI.BeginChangeCheck();

#region Prefab Loading
//Filepath
string path = "Assets/Resources/Prefabs/";  
string fileExstension = "prefab";
//Button save
if (GUILayout.Button("Save to prefab")) {
    //save location
    string filepath = EditorUtility.SaveFilePanel("Select location to save Prefab", path, "", fileExstension); 
    //path exists
    if (filepath.Length != 0) {
        bool success;
        //save
        PrefabUtility.SaveAsPrefabAsset(myScript.gameObject, filepath, out success);

        //debug success
        if (success) {
            Debug.Log("Success in saving file to: " + filepath);
        } else {
            Debug.Log("Failed in saving file to: " + filepath);
        }
    } 
    //validate script
    Validate(myScript);
}
//Button load
if (GUILayout.Button("Load from prefab")) {
    //Choose file
    string filepath = EditorUtility.OpenFilePanel("Select Prefab to Open", path, fileExstension);
    //File exists
    if (filepath.Length != 0) {
        //Load
        GameObject gm = Instantiate<GameObject>(PrefabUtility.LoadPrefabContents(filepath));
        Debug.Log("Asset was found and loaded!");
        gm.transform.rotation = myScript.gameObject.transform.rotation;
        gm.name = myScript.gameObject.name;
        myScript.gameObject.AddComponent<Destroy>();
    } else {
        Debug.LogError("Failed loading prefab contents");
    } 
    //Validate script
    Validate(myScript); 
}
#endregion 

#region Colour 
//Toggle solid
EditorGUILayout.LabelField("Colour", EditorStyles.boldLabel);
myScript.SolidColour = EditorGUILayout.Toggle("SolidColour", myScript.SolidColour);
//Flip colour
if (GUILayout.Button("FlipColour")) {
    //set to opposite and revalidate
    myScript.FlipColour = !myScript.FlipColour;
    Validate(myScript);
}
//Solid Colour
if (myScript.SolidColour) {
    //display appropirate colour dependant on flip
    if (myScript.FlipColour)
        myScript.Col2 = EditorGUILayout.ColorField("Col2", myScript.Col2);
    else
        myScript.Col1 = EditorGUILayout.ColorField("Col1", myScript.Col1);
} else {
    //show colours in flipped order
    if (myScript.FlipColour) {
        myScript.Col2 = EditorGUILayout.ColorField("Col2", myScript.Col2);
        myScript.Col1 = EditorGUILayout.ColorField("Col1", myScript.Col1);
    } else {
        myScript.Col1 = EditorGUILayout.ColorField("Col1", myScript.Col1);
        myScript.Col2 = EditorGUILayout.ColorField("Col2", myScript.Col2);
    }
}
#endregion

#region Rotation
//Display rotate toggle
EditorGUILayout.LabelField("Rotation", EditorStyles.boldLabel);
myScript.rotate = EditorGUILayout.Toggle("rotate", myScript.rotate);

//rotation
if (!myScript.rotate) {
    EditorGUILayout.LabelField("Rotation Angle");
    myScript.RotationAngle = EditorGUILayout.IntSlider(myScript.RotationAngle, 0, 360);
}
#endregion

DrawDefaultInspector();

#region Dictionary String  
//Display rule length
EditorGUILayout.LabelField("Rules", EditorStyles.boldLabel);
myScript.RuleLength = EditorGUILayout.IntSlider(myScript.RuleLength, 1, 10);
//For each rule
for (int i = 0; i < myScript.RuleLength; i++) {
    //Display Rule
    EditorGUILayout.LabelField("Rule " + i);
    string Rule = EditorGUILayout.TextField("Rule", myScript.dictionaryChar[i].ToString());
    //Dont allow it to be blank
    if (Rule == "")
        Rule = "F";
    myScript.dictionaryChar[i] = Rule.ToCharArray()[0];
    //find index
    _choiceIndex[i] = EditorGUILayout.Popup(_choiceIndex[i], _choices);
    if (_choiceIndex[i] == _choices.Length - 1) {
        //custom string
        string String = EditorGUILayout.TextField("Rule", myScript.dictionaryString[i]);
        //dont allow to be empty
        if (String == "")
            String = "F";
        myScript.dictionaryString[i] = String;
    } else {
        //dropdown string option
        myScript.dictionaryString[i] = _choices[_choiceIndex[i]];
    }
}
#endregion


//validate
if (GUILayout.Button("Recalculate"))
    Validate(myScript);
//rotate
if (GUI.changed) 
    myScript.Rotation(); 
_prevRotation = myScript.RotationAngle;

}
//Call validation in the script
private void Validate(Generation myScript) { 
if (Application.isPlaying) { myScript.Validate = true; }
} 
} 