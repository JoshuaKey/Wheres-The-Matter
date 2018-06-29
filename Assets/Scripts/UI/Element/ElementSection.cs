using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ElementSection : MonoBehaviour {

    [SerializeField] private ElementDisplay elementDisplayPrefab;
    private List<ElementDisplay> elementDisplays = new List<ElementDisplay>();

    private void Start() {
        RectTransform rect = GetComponent<RectTransform>();
        float width = rect.rect.width;
        float height = rect.rect.height;

        float boxSize = Mathf.Min(Mathf.FloorToInt(width / 18), Mathf.FloorToInt(height / 9));
        Vector2 size = new Vector2(boxSize, boxSize);
        int amo = Game.Instance.gameData.GetAtomAmount();
        for(int i = 0; i < amo; i++) {
            CreateChild(Game.Instance.gameData.FindAtomData(i + 1).GetAtom(), size);
        }
    }

    private void CreateChild(Atom atom, Vector2 size) {
        // Instantiate
        ElementDisplay display = Instantiate(elementDisplayPrefab, this.transform);
        display.atom = atom;

        elementDisplays.Add(display);

        RectTransform child = display.GetComponent<RectTransform>();

        // Grid Pos
        AtomInfo info = Game.Instance.gameData.FindAtomInfo(atom.GetAtomicNumber());
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
    }

    public void Refresh() {
        for(int i = 0; i < elementDisplays.Count; i++) {
            elementDisplays[i].SetDisplay();
        }
    }

}
