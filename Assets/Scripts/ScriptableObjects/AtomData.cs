using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct AtomAmo {
    public Atom atom;
    public int amo;
}

[Serializable]
public struct SerializableAtomData {
    [SerializeField] public int currAmo;
    [SerializeField] public float passiveGain;
    [SerializeField] public int totalCollected;
    [SerializeField] public int totalUsed;
    [SerializeField] public bool isDiscovered;
}

[CreateAssetMenu(fileName = "AtomData", menuName = "Matter/AtomData", order= 2)]
public class AtomData : ScriptableObject, IComparable<AtomData> {

    [SerializeField] private Atom atom;

    //[SerializeField] private int currAmo = 0;
    //[SerializeField] private float passiveGain = 0f;
    //[SerializeField] private int totalCollected;
    //[SerializeField] private int totalUsed;

    //[SerializeField] private bool isDiscovered = false;
    public SerializableAtomData data;

    public void Init(SerializableAtomData atomData) {
        data.currAmo = atomData.currAmo;
        data.passiveGain = atomData.passiveGain;
        data.totalCollected = atomData.totalCollected;
        data.totalUsed = atomData.totalUsed;
        data.isDiscovered = atomData.isDiscovered;
    }

    public void Gain(int amo) {
        data.currAmo += amo;
        data.totalCollected += amo;
    }
    public void Lose(int amo) {
        data.currAmo -= amo;
        data.totalUsed += amo;
    }
    public void Reset() {
        data.currAmo = 0;
        data.passiveGain = 0f;

        data.isDiscovered = false;

        data.totalUsed = 0;
        data.totalCollected = 0;
    }

    public Atom GetAtom() { return atom; }
    public int GetCurrAmo() { return data.currAmo; }
    public float GetPassiveGain() { return data.passiveGain; }
    public int GetTotalCollected() { return data.totalCollected; }
    public float GetTotalUsed() { return data.totalUsed; }

    public void SetIsDiscovered(bool value) { data.isDiscovered = value; }
    public bool IsDiscovered() { return data.isDiscovered; }

    public int CompareTo(AtomData other) {
        return atom.CompareTo(other.atom);
    }
    
    public static AtomData CreateNewAtomData(Atom a) {
        AtomData data = ScriptableObject.CreateInstance<AtomData>();
        data.atom = a;
        return data;
    }
    
}
