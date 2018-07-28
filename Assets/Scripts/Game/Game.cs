using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour {

    [Header("Data")]
    public GameData gameData;
    public PlayerData playerData;
    public float time { get; private set; }

    [Header("Game")]
    [SerializeField] public Player player;
    [SerializeField] public Cursor cursor;
    [SerializeField] public World world;
    [SerializeField] public Room room;
    [SerializeField] public DialogueSystem dialogueSystem;
    [SerializeField] public Story story;
    [SerializeField] public LogSystem logSystem;

    [Header("Canvas")]
    [SerializeField] public RectTransform menu;
    [SerializeField] public Canvas worldCanvas;
    [SerializeField] public MenuUI menuCanvas;
    [SerializeField] public GameUI gameCanvas;
    [SerializeField] public ElementsPage elementCanvas;
    [SerializeField] public MapUI mapCanvas;
    [SerializeField] public LaboratoryUI laboratoryCanvas;
    [SerializeField] public SaveUI saveCanvas;
    [SerializeField] public Canvas settingCanvas;

    [Header("UI")]
    [SerializeField] public Background background;
    [SerializeField] private Image newElementImage;
    [SerializeField] private Image newMapImage;
    [SerializeField] public AtomParticlePool particlePool;
    [SerializeField] public float particleEffectAdditionalDist;

    [Header("Music")]
    [SerializeField] private AudioClip[] areaMusic;
    [SerializeField] private AudioClip discoverySound;

    private RectTransform currCanvas;

    public static Game Instance = null;
    private void Awake() {
        Instance = this;
    }

    private void Start() {
        string loadFile = PlayerPrefs.GetString("loadFile");
        if(loadFile != null && loadFile != "") {
            Load(loadFile);
            PlayerPrefs.SetString("loadFile", "");
            return;
        }

        gameData.Init();
        playerData.Init();
        world.Init(player.transform.position, (int)System.DateTime.Now.Ticks, World.AreaType.FOREST);

        DisplayGame();
        PlayAreaMusic();

        gameData.OnAtomDiscover += (x, y) => {
            if(Game.Instance.currCanvas == gameCanvas.rect) {
                AudioManager.Instance.PlaySound(discoverySound);
                Game.Instance.player.PlayDiscoverEffect();
                newElementImage.gameObject.SetActive(true);
            }    
        };
        story.onMapDiscover += (x) => { AudioManager.Instance.PlaySound(discoverySound); newMapImage.gameObject.SetActive(true); };
    }

    public void Update() {
        time += Time.deltaTime;
    }

    public void LoadArea(World.AreaType area) {
        if(world.currArea == area) { return; }

        player.transform.position = Vector3.back * 10; //Vector3(0, 0, -10);

        world.LoadArea(player.transform.position, area);

        PlayAreaMusic();
    }
    public void PlayAreaMusic() {
        int areaIndex = (int)world.currArea;
        if (areaMusic.Length > areaIndex) {
            AudioManager.Instance.PlayMusic(areaMusic[areaIndex], 1.0f);
        }
    }
    public void Move(Vector3 mvmt) {
        Vector3 pos = player.transform.position + mvmt;

        world.LoadChunks(pos);

        player.transform.position = pos;
    }

    public void PlayEffect(Atom atom, int amo, Vector3 pos) {
        var temp = Random.insideUnitCircle * (cursor.GetSize()) * worldCanvas.scaleFactor;
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

        player.CanCollect(!isActive && currCanvas == gameCanvas.rect);
        player.CanMove(!isActive && currCanvas == gameCanvas.rect);
    }
    public void DisplayMenu() {
        menu.gameObject.SetActive(true);
        player.CanCollect(false);
        player.CanMove(false);
    }
    public void HideMenu() {
        menu.gameObject.SetActive(false);

        if(currCanvas == gameCanvas.rect) {
            player.CanCollect(true);
            player.CanMove(true);
        }
    }

    public void QueueDialogue(string text, bool display = false) {
        dialogueSystem.QueueDialogue(text, display);
        player.CanCollect(!display);
        dialogueSystem.OnDialogueEnd += FinishDialogue;
    }
    public void QueueDialogue(string[] text, bool display = false) {
        dialogueSystem.QueueDialogue(text, display);
        player.CanCollect(!display);
        player.CanMove(!display);
        dialogueSystem.OnDialogueEnd += FinishDialogue;
    }
    private void FinishDialogue() {
        player.CanCollect(!menu.gameObject.activeInHierarchy && currCanvas == gameCanvas.rect);
        player.CanMove(!menu.gameObject.activeInHierarchy && currCanvas == gameCanvas.rect);
        dialogueSystem.OnDialogueEnd -= FinishDialogue;
    }

    public void DisplayGame() {
        currCanvas = gameCanvas.rect;

        elementCanvas.gameObject.SetActive(false);
        mapCanvas.gameObject.SetActive(false);
        laboratoryCanvas.gameObject.SetActive(false);
        saveCanvas.gameObject.SetActive(false);
        menu.gameObject.SetActive(false);

        player.CanCollect(true);
        player.CanMove(true);
        world.gameObject.SetActive(true);
        room.gameObject.SetActive(false);
        gameCanvas.gameObject.SetActive(true);
        background.gameObject.SetActive(false);
    }

    public void DisplayRoom() {
        currCanvas = gameCanvas.rect;

        elementCanvas.gameObject.SetActive(false);
        mapCanvas.gameObject.SetActive(false);
        laboratoryCanvas.gameObject.SetActive(false);
        saveCanvas.gameObject.SetActive(false);
        menu.gameObject.SetActive(false);

        player.CanCollect(true);
        player.CanMove(false);
        world.gameObject.SetActive(false);
        room.gameObject.SetActive(true);
        gameCanvas.gameObject.SetActive(true);
        background.gameObject.SetActive(false);
    }

    public void DisplayElements() {
        currCanvas = elementCanvas.rect;

        elementCanvas.gameObject.SetActive(true);
        mapCanvas.gameObject.SetActive(false);
        laboratoryCanvas.gameObject.SetActive(false);
        gameCanvas.gameObject.SetActive(false);
        saveCanvas.gameObject.SetActive(false);
        menu.gameObject.SetActive(false);

        player.CanCollect(false);
        player.CanMove(false);
        world.gameObject.SetActive(false);
        room.gameObject.SetActive(false);
        background.gameObject.SetActive(false);

        newElementImage.gameObject.SetActive(false);

    }

    public void DisplayMap() {
        currCanvas = mapCanvas.rect;

        elementCanvas.gameObject.SetActive(false);
        mapCanvas.gameObject.SetActive(true);
        laboratoryCanvas.gameObject.SetActive(false);
        saveCanvas.gameObject.SetActive(false);
        menu.gameObject.SetActive(false);

        player.CanCollect(false);
        player.CanMove(false);
        world.gameObject.SetActive(false);
        room.gameObject.SetActive(false);
        gameCanvas.gameObject.SetActive(true);
        background.gameObject.SetActive(false);

        newMapImage.gameObject.SetActive(false);
    }

    public void DisplayLaboratory() {
        currCanvas = laboratoryCanvas.rect;

        elementCanvas.gameObject.SetActive(false);
        mapCanvas.gameObject.SetActive(false);
        laboratoryCanvas.gameObject.SetActive(true);
        saveCanvas.gameObject.SetActive(false);
        menu.gameObject.SetActive(false);

        player.CanCollect(false);
        player.CanMove(false);
        world.gameObject.SetActive(false);
        room.gameObject.SetActive(false);
        gameCanvas.gameObject.SetActive(true);
        background.gameObject.SetActive(false);
    }

    public void DisplaySave() {
        currCanvas = saveCanvas.rect;

        elementCanvas.gameObject.SetActive(false);
        mapCanvas.gameObject.SetActive(false);
        laboratoryCanvas.gameObject.SetActive(false);
        saveCanvas.gameObject.SetActive(true);
        menu.gameObject.SetActive(false);

        player.CanCollect(false);
        player.CanMove(false);
        world.gameObject.SetActive(false);
        room.gameObject.SetActive(false);
        gameCanvas.gameObject.SetActive(true);
        background.gameObject.SetActive(false);
    }

    public void DisplaySettings() {
        print("No Settings");
    }

    public void DisplayBackground() {
        elementCanvas.gameObject.SetActive(false);
        mapCanvas.gameObject.SetActive(false);
        laboratoryCanvas.gameObject.SetActive(false);
        gameCanvas.gameObject.SetActive(false);
        saveCanvas.gameObject.SetActive(false);
        menu.gameObject.SetActive(false);

        player.CanCollect(false);
        player.CanEscape(false);
        player.CanMove(false);

        world.gameObject.SetActive(false);
        room.gameObject.SetActive(false);

        background.gameObject.SetActive(true);
    }

    public void Save(string fileName) {
        SaveData saveData = new SaveData();

        saveData.time = time;
        saveData.lastSave = System.DateTime.Now.ToShortDateString();
        print(saveData.lastSave);
        saveData.playerPosition = player.transform.position;

        world.Save(saveData);
        gameData.Save(saveData);
        playerData.Save(saveData);
        story.Save(saveData);

        string json = JsonUtility.ToJson(saveData);
        string path = fileName;

        print("Saving " + path);

        File.WriteAllText(path, json);
        
        // World Chunks
            // Moveable Objects
            // Atom Collector Life
        // Achievements??? 
    }
    public void Load(string fileName) {
        string path = fileName;
        print("Loading " + path);

        string json = File.ReadAllText(path);
        SaveData saveData = JsonUtility.FromJson<SaveData>(json);

        time = saveData.time;
        player.transform.position = saveData.playerPosition;

        world.Load(saveData);
        gameData.Load(saveData);
        playerData.Load(saveData);
        story.Load(saveData);

        PlayAreaMusic();

        DisplayGame();
    }

    public void Exit() {
        menu.gameObject.SetActive(false);
        cursor.gameObject.SetActive(false);
        SceneManager.LoadScene(0);
    }
}