using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ElementSection : MonoBehaviour {

    [SerializeField] private RectTransform elementDiscoveryParent;

    [SerializeField] private ElementDisplay elementDisplayPrefab;
    [SerializeField] private RectTransform elementDiscoveryPrefab;

    private List<ElementDisplay> elementDisplays = new List<ElementDisplay>();

    private void Start() {
        RectTransform rect = GetComponent<RectTransform>();
        float width = rect.rect.width;
        float height = rect.rect.height;

        float boxSize = Mathf.Min(Mathf.FloorToInt(width / 18), Mathf.FloorToInt(height / 9));
        Vector2 size = new Vector2(boxSize, boxSize);
        int amo = Game.Instance.gameData.GetAtomAmount();
        for(int i = 0; i < amo && i < 118; i++) {
            CreateChild(Game.Instance.gameData.FindAtomData(i + 1).GetAtom(), size);
        }

        for (int i = 118; i < amo; i++) {
            CreateChildPage2(Game.Instance.gameData.FindAtomData(i + 1).GetAtom(), size);
        }

        Game.Instance.gameData.OnAtomDiscover += (x, y) => {
            Refresh();
        };
    }

    private void CreateChild(Atom atom, Vector2 size) {
        AtomInfo info = Game.Instance.gameData.FindAtomInfo(atom.GetAtomicNumber());
        if(info == null || (info.GetGroup() == 0 && info.GetPeriod() == 0)) { return; }

        // Instantiate
        ElementDisplay display = Instantiate(elementDisplayPrefab, this.transform);
        display.atom = atom;

        elementDisplays.Add(display);

        RectTransform child = display.GetComponent<RectTransform>();

        // Grid Pos 
        int gridX = info.GetGroup() - 1;
        int gridY = info.GetPeriod() - 1;

        // Pivot
        child.pivot = Vector2.up;

        // Anchor
        child.anchorMax = Vector2.up;
        child.anchorMin = Vector2.up;

        // Width and Height
        child.sizeDelta = size;

        // LocalPosition - Grid(0, 0)
        float x = size.x * gridX;
        float y = -size.y * gridY;
        child.anchoredPosition3D = new Vector3(x, y, .0f);

        var elementDiscovery = Instantiate(elementDiscoveryPrefab, elementDiscoveryParent);
        display.SetExclamationImage(elementDiscovery);
    }

    private void CreateChildPage2(Atom atom, Vector2 size) {
        AtomInfo info = Game.Instance.gameData.FindAtomInfo(atom.GetAtomicNumber());
        if (info == null || (info.GetGroup() == 0 && info.GetPeriod() == 0)) { return; }

        // Instantiate
        ElementDisplay display = Instantiate(elementDisplayPrefab, this.transform);
        display.atom = atom;

        elementDisplays.Add(display);

        RectTransform child = display.GetComponent<RectTransform>();

        // Grid Pos 
        int gridX = info.GetGroup() - 1;
        int gridY = info.GetPeriod() - 1;

        // Pivot
        child.pivot = Vector2.up;

        // Anchor
        child.anchorMax = Vector2.up;
        child.anchorMin = Vector2.up;

        // Width and Height
        child.sizeDelta = size;

        // LocalPosition - Grid(0, 0)
        float x = size.x * gridX;
        float y = -size.y * gridY - size.y*2.4f;
        child.anchoredPosition3D = new Vector3(x, y, .0f);

        var elementDiscovery = Instantiate(elementDiscoveryPrefab, elementDiscoveryParent);
        display.SetExclamationImage(elementDiscovery);
    }

    public void ElementClick(Atom atom) {
        elementDisplays[atom.GetAtomicNumber() - 1].MakeOld();
    }

    public void Refresh() {
        for(int i = 0; i < elementDisplays.Count; i++) {
            elementDisplays[i].SetDisplay();
        }
    }

}
