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

    [Header("Chapter 0")]
    [SerializeField] private Sprite introImage;

    [Header("Chapter 1")]
    [SerializeField] private Sprite letterImage;
    [SerializeField] private string letterText;
    [SerializeField] private float letterHeight;
    [SerializeField] private float letterAnimateTime;

    [Header("Professor Info")]
    [SerializeField] private string professorName;
    [SerializeField] private Sprite professorImage;

    [Header("Map Unlockables")]
    [SerializeField] private MapUI mapUI;
    [SerializeField] private Craftable mineUnlockable;
    [SerializeField] private Craftable beachUnlockable;
    [SerializeField] private Craftable oceanUnlockable;
    [SerializeField] private Craftable desertUnlockable;
    [SerializeField] private Craftable townUnlockable;

    private void Start() {
        ProgressStory();

        Game.Instance.playerData.OnCraftableProduced += CheckMap;
        Game.Instance.playerData.OnCraftableSold += CheckMap;
        Game.Instance.playerData.OnMoneyChange += CheckWin;
    }

    public void CheckMap(Craftable c, float amo) {
        if(c == mineUnlockable) {
            mapUI.UnlockArea(World.AreaType.MINE, Game.Instance.playerData.GetCraftableAmount(c) > 0);
        } else if(c == beachUnlockable) {
            mapUI.UnlockArea(World.AreaType.COAST, Game.Instance.playerData.GetCraftableAmount(c) > 0);
        } else if (c == desertUnlockable) {
            mapUI.UnlockArea(World.AreaType.DESERT, Game.Instance.playerData.GetCraftableAmount(c) > 0);
        } else if (c == oceanUnlockable) {
            mapUI.UnlockArea(World.AreaType.OCEAN, Game.Instance.playerData.GetCraftableAmount(c) > 0);
        } else if (c == townUnlockable) {
            mapUI.UnlockArea(World.AreaType.TOWN, Game.Instance.playerData.GetCraftableAmount(c) > 0);
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
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;
            case 7:
                break;
        }
    }

    private void Chapter0() {
        // Like a chain of events

        // Disable all UI buttons, Menu, Laboratory
        // Display Background
        // Animate a little
        // Some sounds
        // Delay

        // Display Knock Dialogue
        // Play Knock sound
        // Wait

        // Play paper slip sound
        // Delay

        // Display Letter
        // Animate down
        // Wait

        // Display Letter Dialogue
        // Wait

        // Fade out
        // Fade in Personal Room
        // Set stats to max, Weight to 1.
        // Activate player controls
        // Display Hydogren Tree Dialogue
        // Wait

        // Wait for Player actions to finish (Collect)

        // Display end Hydrogen Tree Dialogue
        // Wait

        // Unlock Menu and Crafting
        // Wait for players actions, (Crafting and Placing)

        // Display Tree Craft Dialogue
        // Wait

        // Unlock forest, Map and Resume
        // Wait for players actions (To Forest) -> Add event for changing location




        //Game.Instance.background.image.sprite = introImage;
        //Game.Instance.DisplayBackground();

        //StartCoroutine(DelayAction(5.0f, () => {
        //    Game.Instance.player.CanCollect(false); // Disable Players
        //    Game.Instance.player.CanEscape(false);
        //    var dialogue = Game.Instance.dialogueSystem;
        //    dialogue.SetCharacterName("???");
        //    dialogue.SetCharacterImage(Game.Instance.gameData.GetUknownInfo().GetImage());
        //    dialogue.QueueDialogue("*Knock, knock*", true);
        //    // Play Knock Noise
        //    // Then play paper noise...
        //    dialogue.OnDialogueEnd += ProgressStory;
        //}));
        //chapter = 1;
    }
    private void Chapter1() {


        //var dialogue = Game.Instance.dialogueSystem;
        //dialogue.OnDialogueEnd -= ProgressStory;

        //// Display Letter
        //Game.Instance.background.image.sprite = letterImage;
        //Game.Instance.background.text.text = letterText;
        //Game.Instance.DisplayBackground();

        // Animate Letter...
        //Event event1 = new Event();
        //event1.onAction += () => {
        //    var pos = Game.Instance.background.transform.position;
        //    pos.y += letterHeight;

        //    Chain(
        //        AnimatePosition(10f, pos, Game.Instance.background.transform),
                
        //        );

        //    //StartCoroutine(AnimatePosition(10f, pos, Game.Instance.background.transform));
        //};

        //Event event2 = new Event();
        //event2.


    }

    //private void OnCompareFloat(float value) { // Compares 
    //    if (value >= comparisionCondition) {
    //        ProgressStory();
    //    }
    //}
    //private void OnCompareAtom(Atom a, float value) {
    //    if (value >= comparisionCondition) {
    //        ProgressStory();
    //    }
    //}
    //private void OnCompareAtom(Craftable c, float value) {
    //    if (value >= comparisionCondition) {
    //        ProgressStory();
    //    }
    //}
    //private void OnEvent() {

    //}

    // Make these into like little structs with bools and evaluations...

    private IEnumerator DelayAction(float time, System.Action actionDelegate) {
        float endTime = Time.time + time;
        while(Time.time < endTime) {
            yield return null;
        }

        actionDelegate.Invoke();
    }

    private IEnumerator AnimatePosition(float time, Vector3 pos, Transform trans) {
        float endTime = Time.time + time;
        Vector3 startPos = trans.position;

        while (Time.time < endTime) {
            float t = 1 - (endTime - Time.time) / time;

            var newPos = Vector3.Lerp(startPos, pos, t);
            trans.position = newPos;

            yield return null;
        }

        trans.position = pos;
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

    /**
     * Usage: StartCoroutine(CoroutineUtils.DelaySeconds(action, delay))
     * For example:
     *     StartCoroutine(CoroutineUtils.DelaySeconds(
     *         () => DebugUtils.Log("2 seconds past"),
     *         2);
     */
    public IEnumerator DelaySeconds(Action action, float delay) {
        yield return new WaitForSeconds(delay);
        action();
    }

    public IEnumerator WaitForSeconds(float time) {
        yield return new WaitForSeconds(time);
    }

    public IEnumerator Do(Action action) {
        action();
        yield return 0;
    }
}
