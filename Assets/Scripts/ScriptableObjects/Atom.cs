using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SerializeableAtom {
    public bool hasBeenRenamed;
    public string name;
    public string abbr;
}

/// <summary>
/// Atom is a simple object containing the bare minimums of an Atom.
/// <para></para>
/// This class should be used for comparision, look up, search, etc. of stored data.
/// <para></para><seealso cref="AtomInfo">Educational info on the atom</seealso>
/// <para></para><seealso cref="AtomData">Game data on the atom</seealso>
/// </summary>
[CreateAssetMenu(fileName = "Atom", menuName = "Matter/Atom", order = 1)]
public class Atom : ScriptableObject, IComparable<Atom> {

    [SerializeField] private new string name;
    [SerializeField] private string abbreviation;
    [SerializeField] private int atomicNumber;

    [Header("Naming")]
    [SerializeField] private bool canBeRenamed = false;
    [SerializeField] private bool hasBeenRenamed = false;
    [SerializeField] private string originalName;
    [SerializeField] private string originalAbbr;

    public Atom(string name, string abbreviation, int atomicNumber) {
        this.name = name;
        this.abbreviation = abbreviation;
        this.atomicNumber = atomicNumber;
    }

    public void Rename(string name, string abbreviation) {
        if (canBeRenamed) {
            originalName = this.name;
            originalAbbr = this.abbreviation;

            this.name = name;
            this.abbreviation = abbreviation;
            hasBeenRenamed = true;
        }
    }
    public void Init(SerializeableAtom atomName) {
        if (canBeRenamed) {
            originalName = this.name;
            originalAbbr = this.abbreviation;

            this.name = atomName.name;
            this.abbreviation = atomName.abbr;
            hasBeenRenamed = atomName.hasBeenRenamed;
        }
    }
    public void Reset() {
        if (!canBeRenamed) {
            originalName = this.name;
            originalAbbr = this.abbreviation;
            return;
        }

        if (originalName != null && originalName != "") {
            this.name = originalName;
        } else {
            originalName = this.name;
        }
        if (originalAbbr != null && originalAbbr != "") {
            this.abbreviation = originalAbbr;
        } else {
            originalAbbr = this.abbreviation;
        }
        hasBeenRenamed = false;
    }

    public int CompareTo(Atom obj) {
        return atomicNumber.CompareTo(obj.atomicNumber);
    }
    public override string ToString() {
        return name + " (" + abbreviation + ") : " + atomicNumber;
    }
    public string GetName() { return name; }
    public int GetAtomicNumber() { return atomicNumber; }
    public string GetAbbreviation() { return abbreviation; }
    public bool CanBeRenamed() { return canBeRenamed && !hasBeenRenamed; }
    public bool HasBeRenamed() { return hasBeenRenamed; }

    public static Atom CreateNewAtom(string name, string abbreviation, int number) {
        Atom a = ScriptableObject.CreateInstance<Atom>();

        a.name = name;
        a.abbreviation = abbreviation;
        a.atomicNumber = number;

        a.originalName = name;
        a.originalAbbr = abbreviation;

        a.canBeRenamed = true;

        return a;
    }
}

