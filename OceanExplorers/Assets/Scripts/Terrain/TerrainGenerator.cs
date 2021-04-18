using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainGenerator : MonoBehaviour {

	const float viewerMoveThresholdForChunkUpdate = 25f;
	const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;


	public int colliderLODIndex = 0;
	public LODInfo[] detailLevels = {new LODInfo(0,0), new LODInfo(1,100), new LODInfo(2,200) };

	public MeshSettings meshSettings;
	public HeightMapSettings heightMapSettings;
	public TextureData textureSettings;
	public TerrainObjectData terrainObjectData;
	public PossonData possonData; 
	public Transform viewer;
	public Material mapMaterial;

	Vector2 viewerPosition;
	Vector2 viewerPositionOld;

	float meshWorldSize;
	int chunksVisibleInViewDst;

	Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
	List<TerrainChunk> visibleTerrainChunks = new List<TerrainChunk>();

	void Start() {
		//check our variables are okay
		if (DebugErrors()) { 
			//apply our materials and update our mesh heights
			textureSettings.ApplyToMaterial(mapMaterial);
			textureSettings.UpdateMeshHeights(mapMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);


			//find our max view dst
			float maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
			meshWorldSize = meshSettings.meshWorldSize;
			chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / meshWorldSize);

			//update our chunks
			UpdateVisibleChunks();
		}
	}

	private bool DebugErrors() { 
		bool Out = true;
        if (mapMaterial == null) {
			Debug.LogWarning("Material slot is null, Trying to fix now...");
			mapMaterial = Resources.Load("/Materials/Sand", typeof(Material)) as Material;

            if (mapMaterial == null) {
				Debug.LogError("Material slot is null and couldnt be loaded, please see this for information to fix");
				/* This issue is often caused by Unity and sometimes occurs when downloading this project for the first time
				 * Please note we have had this occur some times with first time downloads, and seems to stem from meta files missing due to gitIgnore.
				 * The Sand.mat seems to get taken off materials. 
				 * 
				 * How to fix: 
				 * -Go to Assets/Materials/ 
				 * -You may need to reimport Sand.mat 
				 * -In some causes the preview material in this scene, and the sand material in other scenes may be pink. Please drag the material to these if possible.  
				 * -Click on the Terrain Generator gameobject in the Main scene. 
				 * -The last property of the Terrain Generator script is called MapMaterial, please search for and add the sand.mat found earlier to this slot */
				Out = false;
			} 
        }
        if (meshSettings == null) { Debug.LogError("The Mesh Settings object is not set to the Terrain Generator, Please add Assets/MeshSettings.asset to TerrainGenerator gameobject"); Out = false; }
		if (heightMapSettings == null) { Debug.LogError("The Height Map Settings object is not set to the Terrain Generator, Please add Assets/HeightMapSettings.asset to TerrainGenerator gameobject"); Out = false; }
		if (textureSettings == null) { Debug.LogError("The Texture Settings object is not set to the Terrain Generator, Please add Assets/TextureSettings.asset to TerrainGenerator gameobject"); Out = false; }
		if (terrainObjectData == null) { Debug.LogError("The Terrain Object Data object is not set to the Terrain Generator, Please add Assets/TerrainObjectData.asset to TerrainGenerator gameobject"); Out = false; }
		if (possonData == null) { Debug.LogError("The Posson Data object is not set to the Terrain Generator, Please add Assets/PossonData.asset to TerrainGenerator gameobject"); Out = false; }
		if (viewer == null) {
			Debug.LogWarning("The player transform needs to be set as a property to the Terrain Generator, trying to fix now....");
			viewer = GameObject.FindGameObjectWithTag("Player").transform;
            if (viewer == null) {
				Debug.LogError("Object with 'Player' tag couldnt be found in scene");
            }
		}
		return Out;
    }
	void Update() {
		//if no variables issues
		if (DebugErrors()) { 
			//calculate our viewer position
			viewerPosition = new Vector2(viewer.position.x, viewer.position.z); 
			//if position changes
			if (viewerPosition != viewerPositionOld) {
				//update out colliders
				foreach (TerrainChunk chunk in visibleTerrainChunks) {
					chunk.UpdateCollisionMesh();
				}
			}

			//udpate our old position if we have moved too far
			if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate) {
				viewerPositionOld = viewerPosition;
				UpdateVisibleChunks();
			}
		}
	}

	void UpdateVisibleChunks() {
		//find the chunks already updated
		HashSet<Vector2> alreadyUpdatedChunkCoords = new HashSet<Vector2>();
		for (int i = visibleTerrainChunks.Count - 1; i >= 0; i--) {
			alreadyUpdatedChunkCoords.Add(visibleTerrainChunks[i].coord);
			visibleTerrainChunks[i].UpdateTerrainChunk();
		}

		//find our current chunk coord
		int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / meshWorldSize);
		int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / meshWorldSize);

		for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++) {
			for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++) {
				Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
				if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord)) {
					if (terrainChunkDictionary.ContainsKey(viewedChunkCoord)) {
						terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
					} else {
						TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, heightMapSettings, meshSettings, detailLevels, colliderLODIndex, transform, viewer, mapMaterial, terrainObjectData, possonData);
						terrainChunkDictionary.Add(viewedChunkCoord, newChunk);
						newChunk.onVisibilityChanged += OnTerrainChunkVisibilityChanged;
						newChunk.Load();
					}
				}

			}
		}
	}

	void OnTerrainChunkVisibilityChanged(TerrainChunk chunk, bool isVisible) {
		if (isVisible) {
			visibleTerrainChunks.Add(chunk);
		} else {
			visibleTerrainChunks.Remove(chunk);
		}
	}

}

[System.Serializable] public struct LODInfo {
	[Range(0, MeshSettings.numSupportedLODs - 1)]
	public int lod;
	public float visibleDstThreshold;

	public LODInfo(int lod, float visibleDstThreshold) {
		this.lod = lod;
		this.visibleDstThreshold = visibleDstThreshold;
	}
	public float sqrVisibleDstThreshold {
		get {
			return visibleDstThreshold * visibleDstThreshold;
		}
	}
}