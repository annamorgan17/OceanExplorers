using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerainGen : MonoBehaviour
{
    [SerializeField] GameObject[] FloorTiles;
    [SerializeField] GameObject Surface;
    [SerializeField] GameObject Water;
    [SerializeField] int width;
    [SerializeField] int height; 
    [SerializeField] string seed;
    [SerializeField] bool useRandomSeed = true; 
    [Range(0, 100)] public int randomFillPercent; 
    [SerializeField] int[,] map;
    [SerializeField] int Size = 10;
    [SerializeField] bool RenderOnlyAround = true;
    private GameObject parent;

    void Start(){
        //Replace for less intensive search later
        FloorTiles = Resources.LoadAll<GameObject>("Tiles/Floor");
        Surface = Resources.Load<GameObject>("Tiles/Surface");
        //Water = GameObject.FindGameObjectWithTag("Water");
        Water.SetActive(true);
        Water.transform.localScale = new Vector3(width, 0, height);
        parent = new GameObject("Terrain");
        GenerateMap(); 
    } 
    void Update(){
        if (Input.GetMouseButtonDown(0)) {
            foreach (Transform item in parent.transform) {
                Destroy(item.gameObject);
            }
            GenerateMap();
        }
    } 
    void Draw() {
        if (map != null) {
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    Vector3 pos = new Vector3(-width / 2 + x + .5f, 0, -height / 2 + y + .5f);
                    pos *= Size;
                    if ((map[x, y] != 1)) {
                        ////water tile
                        GameObject floor = GameObject.Instantiate(FloorTiles[0]); 
                        floor.transform.position = pos;
                        floor.transform.SetParent(parent.transform);
                    } else {
                        GameObject surface = GameObject.Instantiate(Surface);
                        pos.y += (surface.transform.localScale.y / 2) + 0.5f;
                        surface.transform.position = pos;
                        surface.transform.SetParent(parent.transform);
                    } 
                }
            }
        }
    } 
    void GenerateMap(){ 
        map = new int[width, height];
        RandomFillMap();
        for (int i = 0; i < 5; i++) 
            SmoothMap();
        Draw();
    } 
    void RandomFillMap() {
        if (useRandomSeed)
            seed = Time.time.ToString();
        System.Random pseudoRandom = new System.Random(seed.GetHashCode()); 
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                    map[x, y] = 1;
                else 
                    map[x, y] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0;
            }
        }
    } 
    void SmoothMap() {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);
                if (neighbourWallTiles > 4)
                    map[x, y] = 1;
                else if (neighbourWallTiles < 4)
                    map[x, y] = 0;
            }
        }
    } 
    int GetSurroundingWallCount(int gridX, int gridY) {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++) {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++) {
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height) {
                    if (neighbourX != gridX || neighbourY != gridY) 
                        wallCount += map[neighbourX, neighbourY];
                } else 
                    wallCount++;
            }
        } 
        return wallCount;
    }
}
