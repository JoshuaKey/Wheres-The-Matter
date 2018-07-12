using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour {

    [Header("Data")]
    public GameData gameData;
    public PlayerData playerData;

    [Header("Game")]
    [SerializeField] public Player player;
    [SerializeField] public Cursor cursor;
    [SerializeField] public World world;
    [SerializeField] public DialogueSystem dialogueSystem;
    [SerializeField] public Story story;
    //[SerializeField] public ElementsPage elementsPage;

    [Header("Canvas")]
    [SerializeField] public RectTransform menu;
    [SerializeField] public Canvas menuCanvas;
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
    [SerializeField] private AudioClip discoverySound;

    private Canvas currCanvas;

    public static Game Instance = null;
    private void Awake() {
        Instance = this;
    }

    private void Start() {
        //elementsPage.Start(); // Because Dumb...
        // Basically start doesnt get called if game object is disabled. therefore it cant subscribe to Events
        // Basically manual Init ourself.

        gameData.Init();
        playerData.Init();
        world.Init(player.transform.position, /*(int)System.DateTime.Now.Ticks*/0, World.AreaType.FOREST);

        DisplayGame();
        int areaIndex = (int)World.AreaType.FOREST;
        if (areaMusic.Length > areaIndex) {
            AudioManager.Instance.PlayMusic(areaMusic[areaIndex], 1.0f);
        }

        gameData.OnAtomDiscover += (x, y) => { AudioManager.Instance.PlaySound(discoverySound); newElementImage.gameObject.SetActive(true); };
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
        var temp = Random.insideUnitCircle * (cursor.GetSize()) * menuCanvas.scaleFactor;
        pos.x += temp.x;
        pos.y += temp.y;
        particlePool.AddParticle(atom, amo, pos);
    }
    public void Absorb(Atom atom, int amo) {
        print("Absorbing " + amo + " " + atom);
        gameData.Absorb(atom, amo);
    }
    public void Use(Atom atom, int amo) {
        print("Using " + amo + " " + atom);
        gameData.Use(atom, amo);
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

    public void QueueDialogue(string text, bool display = false) {
        dialogueSystem.QueueDialogue(text, display);
        player.SetCanCollect(!display);
        dialogueSystem.OnDialogueEnd += FinishDialogue;
    }
    public void QueueDialogue(string[] text, bool display = false) {
        dialogueSystem.QueueDialogue(text, display);
        player.SetCanCollect(!display);
        dialogueSystem.OnDialogueEnd += FinishDialogue;
    }
    private void FinishDialogue() {
        player.SetCanCollect(!menu.gameObject.activeInHierarchy && currCanvas == gameCanvas);
        dialogueSystem.OnDialogueEnd -= FinishDialogue;
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
        menu.gameObject.SetActive(false);
        cursor.gameObject.SetActive(false);
        SceneManager.LoadScene(0);
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