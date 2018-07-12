using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class GameData  {
    // Game Data is responsible for holding data relevant to the game. Components should b able to access this data to format and display appropraitely.

    [SerializeField] private AtomInfo unknownInfo;
    [SerializeField] private AtomData unknownData;

    [SerializeField] private List<Atom> atoms;
    [SerializeField] private List<AtomData> atomData;
    [SerializeField] private List<AtomInfo> atomInfo;

    [SerializeField] private List<Craftable> craftables;

    public delegate void OnCraftableChange(Craftable atom, float value);
    public delegate void OnAtomChange(Atom atom, float value);

    public event OnAtomChange OnAtomAdd;
    public event OnAtomChange OnAtomUse;
    public event OnAtomChange OnAtomDiscover;
    public event OnCraftableChange OnCraftableDiscover;

    // Should this be a Struct?
    public void Init() {
        // Check for Load???

        for(int i = 0; i < atomData.Count; i++) {
            atomData[i].Reset();
            atomInfo[i].Reset();
            atoms[i].Reset();
        }
        atoms.Sort();
        atomData.Sort();
        atomInfo.Sort();
    }

    public void Absorb(Atom atom, int amo) {
        if (amo == 0) { return; }

        AtomData data = FindAtomData(atom.GetAtomicNumber());
        AtomInfo info = FindAtomInfo(atom.GetAtomicNumber());
        if (data == null) { return; }


        if (!info.IsDiscovered()) {
            info.SetIsDiscovered(true);
            if(OnAtomDiscover != null) {
                OnAtomDiscover(atom, amo);
            }
        }

        data.Gain(amo);
        if (OnAtomAdd != null) {
            OnAtomAdd(atom, amo);
        }
    }
    public void Use(Atom atom, int amo) {
        if (amo == 0) { return; }

        AtomData data = FindAtomData(atom.GetAtomicNumber());
        if (data == null) { return; }

        data.Lose(amo);
        if (OnAtomUse != null) {
            OnAtomUse(atom, amo);
        }
    }

    public void AddCraftable(Craftable c) {
        craftables.Add(c);
        if (OnCraftableDiscover != null) {
            OnCraftableDiscover(c, 0);
        }
    }

    public Atom FindAtom(int atomicNumber) {

        if (atomicNumber < 1 || atomicNumber > atomData.Count) { // Atomic Number is 1 based
            Debug.Log("Could not find Atom [" + atomicNumber + "]");
            return null;
        }
        return atoms[atomicNumber - 1];
    }
    public Atom FindAtom(string name) {
        for (int i = 0; i < atomData.Count; i++) {
            if (name == atoms[i].GetName()) {
                return atoms[i];
            }
        }
        Debug.Log("Could not find Atom [" + name + "]");
        return null;
    }

    public AtomData FindAtomData(int atomicNumber) {
        
        if(atomicNumber < 1 || atomicNumber > atomData.Count) { // Atomic Number is 1 based
            Debug.Log("Could not find Atom [" + atomicNumber + "]");
            return null;
        }
        return atomData[atomicNumber-1];
    }
    public AtomData FindAtomData(string name) {
        for(int i = 0; i < atomData.Count; i++) {
            if(name == atomData[i].GetAtom().GetName()) {
                return atomData[i];
            }
        }
        Debug.Log("Could not find Atom [" + name + "]");
        return null;
    }

    public AtomInfo FindAtomInfo(int atomicNumber) {

        if (atomicNumber < 1 || atomicNumber > atomData.Count) { // Atomic Number is 1 based
            Debug.Log("Could not find Atom [" + atomicNumber + "]");
            return null;
        }
        return atomInfo[atomicNumber - 1];
    }
    public AtomInfo FindAtomInfo(string name) {
        for (int i = 0; i < atomData.Count; i++) {
            if (name == atomData[i].GetAtom().GetName()) {
                return atomInfo[i];
            }
        }
        Debug.Log("Could not find Atom [" + name + "]");
        return null;
    }

    public Craftable GetCraftable(int i) { return craftables[i]; }

    public int GetAtomAmount() { return atomData.Count; }
    public int GetCraftableAmount() { return craftables.Count; }

    public Atom GetUknown() { return unknownInfo.GetAtom(); }
    public AtomInfo GetUknownInfo() { return unknownInfo; }
    public AtomData GetUknownData() { return unknownData; }

}