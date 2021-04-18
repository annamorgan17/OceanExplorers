using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//a custom gradient created as a experiement
[System.Serializable] public class CustomGradient {
    //a get string function which is no longer used, but previous was used to generate a unique name for each plant based on its values
    public string GetString() {
        string Out = string.Empty; 
        foreach (var key in keys) {
            Out += key.Time.ToString("F2") + "(" + key.Colour.r.ToString("F2") + "," + key.Colour.g.ToString("F2") + "," + key.Colour.b.ToString("F2") + "," + key.Colour.a.ToString("F2") + ")";
        }
        return Out;
    }

    //Variables
    public enum BlendMode { Linear, Discrete}
    public BlendMode blendMode;
    public bool randomColour;
    [SerializeField] List<ColourKey> keys = new List<ColourKey>();

    //Create a default one with keys
    public CustomGradient() {
        AddKey(Color.white, 0);
        AddKey(Color.black, 1);
    }

    //Evauluate the colour at a specific time
    public Color Evaluate(float time) { 
        ColourKey keyLeft = keys[0];
        ColourKey keyRight = keys[NumKeys - 1];

        //find the right and left keys
        for (int i = 0; i < keys.Count; i++) {
            if (keys[i].Time < time) {
                keyLeft = keys[i];
            }
            if (keys[i].Time > time) {
                keyRight = keys[i];
                break;
            }
        }

        //if linear colour get the lerp between
        if (blendMode == BlendMode.Linear) {
            float blendTime = Mathf.InverseLerp(keyLeft.Time, keyRight.Time, time);
            return Color.Lerp(keyLeft.Colour, keyRight.Colour, blendTime);
        }

        //if not get the right colour
        return keyRight.Colour;

    }

    //adding a key at a specific time by inserting it at the correct position
    public int AddKey(Color colour, float time) {
        ColourKey newKey = new ColourKey(colour, time);
        for (int i = 0; i < NumKeys; i++) {
            if (newKey.Time < keys[i].Time) {
                keys.Insert(i, newKey);
                return i;
            }
        }
        keys.Add(newKey);
        return NumKeys - 1;
    }
    //remove a key at a position
    public void RemoveKey(int index) {
        if (keys.Count >= 2) {
            keys.RemoveAt(index);
        }
    }

    //update a key colour
    public void UpdateKeyColour(int index, Color newColour) {
        keys[index] = new ColourKey(newColour, keys[index].Time);
    }
    //update a key time
    public int UpdateKeyTime(int index, float time) {
        Color col = keys[index].Colour;
        RemoveKey(index);
        return AddKey(col, time);
    }
    //get the number of keys
    public int NumKeys {
        get {
            return keys.Count;
        }
    }
    //get a key using the index
    public ColourKey GetKey(int i) {
        if (i < NumKeys & i > 0) {

        }
        return keys[i];
    }
    //get the colours as a texture to display
    public Texture2D GetTexture(int width) {
        Texture2D texture = new Texture2D(width, 1);
        Color[] colours = new Color[width];

        //loop through and evaulate the colours
        for (int i = 0; i < width; i++) {
            colours[i] = Evaluate((float)i / (width - 1));
        }
        texture.SetPixels(colours);
        texture.Apply();
        return texture;
    }

    //data type for a colour field
    [System.Serializable]  public struct ColourKey {
        [SerializeField] Color colour;
        [SerializeField] float time;
        public ColourKey(Color colour, float time) {
            this.colour = colour;
            this.time = time;
        }
        public Color Colour {
            get {
                return colour;
            }
        }
        public float Time {
            get {
                return time;
            }
        }
    }
}
