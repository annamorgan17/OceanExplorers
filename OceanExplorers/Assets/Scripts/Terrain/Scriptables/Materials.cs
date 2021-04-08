using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
[CreateAssetMenu()]
public class Materials : ScriptableObject {
    public Material SandProcedural;
    public Material Caustic;

    public RenderTexture ResultTexture;
    public int Size = 2048;

    public Material getMaterial(GameObject gm) {
        /*
        if (ResultTexture == null) {
            ResultTexture = new RenderTexture(Size, Size, 0);
            ResultTexture.name = "Baked Texture";
        }

        bakeTexture(gm);

        if (Caustic != null) {
            gm.GetComponent<Renderer>().material = Caustic;
            Caustic.mainTexture = ResultTexture;
        }
        */
        return SandProcedural;
    }
    void bakeTexture(GameObject gm) {
        var renderer = gm.GetComponent<Renderer>();
        var material = Instantiate(renderer.material);
        Graphics.Blit(material.mainTexture, ResultTexture, material);

        Texture2D frame = new Texture2D(ResultTexture.width, ResultTexture.height);
        frame.ReadPixels(new Rect(0, 0, ResultTexture.width, ResultTexture.height), 0, 0, false);
        frame.Apply();
        byte[] bytes = frame.EncodeToPNG();
        FileStream file = File.Open(@"C:\Works.png", FileMode.Create);
        BinaryWriter binary = new BinaryWriter(file);
        binary.Write(bytes);
        file.Close();
    }
}

