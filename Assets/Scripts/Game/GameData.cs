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
    public Atom maxAtom;

    [SerializeField] private List<Craftable> craftables;

    public delegate void OnCraftableChange(Craftable atom, float value);
    public delegate void OnAtomChange(Atom atom, float value);

    public event OnAtomChange OnAtomAdd;
    public event OnAtomChange OnAtomUse;
    public event OnAtomChange OnAtomDiscover;
    public event OnCraftableChange OnCraftableDiscover;

    // Should this be a Struct?
    public void Init() { 
        for(int i = 0; i < atomData.Count; i++) {
            atomData[i].Reset();
            atoms[i].Reset();
        }
        atoms.Sort();
        atomData.Sort();
        atomInfo.Sort();
    }

    public void Save(SaveData s) {
        SerializeableAtom atomName = new SerializeableAtom();

        for (int i = 0; i < atomData.Count; i++) {
            atomName.abbr = atoms[i].GetAbbreviation();
            atomName.name = atoms[i].GetName();
            atomName.hasBeenRenamed = atoms[i].HasBeRenamed();

            s.atomNames.Add(atomName);

            s.atomData.Add(atomData[i].data);
        }
        s.maxAtom = maxAtom;
    }
    public void Load(SaveData s) {
        for (int i = 0; i < atomData.Count; i++) {
            atoms[i].Init(s.atomNames[i]);
            atomData[i].Init(s.atomData[i]);
        }
        maxAtom = s.maxAtom;

        for (int i = 118; i < atomData.Count; i++) {
            if (atomData[i].IsDiscovered()) {
                var craftable = Craftable.CreateNewBlock(atomData[i].GetAtom());
                Game.Instance.gameData.AddCraftable(craftable);
            }
        }
    }

    public void Absorb(Atom atom, int amo) {
        if (amo == 0) { return; }

        AtomData data = FindAtomData(atom.GetAtomicNumber());
        if (data == null) { return; }

        if (!data.IsDiscovered()) {
            data.SetIsDiscovered(true);
            if(OnAtomDiscover != null) {
                OnAtomDiscover(atom, amo);
            }

            if(maxAtom == null || atom.GetAtomicNumber() > maxAtom.GetAtomicNumber()) {
                maxAtom = atom;
            }
        }
        if(amo + data.GetCurrAmo() < data.GetCurrAmo()) {
            amo = int.MaxValue - data.GetCurrAmo();
            if (amo == 0) { return; }
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

    public Craftable FindCraftable(string name) {
        return craftables.Find((x) => {
            return x.GetName() == name;
        });
    }

    public Craftable GetCraftable(int i) { return craftables[i]; }

    public int GetAtomAmount() { return atomData.Count; }
    public int GetCraftableAmount() { return craftables.Count; }

    public Atom GetUknown() { return unknownInfo.GetAtom(); }
    public AtomInfo GetUknownInfo() { return unknownInfo; }
    public AtomData GetUknownData() { return unknownData; }

    public Atom GetMaxAtom() {
        return maxAtom;
    }

}