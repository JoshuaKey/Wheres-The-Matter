using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class SaveData  { // Holds all Save Data

    // Game Data
    public float money;
    public float time;
    public string lastSave;
    public Atom maxAtom;
    public int storyChapter;

    // World Data
    public Vector3 playerPosition;
    public int worldSeed;
    public World.AreaType worldArea;

    // Player Data
    public List<Craftable> craftables = new List<Craftable>();
    public List<int> craftableAmo = new List<int>();
    public List<PlayerData.UpgradeData> upgradeData = new List<PlayerData.UpgradeData>();

    // Atom Data
    public List<SerializeableAtom> atomNames = new List<SerializeableAtom>();
    public List<SerializableAtomData> atomData = new List<SerializableAtomData>();

}