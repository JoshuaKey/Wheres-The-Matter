using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Story : MonoBehaviour {

    public delegate void OnAction();
    public delegate bool OnCondition();
    public struct Event {
        public OnAction onAction;
        public OnCondition onCondition;
        public void CheckCondition() {
            if(onCondition != null && onCondition.Invoke()) {
                if(onAction != null) {
                    onAction.Invoke();
                }
            }
        }
        public void CheckCondition<T>(T arg) {
            if (onCondition != null && onCondition.Invoke()) {
                if (onAction != null) {
                    onAction.Invoke();
                }
            }
        }
        public void CheckCondition<T, U>(T arg1, U arg2) {
            if (onCondition != null && onCondition.Invoke()) {
                if (onAction != null) {
                    onAction.Invoke();
                }
            }
        }
        public void CallAction() {
            if (onAction != null) {
                onAction.Invoke();
            }
        }
    }


    [SerializeField] private int chapter = 0;

    [Header("Intro Info")]
    [SerializeField] private Sprite introImage;

    [Header("Letter Info")]
    [SerializeField] private Sprite letterImage;
    [SerializeField][TextArea(1, 5)] private string letterText;
    [SerializeField] private float letterStartPos;
    [SerializeField] private float letterTextHeight;
    [SerializeField] private float letterImageHeight;
    [SerializeField] private float letterAnimateTime;

    [Header("Dialogue")]
    [SerializeField] [TextArea(1, 5)] private string[] doctorChapter0LetterText; // Chapter 0
    [SerializeField] [TextArea(1, 5)] private string[] doctorChapter1RoomIntroText;
    [SerializeField] [TextArea(1, 5)] private string[] doctorChapter1EndCollectText;
    [SerializeField] [TextArea(1, 5)] private string[] doctorChapter2CraftText;
    [SerializeField] [TextArea(1, 5)] private string[] doctorChapter3ForestText;
    [SerializeField] [TextArea(1, 5)] private string[] doctorChapter3CollectText;
    [SerializeField] [TextArea(1, 5)] private string[] doctorChapter3ExplosionText;
    [SerializeField] [TextArea(1, 5)] private string[] doctorChapter4WorldText;

    [Header("Progression")]
    [SerializeField] private Craftable firstCraft;

    [Header("Sounds")]
    [SerializeField] private AudioClip knockSound;
    [SerializeField] private AudioClip paperSound;
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private AudioClip music;

    [Header("Professor Info")]
    [SerializeField] private string professorName;
    [SerializeField] private Sprite professorImage;

    [Header("Map Unlockables")]
    [SerializeField] private MapUI mapUI;
    [SerializeField] public Craftable mineUnlockable;
    [SerializeField] public Craftable beachUnlockable;
    [SerializeField] public Craftable oceanUnlockable;
    [SerializeField] public Craftable desertUnlockable;
    [SerializeField] public Craftable townUnlockable;

    public delegate void OnMapDiscover(World.AreaType type);
    public event OnMapDiscover onMapDiscover;

    private void Start() {
        ProgressStory();

        Game.Instance.playerData.OnCraftableProduced += CheckMap;
        Game.Instance.playerData.OnCraftableSold += CheckMap;
        Game.Instance.playerData.OnMoneyChange += CheckWin;
    }
    
    public void Save(SaveData s) {
        s.storyChapter = chapter;
    }
    public void Load(SaveData s) {
        chapter = s.storyChapter;
        StopAllCoroutines();
        //ProgressStory();

        //mapUI.UnlockArea(World.AreaType.MINE, Game.Instance.playerData.GetCraftableAmount(mineUnlockable) > 0);
        //mapUI.UnlockArea(World.AreaType.BEACH, Game.Instance.playerData.GetCraftableAmount(beachUnlockable) > 0);
        //mapUI.UnlockArea(World.AreaType.DESERT, Game.Instance.playerData.GetCraftableAmount(desertUnlockable) > 0);
        //mapUI.UnlockArea(World.AreaType.OCEAN, Game.Instance.playerData.GetCraftableAmount(oceanUnlockable) > 0);
        //mapUI.UnlockArea(World.AreaType.TOWN, Game.Instance.playerData.GetCraftableAmount(townUnlockable) > 0);   
    }

    public void CheckMap(Craftable c, float amo) {
        return;

        if(c == mineUnlockable) {
            Game.Instance.logSystem.Log("Discovered Mine");
            mapUI.UnlockArea(World.AreaType.MINE, Game.Instance.playerData.GetCraftableAmount(c) > 0);
            if(onMapDiscover != null) {
                onMapDiscover(World.AreaType.MINE);
            }
        } else if(c == beachUnlockable) {
            Game.Instance.logSystem.Log("Discovered Beach");
            mapUI.UnlockArea(World.AreaType.BEACH, Game.Instance.playerData.GetCraftableAmount(c) > 0);
            if (onMapDiscover != null) {
                onMapDiscover(World.AreaType.BEACH);
            }
        } else if (c == desertUnlockable) {
            Game.Instance.logSystem.Log("Discovered Desert");
            mapUI.UnlockArea(World.AreaType.DESERT, Game.Instance.playerData.GetCraftableAmount(c) > 0);
            if (onMapDiscover != null) {
                onMapDiscover(World.AreaType.DESERT);
            }
        } else if (c == oceanUnlockable) {
            Game.Instance.logSystem.Log("Discovered Ocean");
            mapUI.UnlockArea(World.AreaType.OCEAN, Game.Instance.playerData.GetCraftableAmount(c) > 0);
            if (onMapDiscover != null) {
                onMapDiscover(World.AreaType.OCEAN);
            }
        } else if (c == townUnlockable) {
            Game.Instance.logSystem.Log("Discovered Town");
            mapUI.UnlockArea(World.AreaType.TOWN, Game.Instance.playerData.GetCraftableAmount(c) > 0);
            if (onMapDiscover != null) {
                onMapDiscover(World.AreaType.TOWN);
            }
        }
    }

    public void CheckWin(float money) {
        //
    }

    public void ProgressStory() {
        switch (chapter) {
            case 0:
                Chapter0();
                break;
            case 1:
                Chapter1();
                break;
            case 2:
                Chapter2();
                break;
            case 3:
                Chapter3();
                break;
            case 4:
                Chapter4();
                break;
            default:
                Game.Instance.particlePool.Story();
                Game.Instance.playerData.Story();
                Game.Instance.laboratoryCanvas.craftUI.Story();
                Game.Instance.laboratoryCanvas.upgradeUI.Story();
                Game.Instance.room.Story();
                break;
        }
    }

    private void Chapter0() {
        StartCoroutine(Chain(
            Do(() => {
                if (AudioManager.Instance.GetMusic() != music || !AudioManager.Instance.IsMusicPlaying()) {
                    AudioManager.Instance.PlayMusic(music);
                }

                // Disable UI
                Game.Instance.background.cozyImage.sprite = introImage;
                Game.Instance.background.cozyImage.gameObject.SetActive(true);
                Game.Instance.background.letterImage.gameObject.SetActive(false);
                Game.Instance.background.text.gameObject.SetActive(false);
                Game.Instance.DisplayBackground();

                Game.Instance.player.CanCollect(false);
                Game.Instance.player.CanMove(false);
                Game.Instance.player.CanEscape(false);
            }),
            Wait(3.0f),
            Do(() => {
                 AudioManager.Instance.PlaySound(knockSound);
            }),
            Wait(.8f),
            Do(() => {
                AudioManager.Instance.PlaySound(paperSound);
                Game.Instance.background.letterImage.sprite = letterImage;
                Game.Instance.background.text.text = letterText;

                var letterPos = Game.Instance.background.letterImage.rectTransform.position;
                letterPos.y = 0;
                Game.Instance.background.letterImage.rectTransform.position = letterPos;

                var imageSize = Game.Instance.background.letterImage.rectTransform.sizeDelta;
                imageSize.y = letterImageHeight;
                Game.Instance.background.letterImage.rectTransform.sizeDelta = imageSize;

                Game.Instance.background.letterImage.gameObject.SetActive(true);
                Game.Instance.background.text.gameObject.SetActive(true);
                Game.Instance.DisplayBackground();
            }),
            AnimatePosition(letterAnimateTime, new Vector3(0f, 2.175f*Screen.height, 0f), 
                Game.Instance.background.letterImage.transform),
            Do(() => {
                Game.Instance.QueueDialogue(doctorChapter0LetterText, true);
            }),
            Wait(() => {
                return Game.Instance.dialogueSystem.IsFinished();
            }),
            Do(() => {
                chapter++;
                ProgressStory();
            })
            ));
    }
    private void Chapter1() {
        StartCoroutine(Chain(
            Do(() => {
                if(AudioManager.Instance.GetMusic() != music || !AudioManager.Instance.IsMusicPlaying()) {
                    AudioManager.Instance.PlayMusic(music);
                }

                Game.Instance.DisplayRoom();

                Game.Instance.gameCanvas.moneyRect.gameObject.SetActive(false);
                Game.Instance.gameCanvas.menuButton.gameObject.SetActive(false);
                Game.Instance.playerData.Story();
                Game.Instance.player.CanEscape(false);
                Game.Instance.player.CanCollect(false);
                Game.Instance.player.CanMove(false);
            }),
            Wait(.75f),
            Do(() => {
                Game.Instance.QueueDialogue(doctorChapter1RoomIntroText, true);
            }),
            Wait(() => {
                return Game.Instance.dialogueSystem.IsFinished();
            }),
            Do(() => {
                Game.Instance.player.CanEscape(false);
                Game.Instance.player.CanMove(false);
                Game.Instance.player.CanCollect(true);
            }),
            Wait(() => {
                return Game.Instance.gameData.FindAtomData(1).GetCurrAmo() >= 10000;
            }),
            Do(() => {
                Game.Instance.QueueDialogue(doctorChapter1EndCollectText, true);
            }),
            Wait(() => {
                return Game.Instance.dialogueSystem.IsFinished();
            }),
            Do(() => {
                chapter++;
                ProgressStory();
            })
            ));
    }
    public void Chapter2() {
        StartCoroutine(Chain(
            Do(() => {
                if (AudioManager.Instance.GetMusic() != music || !AudioManager.Instance.IsMusicPlaying()) {
                    AudioManager.Instance.PlayMusic(music);
                }

                if(Game.Instance.gameData.FindAtomData(1).GetCurrAmo() < 10000) {
                    var data = Game.Instance.gameData.FindAtomData(1);
                    Game.Instance.gameData.Absorb(data.GetAtom(), 10000 - data.GetCurrAmo());
                }

                Game.Instance.room.Story();
                Game.Instance.DisplayRoom();

                Game.Instance.player.CanCollect(false);
                Game.Instance.player.CanMove(false);
                Game.Instance.player.CanEscape(true);
                Game.Instance.gameCanvas.gameObject.SetActive(true);
                Game.Instance.gameCanvas.moneyRect.gameObject.SetActive(true);
                Game.Instance.gameCanvas.menuButton.gameObject.SetActive(true);

                Game.Instance.menuCanvas.elementBtn.interactable = false;
                Game.Instance.menuCanvas.resumeBtn.interactable = false;
                Game.Instance.menuCanvas.mapBtn.interactable = false;
                Game.Instance.menuCanvas.labBtn.interactable = true;
                Game.Instance.menuCanvas.exitBtn.interactable = true;
                Game.Instance.menuCanvas.saveBtn.interactable = true;
                //Game.Instance.menuCanvas.settingBtn.interactable = true;

                Game.Instance.laboratoryCanvas.backBtn.gameObject.SetActive(false);
                Game.Instance.laboratoryCanvas.upgradeBtn.interactable = false;
                Game.Instance.laboratoryCanvas.combineBtn.interactable = false;
                Game.Instance.laboratoryCanvas.splitBtn.interactable = false;
                Game.Instance.laboratoryCanvas.craftBtn.interactable = true;

                Game.Instance.saveCanvas.backBtn.gameObject.SetActive(false);
                Game.Instance.settingCanvas.backBtn.gameObject.SetActive(false);

                Game.Instance.laboratoryCanvas.craftUI.Story();
                Game.Instance.playerData.Story();
            }),
            Wait(() => {
                return Game.Instance.playerData.GetCraftableAmount(firstCraft) > 0;
            }),
            Wait(() => {
                return !Game.Instance.laboratoryCanvas.resultUI.gameObject.activeInHierarchy;
            }),
            Wait(.2f),
            Do(() => {
                Game.Instance.QueueDialogue(doctorChapter2CraftText, true);
            }),
            Wait(() => {
                return Game.Instance.dialogueSystem.IsFinished();
            }),           
            Do(() => {
                chapter++;
                ProgressStory();
            })
            ));
    }
    public void Chapter3() {
        StartCoroutine(Chain(
            Do(() => {
                if (AudioManager.Instance.GetMusic() != music || !AudioManager.Instance.IsMusicPlaying()) {
                    AudioManager.Instance.PlayMusic(music);
                }

                Game.Instance.DisplayLaboratory();
                Game.Instance.laboratoryCanvas.DisplayCraft();

                Game.Instance.player.CanCollect(false);
                Game.Instance.player.CanMove(false);
                Game.Instance.player.CanEscape(true);

                Game.Instance.menuCanvas.resumeBtn.interactable = true;
                Game.Instance.menuCanvas.elementBtn.interactable = false;
                Game.Instance.menuCanvas.mapBtn.interactable = false;
                Game.Instance.menuCanvas.labBtn.interactable = true;
                Game.Instance.menuCanvas.exitBtn.interactable = true;
                Game.Instance.menuCanvas.saveBtn.interactable = true;
                Game.Instance.menuCanvas.settingBtn.interactable = true;

                Game.Instance.laboratoryCanvas.backBtn.gameObject.SetActive(true);
                Game.Instance.laboratoryCanvas.upgradeBtn.interactable = false;
                Game.Instance.laboratoryCanvas.combineBtn.interactable = false;
                Game.Instance.laboratoryCanvas.splitBtn.interactable = false;
                Game.Instance.laboratoryCanvas.craftBtn.interactable = true;

                Game.Instance.saveCanvas.backBtn.gameObject.SetActive(true);
                Game.Instance.settingCanvas.backBtn.gameObject.SetActive(true);
                Game.Instance.playerData.Story();
            }),
            Wait(() => {
                return Game.Instance.world.gameObject.activeInHierarchy;
            }),
            Do(() => {
                Game.Instance.PlayAreaMusic();
                Game.Instance.QueueDialogue(doctorChapter3ForestText, true);
            }),
            Wait(() => {
                return Game.Instance.dialogueSystem.IsFinished();
            }),
            Wait(() => {
                return Game.Instance.gameData.FindAtomData(1).GetCurrAmo() >= 100000;
            }),
            Do(() => {
                Game.Instance.QueueDialogue(doctorChapter3CollectText, true);
            }),
            Wait(() => {
                return Game.Instance.dialogueSystem.IsFinished();
            }),
            Do(() => {
                Game.Instance.player.ScreenShake(4, 2.5f);
                AudioManager.Instance.PlaySound(explosionSound);
                Game.Instance.particlePool.Story();

                Atom a = Game.Instance.gameData.FindAtom(1);
                int amo = Game.Instance.gameData.FindAtomData(1).GetCurrAmo();
                Game.Instance.gameData.Use(a, amo);
                Game.Instance.gameData.Absorb(a, 100);

                Game.Instance.playerData.Reset();
            }),
            Wait(1.0f),
            Do(() => {
                Game.Instance.QueueDialogue(doctorChapter3ExplosionText, true);
            }),
            Wait(() => {
                return Game.Instance.dialogueSystem.IsFinished();
            }),
           Do(() => {
               chapter++;
               ProgressStory();
           })
           ));
    }
    public void Chapter4() {
        StartCoroutine(Chain(
            Do(() => {
                Game.Instance.player.CanCollect(true);
                Game.Instance.player.CanMove(true);
                Game.Instance.player.CanEscape(true);

                Game.Instance.menuCanvas.elementBtn.interactable = true;
                Game.Instance.menuCanvas.mapBtn.interactable = false;
                Game.Instance.menuCanvas.labBtn.interactable = true;
                Game.Instance.menuCanvas.exitBtn.interactable = true;
                Game.Instance.menuCanvas.saveBtn.interactable = true;
                //Game.Instance.menuCanvas.settingBtn.interactable = true;

                Game.Instance.laboratoryCanvas.backBtn.gameObject.SetActive(true);
                Game.Instance.laboratoryCanvas.upgradeBtn.interactable = true;
                Game.Instance.laboratoryCanvas.combineBtn.interactable = false;
                Game.Instance.laboratoryCanvas.splitBtn.interactable = false;
                Game.Instance.laboratoryCanvas.craftBtn.interactable = true;

                Game.Instance.playerData.Story();
                Game.Instance.particlePool.Story();
            }),
            Wait(() => {
                return Game.Instance.playerData.GetUpgradeLevel(PlayerData.UpgradeType.Collect_Weight) > 1;
            }),
            Wait(() => {
                return !Game.Instance.laboratoryCanvas.resultUI.gameObject.activeInHierarchy;
            }),
            Wait(.2f),
            Do(() => {
                Game.Instance.QueueDialogue(doctorChapter4WorldText, true);
            }),
            Wait(() => {
                return Game.Instance.dialogueSystem.IsFinished();
            }),
           Do(() => {
               Game.Instance.laboratoryCanvas.combineBtn.interactable = true;
               Game.Instance.laboratoryCanvas.splitBtn.interactable = true;

               Game.Instance.menuCanvas.mapBtn.interactable = true;

               Game.Instance.laboratoryCanvas.craftUI.Story();
               Game.Instance.laboratoryCanvas.upgradeUI.Story();

               chapter++;
               ProgressStory();
           })
           ));
    }

    private IEnumerator AnimatePosition(float time, Vector3 posAdd, Transform trans) {
        float endTime = Time.time + time;
        Vector3 startPos = trans.position;
        Vector3 endPos = startPos + posAdd;

        while (Time.time < endTime) {
            float t = 1 - (endTime - Time.time) / time;

            var newPos = Vector3.Lerp(startPos, endPos, t);
            trans.position = newPos;

            yield return null;
        }

        trans.position = endPos;
    }

    /**
 * Usage: StartCoroutine(CoroutineUtils.Chain(...))
 * For example:
 *     StartCoroutine(CoroutineUtils.Chain(
 *         CoroutineUtils.Do(() => Debug.Log("A")),
 *         CoroutineUtils.WaitForSeconds(2),
 *         CoroutineUtils.Do(() => Debug.Log("B"))));
 */
    public IEnumerator Chain(params IEnumerator[] actions) {
        foreach (IEnumerator action in actions) {
            yield return StartCoroutine(action);
        }
    }

    public IEnumerator Wait(float time) {
        yield return new WaitForSeconds(time);
    }

    public IEnumerator Do(Action action) {
        action();
        yield return 0;
    }

    public IEnumerator Wait(OnCondition checkCondition) {
        if(checkCondition == null) { yield break; }

        while (!checkCondition()) {
            yield return null;
        }
    }

    public int GetChapter() { return chapter; }

}
