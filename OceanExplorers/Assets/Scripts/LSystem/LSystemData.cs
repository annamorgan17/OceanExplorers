using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public struct EditorVar { public int[] _choiceIndex; public int _prevRotation; }
public enum Shape { PrimitiveCube, PrimitiveCylinder }
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
    [Range(1, 50)] public float thickness = 8;
    [Range(1, 10)] public int generations = 3;
    [Range(0, 1)] public float lengthVariance = 0f;
    [Range(0, 100)] public int ammendmentChance = 0;
    [Range(0f, 5f)] public int randomThickness = 0;
    [Range(0, 4)] public int pillarHeight = 0;
     
    [Header("Visual")]
    //replace with the data being created dynamically
    public Shape shape = Shape.PrimitiveCube;
    //streamline
    public GenerationType LeafGeneration = GenerationType.PrimitiveSphere;
    

    [Header("Formuli")]
    public string StartString = string.Empty;
    [HideInInspector] public List<Param> Variables = new List<Param>(); 
    [HideInInspector] public List<dictValues> dictionary = new List<dictValues>() { new dictValues('F', "F+[F+F]") };  
}
