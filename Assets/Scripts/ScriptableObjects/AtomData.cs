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

    public AtomData(Atom atom) {
        this.atom = atom;
    }

    public void Gain(int amo) {
        currAmo += amo;
    }
    public void Lose(int amo) {
        currAmo -= amo;
    }
    public void Reset() {
        currAmo = 0;
        passiveGain = 0f;
    }

    public Atom GetAtom() { return atom; }
    public int GetCurrAmo() { return currAmo; }
    public float GetPassiveGain() { return passiveGain; }

    public int CompareTo(AtomData other) {
        return atom.CompareTo(other.atom);
    }
}
