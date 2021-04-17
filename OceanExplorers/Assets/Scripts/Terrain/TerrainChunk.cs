﻿using UnityEngine;
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
		this.coord = coord;
		this.detailLevels = detailLevels;
		this.colliderLODIndex = colliderLODIndex;
		this.heightMapSettings = heightMapSettings;
		this.meshSettings = meshSettings;
		this.viewer = viewer;
		this.terrainObjectData = terrainObjectData;
		this.possonData = possonData;

		sampleCentre = coord * meshSettings.meshWorldSize / meshSettings.meshScale;
		Vector2 position = coord * meshSettings.meshWorldSize;
		bounds = new Bounds(position, Vector2.one * meshSettings.meshWorldSize);


		meshObject = new GameObject("Terrain Chunk");
		meshRenderer = meshObject.AddComponent<MeshRenderer>();
		meshFilter = meshObject.AddComponent<MeshFilter>();
		meshCollider = meshObject.AddComponent<MeshCollider>();
		meshRenderer.material = material;

		meshObject.transform.position = new Vector3(position.x, 0, position.y);
		meshObject.transform.parent = parent;
		SetVisible(false);

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

		SpawnParent = new GameObject("Spawn Parent");
		SpawnParent.transform.SetParent(meshObject.transform);

	}
	List<Vector2> points = new List<Vector2>();
	bool GeneratedObjects = false;
	private void GenerateObjects() {
		GeneratedObjects = true;
		PossonData data = possonData; 
		data.sampleRegionSize = new Vector2( heightMap.values.GetLength(0), heightMap.values.GetLength(1));  
		points = PossonDiscSampling.GeneratePoints(data.radius, data.sampleRegionSize, data.numSamplesBeforeRejection);

		if (points != null) {
			foreach (Vector2 point in points) {
				TerrainObject to = terrainObjectData.getRandomObject();
				GameObject gm = Object.Instantiate( to.objectPrefab);
				float sampleHeight;

				if (to.objectPrefab == terrainObjectData.flockManager)
                {
					sampleHeight = 40.0f;
				}
                else
                {
					sampleHeight = heightMap.values[(int)point.x, (int)point.y];
				}
							

				float surfaceHeight = 58.5f;
				if (to.surfaceSpawn == true) {
					sampleHeight = surfaceHeight;

				}
				Vector3 pos = new Vector3(
										bounds.center.x + point.x - (bounds.size.x / 2), 
										sampleHeight, 
										bounds.center.y + point.y - (bounds.size.y / 2));
                if (to.randomRotation) {
					//gm.transform.localScale = new Vector3(0, Random.Range(0,360), 0);
                }
				//change to be less intensive 
				gm.transform.position = pos;
				gm.transform.SetParent(SpawnParent.transform);
			}
		} else {
			Debug.LogError("points was null");
		}
	}
	public void Load() {
		ThreadedDataRequester.RequestData(() => HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, sampleCentre), OnHeightMapReceived);
	}



	void OnHeightMapReceived(object heightMapObject) {
		this.heightMap = (HeightMap)heightMapObject;
		heightMapReceived = true;

		UpdateTerrainChunk();
	}

	Vector2 viewerPosition {
		get {
			return new Vector2(viewer.position.x, viewer.position.z);
		}
	}


	public void UpdateTerrainChunk() {
		if (heightMapReceived) {
			float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));

			bool wasVisible = IsVisible();
			bool visible = viewerDstFromNearestEdge <= maxViewDst;

			if (visible) {
				int lodIndex = 0;

				for (int i = 0; i < detailLevels.Length - 1; i++) {
					if (viewerDstFromNearestEdge > detailLevels[i].visibleDstThreshold) {
						lodIndex = i + 1;
					} else {
						break;
					}
				}

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

			if (wasVisible != visible) {

				SetVisible(visible);
				if (onVisibilityChanged != null) {
					onVisibilityChanged(this, visible);
				}
			}
			if (GeneratedObjects == false) {
				GenerateObjects();
			}
		}
	}

	public void UpdateCollisionMesh() {
		if (!hasSetCollider) {
			float sqrDstFromViewerToEdge = bounds.SqrDistance(viewerPosition);

			if (sqrDstFromViewerToEdge < detailLevels[colliderLODIndex].sqrVisibleDstThreshold) {
				if (!lodMeshes[colliderLODIndex].hasRequestedMesh) {
					lodMeshes[colliderLODIndex].RequestMesh(heightMap, meshSettings);
				}
			}

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
		mesh = ((MeshData)meshDataObject).CreateMesh();
		hasMesh = true;

		updateCallback();
	}

	public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings) {
		hasRequestedMesh = true;
		ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, lod), OnMeshDataReceived);
	}

} 