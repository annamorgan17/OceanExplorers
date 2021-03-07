using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class NavData : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Awake() {
        var flags = StaticEditorFlags.OccluderStatic | StaticEditorFlags.OccludeeStatic; 
        GameObjectUtility.SetStaticEditorFlags(gameObject, StaticEditorFlags.NavigationStatic);
        gameObject.AddComponent<NavMeshSourceTag>();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
