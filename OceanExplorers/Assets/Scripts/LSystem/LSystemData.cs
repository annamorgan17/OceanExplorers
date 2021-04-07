using System.Collections;
using System.Collections.Generic;
using UnityEngine;  
[System.Serializable]
public struct dictValues {
    public char Key;
    public string Value;
    public dictValues(char Key, string Value) {
        this.Key = Key;
        this.Value = Value;
    }
    public override string ToString() {
        return Key.ToString() + " -> " + Value;
    }
}
/// <summary>
/// Data structure to store variables and keywords
/// </summary>
[System.Serializable]
public struct Param {
    public char Character;
    public float Value;
    public Param(char character, float value) {
        this.Character = character; 
        this.Value = value;
    }
}
[CreateAssetMenu()]
public class LSystemVisualData : ScriptableObject { 
    //redo to use based on height
    [Header("Colour")]
    public CustomGradient Colour;
    
    //Display
    [Header("Display settings")]
    public float angle = 25.000f;
    public bool rotate = true;
    public int RotationAngle = 90;

    //Branch generation settings
    [Header("Generation")]
    [Range(0, 10.0f)] public float length = 1.0f;   
    [Range(1, 10)] public int generations = 3;
    [Range(0, 1)] public float lengthVariance = 0f;
    [Range(0, 100)] public int ammendmentChance = 0;
    [Range(0f, 5f)] public int randomThickness = 0;
    [Range(0, 4)] public int pillarHeight = 0;

    [Header("Visual")]
    [Range(3, 20)] public int points = 8;
    public Vector2 radius;

    //streamline
    public GenerationType LeafGeneration = GenerationType.PrimitiveSphere;
    
    [Header("Formuli")]
    public string StartString = string.Empty;
    [HideInInspector] public List<Param> Variables = new List<Param>(); 
    [HideInInspector] public List<dictValues> dictionary = new List<dictValues>() { new dictValues('F', "F+[F+F]") };
    private string returnVariablesToString() {
        string Out = string.Empty;
        foreach (var item in Variables) {
            Out += item.Character + "," + item.Value;
        }
        return Out;
    }
    private string returnDictionaryToString() {
        string Out = string.Empty;
        foreach (var item in dictionary) {
            Out += item.Key + "," + item.Value;
        }
        return Out;
    }
    private void OnValidate() {
        if (radius.x <= 0) {
            radius = new Vector2(1, radius.y);
        }
        if (radius.y <= 0) {
            radius = new Vector2(radius.x, 1);
        }
    }
    /*
    public override string ToString() {
        string startingString;
        if (StartString == string.Empty) {
            startingString = "Empty";
        } else {
            startingString = StartString;
        }
        return Colour.GetString() + ":" + Colour.blendMode.ToString() + ":" + angle + ":" + rotate + ":" + RotationAngle + ":" + length + ":" + generations + ":" + lengthVariance +
            ":" + ammendmentChance + ":" + randomThickness + ":" + pillarHeight + ":" + points + ":" + radius + ":" + LeafGeneration.ToString() +
            ":" + startingString; // + ":" + returnVariablesToString() + ":" + returnDictionaryToString();
    }
    */
}
