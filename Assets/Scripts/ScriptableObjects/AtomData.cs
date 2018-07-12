using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct AtomAmo {
    public Atom atom;
    public int amo;
}

[CreateAssetMenu(fileName = "AtomData", menuName = "Matter/AtomData", order= 2)]
public class AtomData : ScriptableObject, IComparable<AtomData> {

    [SerializeField] private Atom atom;

    [SerializeField] private int currAmo = 0;
    [SerializeField] private float passiveGain = 0f;
    [SerializeField] private int totalCollected;
    [SerializeField] private int totalUsed;

    public AtomData(Atom atom) {
        this.atom = atom;
    }

    public void Gain(int amo) {
        currAmo += amo;
        totalCollected += amo;
    }
    public void Lose(int amo) {
        currAmo -= amo;
        totalUsed += amo;
    }
    public void Reset() {
        currAmo = 0;
        passiveGain = 0f;

        totalUsed = 0;
        totalCollected = 0;
    }

    public Atom GetAtom() { return atom; }
    public int GetCurrAmo() { return currAmo; }
    public float GetPassiveGain() { return passiveGain; }
    public int GetTotalCollected() { return totalCollected; }
    public float GetTotalUsed() { return totalUsed; }

    public int CompareTo(AtomData other) {
        return atom.CompareTo(other.atom);
    }

    
    public static AtomData CreateNewAtomData(Atom a) {
        AtomData data = ScriptableObject.CreateInstance<AtomData>();
        data.atom = a;
        return data;
    }
    
}
