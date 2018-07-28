using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapUI : MonoBehaviour {

    [SerializeField] public RectTransform rect;
    [SerializeField] private Button forestButton;
    [SerializeField] private Button beachButton;
    [SerializeField] private Button mineButton;
    [SerializeField] private Button oceanButton;
    [SerializeField] private Button desertButton;
    [SerializeField] private Button townButton;

    public void LoadArea(int type) {
        World.AreaType area = (World.AreaType)type;

        Game.Instance.LoadArea(area);
        Game.Instance.DisplayGame();
    }

    public void UnlockArea(World.AreaType type, bool unlocked) {
        switch (type) {
            case World.AreaType.FOREST:
                forestButton.interactable = unlocked;
                break;
            case World.AreaType.MINE:
                mineButton.interactable = unlocked;
                break;
            case World.AreaType.BEACH:
                beachButton.interactable = unlocked;
                break;
            case World.AreaType.OCEAN:
                oceanButton.interactable = unlocked;
                break;
            case World.AreaType.DESERT:
                desertButton.interactable = unlocked;
                break;
            case World.AreaType.TOWN:
                townButton.interactable = unlocked;
                break;
        }
    }

}
