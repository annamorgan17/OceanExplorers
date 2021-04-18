using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
//Generate a nav mesh dynamicaly for procedural mesh
public class NavData : MonoBehaviour {
    #if UNITY_EDITOR
        private void Awake() {
            var flags = StaticEditorFlags.OccluderStatic | StaticEditorFlags.OccludeeStatic;
            GameObjectUtility.SetStaticEditorFlags(gameObject, StaticEditorFlags.NavigationStatic);
            gameObject.AddComponent<NavMeshSourceTag>();
        }
    #endif
}
