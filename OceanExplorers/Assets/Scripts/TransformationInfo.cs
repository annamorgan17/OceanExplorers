using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// A class to hold information from locations such as branch length and other data that might be to be retreived.
/// </summary>
public class TransformInfo {
    /// <summary>
    /// Create a transform info with given data
    /// </summary>
    /// <param name="_transform">Transformholder to hold location</param>
    /// <param name="_length">Branch length</param>
    /// <param name="_mesh"></param>
    public TransformInfo(TransformHolder _transform, float _length){
        transform = _transform; 
        branchLength = _length;  
        Y = transform.position.y;
    }
    /// <summary>
    /// Create blank transform info
    /// </summary>
    public TransformInfo(){
        transform = new TransformHolder(); 
        branchLength = 0;  
        Y = transform.position.y;
    }
    /// <summary>
    /// Set the gameobject transform using transform info data
    /// </summary>
    /// <param name="a">gameobject we are setting the values of</param>
    public void SetTransform(ref GameObject a) {
        a.transform.position = transform.position;
        a.transform.rotation = transform.rotation;  
    }

    //Transform data
    public TransformHolder transform;
    //Y value
    public float Y;
    //branch Length
    public float branchLength;  
}

/// <summary>
/// Hold the transform data. Transform cannot be saved and therefore position, scale and roation needs to be saved
/// </summary>
public class TransformHolder{
    /// <summary>
    /// Create a default value
    /// </summary>
    public TransformHolder(){
        position = new Vector3(); 
        localScale = new Vector3(); 
        rotation = new Quaternion();
    }
    /// <summary>
    /// create a transform holder with given data
    /// </summary>
    /// <param name="_Position">Position data</param>
    /// <param name="_LocalScale">Local scale data</param>
    /// <param name="_Rotation">Rotation data</param>
    public TransformHolder(Vector3 _Position, Vector3 _LocalScale, Quaternion _Rotation){
        position = _Position; 
        localScale = _LocalScale; 
        rotation = _Rotation;
    }
    /// <summary>
    /// Create based on transform as refernce
    /// </summary>
    /// <param name="transform">Transform to copy</param>
    public TransformHolder(Transform transform) {
        position = transform.position;
        rotation = transform.rotation;
        localScale = transform.localScale;
    } 
    //variables
    public Vector3 position;
    public Vector3 localScale;
    public Quaternion rotation;
}
