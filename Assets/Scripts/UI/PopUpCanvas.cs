using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpCanvas : MonoBehaviour {

    [SerializeField] private AtomDiscovery atomDiscoveryPrefab;

    private List<AtomDiscovery> popups = new List<AtomDiscovery>();

    public void Popup(Atom a) {
        for(int i = 0; i < popups.Count; i++) {
            if (!popups[i].gameObject.activeSelf) {
                popups[i].Setup(a);
                return;
            }
        }

        AtomDiscovery atomDiscovery = Instantiate(atomDiscoveryPrefab, this.transform);
        atomDiscovery.Setup(a);
        popups.Add(atomDiscovery);
    }
}
