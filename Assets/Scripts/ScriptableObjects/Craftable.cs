﻿using UnityEngine;

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

}
