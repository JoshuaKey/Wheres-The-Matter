using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

    public enum AreaType {
        FOREST,
        MINE,
        COAST,
        OCEAN,
        DESERT,
        TOWN,
    }

    Dictionary<AreaType, Dictionary<Vector2, Chunk>> areas = new Dictionary<AreaType, Dictionary<Vector2, Chunk>>();

    Dictionary<Vector2, Chunk> currChunkMap;
    AreaType currArea;
    public Vector2Int currChunk { get; private set; }

    public int seed { get; private set; }

    public void Init(Vector2 pos, int seed, AreaType area) {
        this.seed = seed;
        print("World Seed is " + seed);

        currArea = area;
        areas[currArea] = new Dictionary<Vector2, Chunk>();
        currChunkMap = areas[currArea];

        Vector2Int index = Vector2Int.zero;

        // Load new Chunks
        currChunk = GetChunkIndex(pos);
        for (int y = -1; y <= 1; y++) {
            for (int x = -1; x <= 1; x++) {
                index.x = x + currChunk.x;
                index.y = y + currChunk.y;
                LoadChunk(index);
            }
        }
    }

    public void LoadArea(Vector2 pos, World.AreaType area) {
        if(area == currArea) { // Same Area
            LoadChunks(pos);
            return;
        }

        Vector2Int index = Vector2Int.zero;

        // UnLoad old Chunks
        for (int y = -1; y <= 1; y++) {
            for (int x = -1; x <= 1; x++) {
                index.x = x + currChunk.x;
                index.y = y + currChunk.y;
                UnloadChunk(index);
            }
        }

        // Set New Area
        currArea = area;
        if (!areas.TryGetValue(currArea, out currChunkMap)) {
            areas[currArea] = new Dictionary<Vector2, Chunk>();
            currChunkMap = areas[currArea];
        }

        // Load new Chunks
        currChunk = GetChunkIndex(pos);
        for (int y = -1; y <= 1; y++) {
            for (int x = -1; x <= 1; x++) {
                index.x = x + currChunk.x;
                index.y = y + currChunk.y;
                LoadChunk(index);
            }
        }
    }

    public void LoadChunks(Vector2 pos) {
        Vector2Int currPos = GetChunkIndex(pos);

        if (currChunk != currPos) {
            Vector2Int index = Vector2Int.zero;
            // UnLoad old Chunks
            for (int y = -1; y <= 1; y++) {
                for (int x = -1; x <= 1; x++) {
                    index.x = x + currChunk.x;
                    index.y = y + currChunk.y;
                    UnloadChunk(index);
                }
            }
            // Load new Chunks
            for (int y = -1; y <= 1; y++) {
                for (int x = -1; x <= 1; x++) {
                    index.x = x + currPos.x;
                    index.y = y + currPos.y;
                    LoadChunk(index);
                }
            }

            currChunk = currPos;
        }
    }

    private Chunk LoadChunk(Vector2Int index) {
        Chunk c = null;   

        // Create Chunk
        if (!currChunkMap.TryGetValue(index, out c)) {
            c = CreateChunk(index);
        } 

        // Load Chunk??? Display???
        else {
            c.gameObject.SetActive(true);
        }
        return c;
    }

    private Chunk UnloadChunk(Vector2Int index) {
        Chunk c = null;

        if(currChunkMap.TryGetValue(index, out c)) {
            // Stop Displaying???
            c.gameObject.SetActive(false);
        }

        return c;
    }

    private Chunk CreateChunk(Vector2Int index) {
        //c = Instantiate<Chunk>(chunkPrefab, new Vector3(x, y, 0f), Quaternion.identity);

        Chunk c = Chunk.CreateChunk(index.x, index.y, currArea);
        c.transform.SetParent(this.transform);

        currChunkMap.Add(index, c);

        return c;
    }

    public Vector2Int CheckChunk(GameObject obj, Vector2Int oldIndex) {
        Vector2Int currIndex = GetChunkIndex(obj.transform.position);

        if(oldIndex != currChunk) {
            Chunk currChunk = null;
            if(!currChunkMap.TryGetValue(currIndex, out currChunk)) {
                currChunk = CreateChunk(currIndex);
                UnloadChunk(currIndex);
            }

            obj.transform.SetParent(currChunk.transform, true);
        }

        return currIndex;
    }

    public Vector2Int GetChunkIndex(Vector2 pos) {
        Vector2Int currPos = Vector2Int.zero;
        currPos.x = Mathf.FloorToInt((pos.x + 10f) / Chunk.size);
        currPos.y = Mathf.FloorToInt((pos.y + 10f) / Chunk.size);
        return currPos;
    }
}
