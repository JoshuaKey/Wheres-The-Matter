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

    [SerializeField] private Atom[] atoms;
    [SerializeField] private AtomData[] atomData;
    [SerializeField] private AtomInfo[] atomInfo;

    [SerializeField] private Craftable[] craftables;

    // Should this be a Struct?
    public void Init() {
        // Check for Load???

        for(int i = 0; i < atomData.Length; i++) {
            atomData[i].Reset();
            atomInfo[i].Reset();
        }
        Array.Sort(atoms);
        Array.Sort(atomData);
        Array.Sort(atomInfo);
    }

    public Atom FindAtom(int atomicNumber) {

        if (atomicNumber < 1 || atomicNumber > atomData.Length) { // Atomic Number is 1 based
            Debug.Log("Could not find Atom [" + atomicNumber + "]");
            return null;
        }
        return atoms[atomicNumber - 1];
    }
    public Atom FindAtom(string name) {
        for (int i = 0; i < atomData.Length; i++) {
            if (name == atoms[i].GetName()) {
                return atoms[i];
            }
        }
        Debug.Log("Could not find Atom [" + name + "]");
        return null;
    }

    public AtomData FindAtomData(int atomicNumber) {
        
        if(atomicNumber < 1 || atomicNumber > atomData.Length) { // Atomic Number is 1 based
            Debug.Log("Could not find Atom [" + atomicNumber + "]");
            return null;
        }
        return atomData[atomicNumber-1];
    }
    public AtomData FindAtomData(string name) {
        for(int i = 0; i < atomData.Length; i++) {
            if(name == atomData[i].GetAtom().GetName()) {
                return atomData[i];
            }
        }
        Debug.Log("Could not find Atom [" + name + "]");
        return null;
    }

    public AtomInfo FindAtomInfo(int atomicNumber) {

        if (atomicNumber < 1 || atomicNumber > atomData.Length) { // Atomic Number is 1 based
            Debug.Log("Could not find Atom [" + atomicNumber + "]");
            return null;
        }
        return atomInfo[atomicNumber - 1];
    }
    public AtomInfo FindAtomInfo(string name) {
        for (int i = 0; i < atomData.Length; i++) {
            if (name == atomData[i].GetAtom().GetName()) {
                return atomInfo[i];
            }
        }
        Debug.Log("Could not find Atom [" + name + "]");
        return null;
    }

    public Craftable GetCraftable(int i) { return craftables[i]; }

    public int GetAtomAmount() { return atomData.Length; }
    public int GetCraftableAmount() { return craftables.Length; }

    public Atom GetUknown() { return unknownInfo.GetAtom(); }
    public AtomInfo GetUknownInfo() { return unknownInfo; }
    public AtomData GetUknownData() { return unknownData; }

}