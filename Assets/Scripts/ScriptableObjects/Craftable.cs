using UnityEngine;

[CreateAssetMenu(fileName = "Craftable", menuName = "Matter/Craftable")]
public class Craftable : ScriptableObject {

    [SerializeField] private Sprite sprite;
    [SerializeField] private new string name;
    [SerializeField] float price;
    [SerializeField] private AtomAmo[] atomsForProductions;

    public Sprite GetSprite() { return sprite; }
    public AtomAmo[] GetAtomsForProduction() { return atomsForProductions; }
    public string GetName() { return name; }
    public float GetPrice() { return price; }

    public static Craftable CreateNewBlock(Atom a) {
        Craftable c = ScriptableObject.CreateInstance<Craftable>();

        c.name = "Block of " + a.GetName();
        c.price = a.GetAtomicNumber();

        c.atomsForProductions = new AtomAmo[1];
        c.atomsForProductions[0].atom = a;
        c.atomsForProductions[0].amo = 1000000000;

        c.sprite = Game.Instance.gameData.GetUknownInfo().GetImage();

        return c;
    }

}
