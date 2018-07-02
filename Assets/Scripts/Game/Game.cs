using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour {

    [Header("Data")]
    public GameData gameData;
    public PlayerData playerData;

    [Header("Game")]
    [SerializeField] public Player player;
    [SerializeField] public Cursor cursor;
    [SerializeField] public World world;

    [Header("Canvas")]
    [SerializeField] public RectTransform menu;
    [SerializeField] public Canvas gameCanvas;
    [SerializeField] public Canvas elementCanvas;
    [SerializeField] public Canvas mapCanvas;
    [SerializeField] public Canvas laboratoryCanvas;
    [SerializeField] public Canvas saveCanvas;
    [SerializeField] public Canvas settingCanvas;

    [Header("UI")]
    [SerializeField] private Image newElementImage;
    [SerializeField] public AtomParticlePool particlePool;
    [SerializeField] public float particleEffectAdditionalDist;

    [Header("Music")]
    [SerializeField] private AudioClip[] areaMusic;

    //private Canvas previousCanvas;
    private Canvas currCanvas;

    public static Game Instance = null;
    private void Awake() {
        if(Instance == null) {
            Instance = this;
        } else {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }

    private void Start() {
        gameData.Init();
        playerData.Init();
        world.Init(player.transform.position, (int)System.DateTime.Now.Ticks, World.AreaType.FOREST);

        DisplayGame();
        int areaIndex = (int)World.AreaType.FOREST;
        if (areaMusic.Length > areaIndex) {
            AudioManager.Instance.PlayMusic(areaMusic[areaIndex], 1.0f);
        }
    }
    
    public void LoadArea(World.AreaType area) {
        if(world.currArea == area) { return; }

        player.transform.position = Vector3.back * 10; //Vector3(0, 0, -10);

        int areaIndex = (int)area;
        if (areaMusic.Length > areaIndex) {
            AudioManager.Instance.PlayMusic(areaMusic[areaIndex], 1.0f);
        }

        world.LoadArea(player.transform.position, area);
    }
    public void Move(Vector3 mvmt) {
        Vector3 pos = player.transform.position + mvmt;

        world.LoadChunks(pos);

        player.transform.position = pos;
    }

    public void PlayEffect(Atom atom, int amo, Vector3 pos) {
        var temp = Random.insideUnitCircle * (cursor.GetSize()/2 + particleEffectAdditionalDist);
        pos.x += temp.x;
        pos.y += temp.y;
        particlePool.AddParticle(atom, amo, pos);
    }
    public void Absorb(Atom atom, int amo) {
        print("Absorbing " + amo + " " + atom);

        AtomData data = gameData.FindAtomData(atom.GetAtomicNumber());
        AtomInfo info = gameData.FindAtomInfo(atom.GetAtomicNumber());
        if (data == null) {
            return;
        }
        
        if (!info.IsDiscovered()) {
            print("Discovered " + atom.name);

            info.SetIsDiscovered(true);
            if (ElementsPage.Instance != null) {
                ElementsPage.Instance.elementSection.Refresh();
            }
            newElementImage.gameObject.SetActive(true);
        }
        data.Gain(amo);
    }
    public void Use(Atom atom, int amo) {
        print("Using " + amo + " " + atom);
        AtomData data = gameData.FindAtomData(atom.GetAtomicNumber());
        data.Lose(amo);
    }

    public void ToggleMenu() {
        bool isActive = !menu.gameObject.activeInHierarchy;
        menu.gameObject.SetActive(isActive);

        player.SetCanCollect(!isActive && currCanvas == gameCanvas);
    }
    public void DisplayMenu() {
        menu.gameObject.SetActive(true);
        player.SetCanCollect(false);
    }
    public void HideMenu() {
        menu.gameObject.SetActive(false);

        if(currCanvas == gameCanvas) {
            player.SetCanCollect(true);
        }
    }

    public void DisplayGame() {
        currCanvas = gameCanvas;

        elementCanvas.gameObject.SetActive(false);
        mapCanvas.gameObject.SetActive(false);
        laboratoryCanvas.gameObject.SetActive(false);
        menu.gameObject.SetActive(false);

        player.SetCanCollect(true);
        world.gameObject.SetActive(true);
        gameCanvas.gameObject.SetActive(true);
    }

    public void DisplayElements() {
        //if (currCanvas == elementCanvas) { DisplayGame(); return; }
        //previousCanvas = currCanvas;
        currCanvas = elementCanvas;

        elementCanvas.gameObject.SetActive(true);
        mapCanvas.gameObject.SetActive(false);
        laboratoryCanvas.gameObject.SetActive(false);
        gameCanvas.gameObject.SetActive(false);
        menu.gameObject.SetActive(false);

        //player.enabled = false;
        player.SetCanCollect(false);
        world.gameObject.SetActive(false);

        newElementImage.gameObject.SetActive(false);
    }

    public void DisplayMap() {
        //if(currCanvas == mapCanvas) { DisplayGame(); return; }
        //previousCanvas = currCanvas;
        currCanvas = mapCanvas;

        elementCanvas.gameObject.SetActive(false);
        mapCanvas.gameObject.SetActive(true);
        laboratoryCanvas.gameObject.SetActive(false);
        gameCanvas.gameObject.SetActive(false);
        menu.gameObject.SetActive(false);

        //player.enabled = false;
        player.SetCanCollect(false);
        world.gameObject.SetActive(false);
    }

    public void DisplayLaboratory() {
        //if (currCanvas == laboratoryCanvas) { DisplayGame(); return; }
        //previousCanvas = currCanvas;
        currCanvas = laboratoryCanvas;

        elementCanvas.gameObject.SetActive(false);
        mapCanvas.gameObject.SetActive(false);
        laboratoryCanvas.gameObject.SetActive(true);
        gameCanvas.gameObject.SetActive(false);
        menu.gameObject.SetActive(false);

        //player.enabled = false;
        player.SetCanCollect(false);
        world.gameObject.SetActive(false);
        gameCanvas.gameObject.SetActive(true);
    }

    public void DisplaySave() {
        print("No Save UI");
    }

    public void DisplaySettings() {
        print("No Settings");
    }

    public void Save() {
        print("Saving");
    }
    public void Load() {
        print("Loading");
    }

    public void Exit() {
        Application.Quit();
        Debug.Break();
    }
}
//RULES:
// You should not pass Atom Unknown, Let each object handle it.

//WORLD:
// World is divided into Chunks
// Each Chunk has its own Background + Collider + Objects. It will be slightly bigger then Camera Size, in Square

// At start, we should choose a base seed for the center Chunk

// Each Chunk should then be loaded via a seed based off the base...?
// However, we should only load a chunk if it is visibile 

// Also, chunks should only be generated if they have not been tampered with. 
// Afterwards, we should load the new state