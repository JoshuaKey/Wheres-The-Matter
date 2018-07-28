using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

    public enum AreaType {
        FOREST,
        MINE,
        BEACH,
        OCEAN,
        DESERT,
        TOWN,
    }

    Dictionary<AreaType, Dictionary<Vector2, Chunk>> areas = new Dictionary<AreaType, Dictionary<Vector2, Chunk>>();

    Dictionary<Vector2, Chunk> currChunkMap;
    public AreaType currArea { get; private set; }
    public Vector2Int currChunk { get; private set; }

    public int seed { get; private set; }

    public delegate void OnAreaChange();
    public event OnAreaChange onAreaChange;

    public void Init(Vector2 pos, int seed, AreaType area) {
        var areaEnumerator = areas.Values.GetEnumerator();
        while (areaEnumerator.MoveNext()) {
            var curr = areaEnumerator.Current;

            var chunkEnumerator = curr.Values.GetEnumerator();
            while (chunkEnumerator.MoveNext()) {
                var currChunk = chunkEnumerator.Current;
                Destroy(currChunk.gameObject);
            }
        }

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

    public void Save(SaveData s) {
        s.worldArea = currArea;
        s.worldSeed = seed;
    }
    public void Load(SaveData s) {
        currArea = s.worldArea;
        seed = s.worldSeed;
        Init(s.playerPosition, seed, currArea);
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

        if(onAreaChange != null) {
            onAreaChange();
        }
    }

    public void LoadChunks(Vector2 pos) {
        Vector2Int currPos = GetChunkIndex(pos);

        if (currChunk != currPos) {
            var diff = currPos - currChunk;
            // CurrPos = 1,1 CurrChunk = 0,0
            // Diff = 1,1
            // Starting from CurrChunk, unload -1,-1 
            // So -1, 0, 1

            Vector2Int index = Vector2Int.zero; // Doesn't work with Diff >= 2.

            if(diff.x != 0) {
                // X Chunkss
                index.x = currChunk.x - diff.x;
                for (int i = -1; i <= 1; i++) {
                    index.y = currChunk.y + i;
                    UnloadChunk(index);
                }

                // X Chunks
                index.x = currPos.x + diff.x;
                for (int i = -1; i <= 1; i++) {
                    index.y = currPos.y + i;
                    LoadChunk(index);
                }
            }
            if (diff.y != 0) {
                // Y Chunks
                index.y = currChunk.y - diff.y;
                for (int i = -1; i <= 1; i++) {
                    index.x = currChunk.x + i;
                    UnloadChunk(index);
                }

                // Y Chunks
                index.y = currPos.y + diff.y;
                for (int i = -1; i <= 1; i++) {
                    index.x = currPos.x + i;
                    LoadChunk(index);
                }
            }

            //// UnLoad old Chunks
            //for (int y = -1; y <= 1; y++) {
            //    for (int x = -1; x <= 1; x++) {
            //        index.x = x + currChunk.x;
            //        index.y = y + currChunk.y;
            //        UnloadChunk(index);
            //    }
            //}
            //// Load new Chunks
            //for (int y = -1; y <= 1; y++) {
            //    for (int x = -1; x <= 1; x++) {
            //        index.x = x + currPos.x;
            //        index.y = y + currPos.y;
            //        LoadChunk(index);
            //    }
            //}

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
        Chunk c = Chunk.CreateChunk(index.x, index.y, currArea);
        c.transform.SetParent(this.transform);

        currChunkMap.Add(index, c);

        return c;
    }

    public Vector2Int GetChunkIndex(Vector2 pos) {
        Vector2Int currPos = Vector2Int.zero;
        currPos.x = Mathf.FloorToInt((pos.x + 10f) / Chunk.size);
        currPos.y = Mathf.FloorToInt((pos.y + 10f) / Chunk.size);
        return currPos;
    }

    public Chunk GetChunk(Vector2Int index) {
        Chunk currChunk = null;
        currChunkMap.TryGetValue(index, out currChunk);
        return currChunk;
    }
}
