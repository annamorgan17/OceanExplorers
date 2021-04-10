using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckHasMaterial : MonoBehaviour
{
    private void Awake() {
        if (gameObject.TryGetComponent<Terrain>(out Terrain te)) {
            if (te.materialTemplate == null) {
                Debug.LogWarning("Material on this object is not here. Trying to fix now...");
                te.materialTemplate = Resources.Load("/Materials/Sand", typeof(Material)) as Material;
                if (te.materialTemplate == null) {
                    Debug.LogError("Couldnt resolve missing material, Please resolve manually. Material is located at Asset/Materials/Sand.mat");
                }
            }
        }
        if (gameObject.TryGetComponent<MeshRenderer>(out MeshRenderer mr)) {
            if (mr.material == null) {
                Debug.LogWarning("Material on this object is not here. Trying to fix now..."); 
                mr.material = Resources.Load("/Materials/Sand", typeof(Material)) as Material;
                if (mr.material == null) {
                    Debug.LogError("Couldnt resolve missing material, Please resolve manually. Material is located at Asset/Materials/Sand.mat");
                }
            }
        } 
    }
}
