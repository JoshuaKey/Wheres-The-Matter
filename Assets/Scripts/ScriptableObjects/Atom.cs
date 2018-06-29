using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public Atom(string name, string abbreviation, int atomicNumber) {
        this.name = name;
        this.abbreviation = abbreviation;
        this.atomicNumber = atomicNumber;
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
}

