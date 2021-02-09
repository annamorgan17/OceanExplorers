using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Plant.Utilities;
/// <summary>
/// Types of generation
/// </summary>
public enum GenerationType {
    PrimitiveCube,
    PrimitiveSphere,
    Mesh,
    None,
}
/// <summary>
/// Create a leaf
/// </summary>
public class Leaf{ 
    //Gameobject
    public GameObject gm; 

    /// <summary>
    /// Create a leaf
    /// </summary>
    /// <param name="transInfo">Transformation information </param> 
    /// <param name="a_Leaves">Parent</param>
    /// <param name="type">Type of generation going on</param>
    public Leaf(TransformInfo transInfo, ref GameObject a_Leaves, GenerationType type) { 
        //Create a gameobject and set its transform
        gm = new GameObject("Leaf", typeof(MeshFilter), typeof(MeshRenderer)); 
        transInfo.SetTransform(ref gm);
        gm.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f); 
        gm.tag = "Validate";
        gm.transform.SetParent(a_Leaves.transform); 

        //Switch dependant the generation type
        switch (type) {
            case GenerationType.Mesh:  
                //Mesh is handled outside of the class
                break;  
            case GenerationType.PrimitiveSphere:
                //Sphere
                gm.GetComponent<MeshFilter>().mesh = MeshExstension.PrimitiveShape(PrimitiveType.Sphere);
                break;
            case GenerationType.PrimitiveCube:
                //Cube
                gm.GetComponent<MeshFilter>().mesh = MeshExstension.PrimitiveShape(PrimitiveType.Cube);
                break;
        }
    } 

    /// <summary>
    /// Return a float colour. This is used find the colour to draw the texture
    /// </summary>
    /// <param name="gm">Gameboy in refernece for positional data</param>
    /// <param name="highest">Highest leaf possible</param>
    /// <returns></returns>
    private float ColourFloat(GameObject gm, float highest) { return gm.transform.position.y / (highest + 1); } 

    /// <summary>
    /// Draw a circle on given texture using positon 
    /// </summary>
    /// <param name="texture">Texture to draw on</param>
    /// <param name="highest">Highest point possible</param>
    /// <param name="or">Origin point</param>
    public void DrawTexture(Texture2D texture, float highest, Vector3 or) {
        //Calculate position
        Vector3 v3 = MapFromWorldSpace(gm.transform.position, or);
        //Calculate colours
        Color col = new Color(ColourFloat(gm, highest), ColourFloat(gm, highest), ColourFloat(gm, highest));
        //Draw circle
        TextureExstension.DrawCircle(ref texture, col, (int)v3.x, (int)v3.z);
        texture.Apply();
    }
     
    /// <summary>
    /// Calculate map position using world data
    /// </summary>
    /// <param name="leaf">Leaf position</param>
    /// <param name="Origin">Origin position</param>
    /// <returns></returns>
    Vector3 MapFromWorldSpace(Vector3 leaf, Vector3 Origin) { 
        float scale = 5;
        //local
        Vector3 localPositionToOrigion = (Origin - leaf) * scale;
        float textureSize = 128f;
        //return
        return new Vector3(
            (textureSize / 2f) - localPositionToOrigion.x, 
            (textureSize / 2f) - localPositionToOrigion.y, 
            (textureSize / 2f) - localPositionToOrigion.z); 
    }  
}
