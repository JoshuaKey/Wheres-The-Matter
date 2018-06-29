using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour {

    public static readonly float size = 20;
    public static List<int> seeds = new List<int>();

    [SerializeField] public int xIndex;
    [SerializeField] public int yIndex;


    public static Chunk CreateChunk(int xIndex, int yIndex, World.AreaType type) {
        Vector2 pos = new Vector2(xIndex * Chunk.size, yIndex * Chunk.size);

        GameObject go = new GameObject(type + " Chunk(" + xIndex + "," + yIndex + ")");
        Chunk c = go.AddComponent<Chunk>();
        c.transform.position = pos;
        c.xIndex = xIndex;
        c.yIndex = yIndex;

        switch (type) {
            case World.AreaType.FOREST:
                c.SetupForest();
                break;
            case World.AreaType.DESERT:
                c.SetupDesert();
                break;
            case World.AreaType.COAST:
                c.SetupCoast();
                break;
            case World.AreaType.MINE:
                c.SetupMine();
                break;
            case World.AreaType.OCEAN:
                c.SetupOcean();
                break;
            case World.AreaType.TOWN:
                c.SetupTown();
                break;
        }

        return c;
    }

    private void SetupForest() {
        var randState = Random.state;
        SetSeed();

        GameObject background = Instantiate(Resources.Load("ForestBackground", typeof(GameObject))) as GameObject;
        background.transform.SetParent(this.transform, false);

        var forestObjects = Resources.LoadAll("Forest", typeof(GameObject));
        int objectAmo = Random.Range(10, 100);
        Vector3 startPos = this.transform.position - new Vector3(size * .5f, size * .5f, 0f);

        for(int i = 0; i < objectAmo; i++) {
            int objIndex = Random.Range(0, forestObjects.Length);
            GameObject obj = Instantiate(forestObjects[objIndex]) as GameObject;
            obj.transform.SetParent(this.transform, false);

            float width = Random.value;
            float height = Random.value;
            var pos = startPos;
            pos.x += width * size;
            pos.y += height * size;
            obj.transform.position = pos;

            obj.GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(obj.transform.position.y * 100f) * -1;
            // For Moving Objects, I'd have to do this constantly...
            // Wraps at about 985 ish...
        }

        Random.state = randState;
    }

    private void SetupDesert() {
        var randState = Random.state;
        SetSeed();

        //TEMPORARY
        GameObject background = Instantiate(Resources.Load("ForestBackground", typeof(GameObject))) as GameObject;
        background.transform.SetParent(this.transform, false);
        background.GetComponent<SpriteRenderer>().color = new Color(.4f, .2f, .2f);
            
        Random.state = randState;
    }

    private void SetupCoast() {
        var randState = Random.state;
        SetSeed();

        //TEMPORARY
        GameObject background = Instantiate(Resources.Load("ForestBackground", typeof(GameObject))) as GameObject;
        background.transform.SetParent(this.transform, false);
        background.GetComponent<SpriteRenderer>().color = new Color(.4f, .2f, .2f);

        Random.state = randState;
    }

    private void SetupMine() {
        var randState = Random.state;
        SetSeed();

        //TEMPORARY
        GameObject background = Instantiate(Resources.Load("ForestBackground", typeof(GameObject))) as GameObject;
        background.transform.SetParent(this.transform, false);
        background.GetComponent<SpriteRenderer>().color = new Color(.4f, .2f, .2f);

        Random.state = randState;
    }

    private void SetupOcean() {
        var randState = Random.state;
        SetSeed();

        //TEMPORARY
        GameObject background = Instantiate(Resources.Load("ForestBackground", typeof(GameObject))) as GameObject;
        background.transform.SetParent(this.transform, false);
        background.GetComponent<SpriteRenderer>().color = new Color(.4f, .2f, .2f);

        Random.state = randState;
    }

    private void SetupTown() {
        var randState = Random.state;
        SetSeed();

        //TEMPORARY
        GameObject background = Instantiate(Resources.Load("ForestBackground", typeof(GameObject))) as GameObject;
        background.transform.SetParent(this.transform, false);
        background.GetComponent<SpriteRenderer>().color = new Color(.4f, .2f, .2f);

        Random.state = randState;
    }

    private void SetSeed() {
        int worldSeed = Game.Instance.world.seed;

        Random.InitState(xIndex + worldSeed);
        float xLoc = Random.value;
        Random.InitState(yIndex + worldSeed);
        float yLoc = Random.value;

        double noise = Mathf.PerlinNoise(xLoc / 100f, yLoc / 100f) * -.5 +
            Mathf.PerlinNoise(xLoc / 1000f, yLoc / 1000f) * -.5 +
            Mathf.PerlinNoise(xLoc / 10000f, yLoc / 10000f) * .5 +
            Mathf.PerlinNoise(xLoc / 100000f, yLoc / 100000f) * .5;

        int seed = (int)(noise * int.MaxValue);
        Random.InitState(seed);

        //print(name + " Seed: " + seed + " Noise: " + noise + " Loc: (" + xLoc + "," + yLoc + ")");
        //if (seeds.Contains(seed)) {
        //    print("Seed " + seed + " is already used!");
        //} else {
        //    seeds.Add(seed);
        //}
    }

}
