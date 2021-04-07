using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CustomGradient {
    public string GetString() {
        string Out = string.Empty; 
        foreach (var key in keys) {
            Out += key.Time.ToString("F2") + "(" + key.Colour.r.ToString("F2") + "," + key.Colour.g.ToString("F2") + "," + key.Colour.b.ToString("F2") + "," + key.Colour.a.ToString("F2") + ")";
        }
        return Out;
    }
    public enum BlendMode { Linear, Discrete}
    public BlendMode blendMode;
    public bool randomColour;
    [SerializeField] List<ColourKey> keys = new List<ColourKey>();
    public CustomGradient() {
        AddKey(Color.white, 0);
        AddKey(Color.black, 1);
    }
    public Color Evaluate(float time) { 
        ColourKey keyLeft = keys[0];
        ColourKey keyRight = keys[NumKeys - 1];

        for (int i = 0; i < keys.Count; i++) {
            if (keys[i].Time < time) {
                keyLeft = keys[i];
            }
            if (keys[i].Time > time) {
                keyRight = keys[i];
                break;
            }
        }
        if (blendMode == BlendMode.Linear) {
            float blendTime = Mathf.InverseLerp(keyLeft.Time, keyRight.Time, time);
            return Color.Lerp(keyLeft.Colour, keyRight.Colour, blendTime);
        }
        return keyRight.Colour;

    }
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
    public void RemoveKey(int index) {
        if (keys.Count >= 2) {
            keys.RemoveAt(index);
        }
    }
    public void UpdateKeyColour(int index, Color newColour) {
        keys[index] = new ColourKey(newColour, keys[index].Time);
    }
    public int UpdateKeyTime(int index, float time) {
        Color col = keys[index].Colour;
        RemoveKey(index);
        return AddKey(col, time);
    }
    public int NumKeys {
        get {
            return keys.Count;
        }
    }
    public ColourKey GetKey(int i) {
        if (i < NumKeys & i > 0) {

        }
        return keys[i];
    }
    public Texture2D GetTexture(int width) {
        Texture2D texture = new Texture2D(width, 1);
        Color[] colours = new Color[width];
        for (int i = 0; i < width; i++) {
            colours[i] = Evaluate((float)i / (width - 1));
        }
        texture.SetPixels(colours);
        texture.Apply();
        return texture;
    }
    [System.Serializable]
    public struct ColourKey {
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
