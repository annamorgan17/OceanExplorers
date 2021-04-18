using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Basic script to check if a object has the sand material, and if not, try and load it in
//This is attached to sand objects to deal with a github issue where material gets lost
public class CheckHasMaterial : MonoBehaviour {
    private void Awake() {
        //Dealing with terrain gameobjects
        //try and get the component
        if (gameObject.TryGetComponent<Terrain>(out Terrain te)) {
            //make sure its null
            if (te.materialTemplate == null) {
                Debug.LogWarning("Material on this object is not here. Trying to fix now...");
                //try and load it in
                te.materialTemplate = Resources.Load("/Materials/Sand", typeof(Material)) as Material;
                //check load was succseful
                if (te.materialTemplate == null) {
                    Debug.LogError("Couldnt resolve missing material, Please resolve manually. Material is located at Asset/Materials/Sand.mat");
                }
            }
        }

        //General materials
        //Try and get componet
        if (gameObject.TryGetComponent<MeshRenderer>(out MeshRenderer mr)) {
            //check its null
            if (mr.material == null) {
                //try and load the material
                Debug.LogWarning("Material on this object is not here. Trying to fix now...");  
                mr.material = Resources.Load("/Materials/Sand", typeof(Material)) as Material;
                //check material is loaded
                if (mr.material == null) {
                    Debug.LogError("Couldnt resolve missing material, Please resolve manually. Material is located at Asset/Materials/Sand.mat");
                }
            }
        } 
    }
}
