using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPreview : MonoBehaviour {
    public Renderer textureRender;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public enum DrawMode { NoiseMap, DrawMesh, FalloffMap }
    public DrawMode mode;

    public MeshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    public TextureData textureData;

    public Material terrainMaterial;

    [Range(0, MeshSettings.numSupportedLODs - 1)] public int editorPreviewLOD;
    public bool autoUpdate;

    public void DrawMapInEditor() {
        textureData.ApplyToMaterial(terrainMaterial);
        textureData.UpdateMeshHeights(terrainMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);
        HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, Vector2.zero);

        if (mode == DrawMode.NoiseMap) {
            DrawTexture(TextureGenerator.TextureFromHeightMap(heightMap));
        } else if (mode == DrawMode.DrawMesh) {
            DrawMesh(MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, editorPreviewLOD));
        } else if (mode == DrawMode.FalloffMap) {
            DrawTexture(TextureGenerator.TextureFromHeightMap(new HeightMap(FalloffGenerator.GenerateFalloffMap(meshSettings.numVertsPerLine), 0, 1)));
        }
    }

    public void DrawTexture(Texture2D texture) { 
        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height);

        textureRender.gameObject.SetActive(true);
        meshFilter.gameObject.SetActive(false);
    } 
    public void DrawMesh(MeshData meshdata) {
        meshFilter.sharedMesh = meshdata.CreateMesh();

        textureRender.gameObject.SetActive(false);
        meshFilter.gameObject.SetActive(true);
    }
    void OnValuesUpdate() {
        if (!Application.isPlaying) {
            DrawMapInEditor();
        }
    }
    void OnTextureValuesUpdated() {
        textureData.ApplyToMaterial(terrainMaterial);
    }
    private void OnValidate() {
        if (meshSettings != null) {
            meshSettings.OnValueUpdate -= OnValuesUpdate;
            meshSettings.OnValueUpdate += OnValuesUpdate;
        }
        if (heightMapSettings != null) {
            heightMapSettings.OnValueUpdate -= OnValuesUpdate;
            heightMapSettings.OnValueUpdate += OnValuesUpdate;
        }
        if (textureData != null) {
            textureData.OnValueUpdate -= OnTextureValuesUpdated;
            textureData.OnValueUpdate += OnTextureValuesUpdated;
        }
    }
}
