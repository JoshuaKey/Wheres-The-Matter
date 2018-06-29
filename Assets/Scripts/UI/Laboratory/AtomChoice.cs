using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AtomChoice : MonoBehaviour {

    public Atom atom;
    public CombineUI combineUI;
    public SplitUI splitUI;
    [SerializeField] public Button trigger;
    [SerializeField] public TextMeshProUGUI text;
    [SerializeField] AudioClip choiceClickSound;


    public void Start() {
        this.gameObject.name = atom.GetName() + "Choice";
        SetDisplay();
        SetEvents();
    }

    public void SetDisplay() {
        //AtomInfo info = Game.Instance.gameData.FindAtomInfo(atom.GetAtomicNumber());
        AtomData data = Game.Instance.gameData.FindAtomData(atom.GetAtomicNumber());

        text.text = atom.GetName() + "\n<size=80%> Atomic Number: " + atom.GetAtomicNumber() + " Curr Amo: " + data.GetCurrAmo();
    }

    public void SetEvents() {
        trigger.onClick.RemoveAllListeners();
        trigger.onClick.AddListener(() => {
            if(combineUI != null) { combineUI.SetAtom(atom); }
            if (splitUI != null) { splitUI.SetAtom(atom); }
            AudioManager.Instance.PlaySound(choiceClickSound);
        });
    }

}
