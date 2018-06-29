using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapUI : MonoBehaviour {

    public void LoadArea(int type) {
        World.AreaType area = (World.AreaType)type;

        Game.Instance.LoadArea(area);
        Game.Instance.DisplayGame();
    }

}
