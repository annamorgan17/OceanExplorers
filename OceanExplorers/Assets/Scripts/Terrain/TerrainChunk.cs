using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class TerrainChunk {
	const float colliderGenerationDistanceThreshold = 5;
	public event System.Action<TerrainChunk, bool> onVisibilityChanged;
	public Vector2 coord;

	GameObject meshObject;
	Vector2 sampleCentre;
	Bounds bounds;

	MeshRenderer meshRenderer;
	MeshFilter meshFilter;
	MeshCollider meshCollider;

	LODInfo[] detailLevels;
	LODMesh[] lodMeshes;
	int colliderLODIndex;

	HeightMap heightMap;
	bool heightMapReceived;
	int previousLODIndex = -1;
	bool hasSetCollider;
	float maxViewDst;

	HeightMapSettings heightMapSettings;
	MeshSettings meshSettings;
	Transform viewer;

	TerrainObjectData terrainObjectData;
	PossonData possonData;

	GameObject SpawnParent;
	public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings, LODInfo[] detailLevels, int colliderLODIndex, Transform parent, Transform viewer, Material material, TerrainObjectData terrainObjectData, PossonData possonData) {
		//Set the values from inspecter
		this.coord = coord;
		this.detailLevels = detailLevels;
		this.colliderLODIndex = colliderLODIndex;
		this.heightMapSettings = heightMapSettings;
		this.meshSettings = meshSettings;
		this.viewer = viewer;
		this.terrainObjectData = terrainObjectData;
		this.possonData = possonData;

		//Sample our scene
		sampleCentre = coord * meshSettings.meshWorldSize / meshSettings.meshScale;
		Vector2 position = coord * meshSettings.meshWorldSize;

		//Calculate our bounds
		bounds = new Bounds(position, Vector2.one * meshSettings.meshWorldSize);

		//Create a mesh object
		meshObject = new GameObject("Terrain Chunk");
		meshRenderer = meshObject.AddComponent<MeshRenderer>();
		meshFilter = meshObject.AddComponent<MeshFilter>();
		meshCollider = meshObject.AddComponent<MeshCollider>();
		meshRenderer.material = material;

		//Transform our position
		meshObject.transform.position = new Vector3(position.x, 0, position.y);
		meshObject.transform.parent = parent;
		SetVisible(false);

		//Calculate our lod Meshes
		lodMeshes = new LODMesh[detailLevels.Length];
		for (int i = 0; i < detailLevels.Length; i++) {
			lodMeshes[i] = new LODMesh(detailLevels[i].lod);
			lodMeshes[i].updateCallback += UpdateTerrainChunk;
			if (i == colliderLODIndex) {
				lodMeshes[i].updateCallback += UpdateCollisionMesh;
			}
		}

		maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
		meshObject.AddComponent<NavData>();

		//Parent a spawn parent for our spawn objects
		SpawnParent = new GameObject("Spawn Parent");
		SpawnParent.transform.SetParent(meshObject.transform);

	}

	List<Vector2> points = new List<Vector2>();
	bool GeneratedObjects = false;

	private void GenerateObjects() {
		//Sample some areas using Posson Disc Sampling
		GeneratedObjects = true;
		PossonData data = possonData; 
		data.sampleRegionSize = new Vector2( heightMap.values.GetLength(0), heightMap.values.GetLength(1));  
		points = PossonDiscSampling.GeneratePoints(data.radius, data.sampleRegionSize, data.numSamplesBeforeRejection);

		//If there are points
		if (points != null) {
			//Loop through
			foreach (Vector2 point in points) {
				//Get a random object 
				float sampleHeight;
				float surfaceHeight = 58.5f;
				TerrainObject to;

				//get a default sample height
				sampleHeight = heightMap.values[(int)point.x, (int)point.y];
									
				//Handle above water
				if (sampleHeight > surfaceHeight) {
					to = terrainObjectData.getType(SpawnType.Above);
				} else { //else get a normal one
					to = terrainObjectData.getRandomObjectNoAbove();
				}

				//overwritting the height if its a fish
				if (to.objectPrefab == terrainObjectData.flockManager) {
					sampleHeight = 40.0f;
				}else if (to.spawntype == SpawnType.Surface) {
					sampleHeight = surfaceHeight;
				} 
				//Calculate it pos
				Vector3 pos = new Vector3(
										bounds.center.x + point.x - (bounds.size.x / 2), 
										sampleHeight, 
										bounds.center.y + point.y - (bounds.size.y / 2)); 
				
				//Create our object and give it the position
				GameObject gm = Object.Instantiate(to.objectPrefab);
				gm.transform.position = pos;
				gm.transform.SetParent(SpawnParent.transform);

				gm.AddComponent<NormalObjects>();
			}
		} else {
			Debug.LogError("points was null");
		}
	}
	public void Load() {
		//Load a height map on a thread
		ThreadedDataRequester.RequestData(() => HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, sampleCentre), OnHeightMapReceived);
	}



	void OnHeightMapReceived(object heightMapObject) {
		//When we receive a height map, update the terrain chunk
		this.heightMap = (HeightMap)heightMapObject;
		heightMapReceived = true;

		UpdateTerrainChunk();
	}

	Vector2 viewerPosition {
		//Get our viewer position
		get {
			return new Vector2(viewer.position.x, viewer.position.z);
		}
	}


	public void UpdateTerrainChunk() {
		//If we received a height map
		if (heightMapReceived) {
			//Find the viewer distance
			float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));

			//Calculate if this is visible
			bool wasVisible = IsVisible();
			bool visible = viewerDstFromNearestEdge <= maxViewDst;

			if (visible) {
				int lodIndex = 0;

				//Handle our detail levels
				for (int i = 0; i < detailLevels.Length - 1; i++) {
					if (viewerDstFromNearestEdge > detailLevels[i].visibleDstThreshold) {
						lodIndex = i + 1;
					} else {
						break;
					}
				}

				//if its a new detail level load it in
				if (lodIndex != previousLODIndex) {
					LODMesh lodMesh = lodMeshes[lodIndex];
					if (lodMesh.hasMesh) {
						previousLODIndex = lodIndex;
						meshFilter.mesh = lodMesh.mesh;
					} else if (!lodMesh.hasRequestedMesh) {
						lodMesh.RequestMesh(heightMap, meshSettings);
					}
				}

				
                if (lodIndex == 0) {
					SpawnParent.SetActive(true);
                } else {
					SpawnParent.SetActive(false); 
				}
			}

			//check is visibility has changed
			if (wasVisible != visible) {

				SetVisible(visible);
				if (onVisibilityChanged != null) {
					onVisibilityChanged(this, visible);
				}
			}

			//if objects havnt been generated before
			if (GeneratedObjects == false) {
				GenerateObjects();
			}
		}
	}

	public void UpdateCollisionMesh() {
		//check if we have a collider
		if (!hasSetCollider) {
			//find distance
			float sqrDstFromViewerToEdge = bounds.SqrDistance(viewerPosition);

			//requiest a new mesh if needed
			if (sqrDstFromViewerToEdge < detailLevels[colliderLODIndex].sqrVisibleDstThreshold) {
				if (!lodMeshes[colliderLODIndex].hasRequestedMesh) {
					lodMeshes[colliderLODIndex].RequestMesh(heightMap, meshSettings);
				}
			}

			//colliders
			if (sqrDstFromViewerToEdge < colliderGenerationDistanceThreshold * colliderGenerationDistanceThreshold) {
				if (lodMeshes[colliderLODIndex].hasMesh) {
					meshCollider.sharedMesh = lodMeshes[colliderLODIndex].mesh;
					hasSetCollider = true;
				}
			}
		}
	}

	public void SetVisible(bool visible) {
		meshObject.SetActive(visible);
	}

	public bool IsVisible() {
		return meshObject.activeSelf;
	}

}

class LODMesh {

	public Mesh mesh;
	public bool hasRequestedMesh;
	public bool hasMesh;
	int lod;
	public event System.Action updateCallback;

	public LODMesh(int lod) {
		this.lod = lod;
	}

	void OnMeshDataReceived(object meshDataObject) {
		//receive our mesh data
		mesh = ((MeshData)meshDataObject).CreateMesh();
		hasMesh = true;

		updateCallback();
	}

	public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings) {
		//requiest our mesh data
		hasRequestedMesh = true;
		ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, lod), OnMeshDataReceived);
	}

} 