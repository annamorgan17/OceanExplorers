using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A script to show a preview image or chunk of how the terrain will look
public class MapPreview : MonoBehaviour {

    //Variables
    public Renderer textureRender;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public enum DrawMode { NoiseMap, DrawMesh, FalloffMap}
    public DrawMode mode;

    public MeshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    public TextureData textureData;

    public Material terrainMaterial;

    [Range(0, MeshSettings.numSupportedLODs - 1)] public int editorPreviewLOD;
    public bool autoUpdate;

    //Drawing a map in editor
    public void DrawMapInEditor() {
        //Apply to the material
        textureData.ApplyToMaterial(terrainMaterial);
        //Update the mesh heights
        textureData.UpdateMeshHeights(terrainMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);
        HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, Vector2.zero);

        //Draw respective mode
        if (mode == DrawMode.NoiseMap) {
            DrawTexture(TextureGenerator.TextureFromHeightMap(heightMap)); //Draw a 2d Heightmap texture
        } else if (mode == DrawMode.DrawMesh) {
            DrawMesh(MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, editorPreviewLOD)); //Draw a 3d Mesh
        } else if (mode == DrawMode.FalloffMap) {
            DrawTexture(TextureGenerator.TextureFromHeightMap(new HeightMap(FalloffGenerator.GenerateFalloffMap(meshSettings.numVertsPerLine), 0, 1))); //Draw a 2D fall of mesh
        } 
    } 
    public void DrawTexture(Texture2D texture) { 
        //setting the texture to the preview
        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height);

        textureRender.gameObject.SetActive(true);
        meshFilter.gameObject.SetActive(false);
    } 
    public void DrawMesh(MeshData meshdata) {
        //Creating a mesh using data
        meshFilter.sharedMesh = meshdata.CreateMesh();

        textureRender.gameObject.SetActive(false);
        meshFilter.gameObject.SetActive(true);
    }
    void OnValuesUpdate() {
        //Draw map while it is not in play mode
        if (!Application.isPlaying) {
            DrawMapInEditor();
        }
    }
    void OnTextureValuesUpdated() {
        //Update material
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
