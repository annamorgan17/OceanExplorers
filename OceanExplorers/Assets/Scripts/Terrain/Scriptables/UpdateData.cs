﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UpdateData : ScriptableObject {
    public event System.Action OnValueUpdate;
    public bool autoUpdate;

    #if UNITY_EDITOR
    protected virtual void OnValidate() {
        if (autoUpdate) {
            UnityEditor.EditorApplication.update += NotifyUpdate;
        }
    }
    public void NotifyUpdate() {
        UnityEditor.EditorApplication.update -= NotifyUpdate;
        if (OnValueUpdate != null) {
            OnValueUpdate();
        }
    }
    #endif
}
