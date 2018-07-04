using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AtomInfo", menuName = "Matter/AtomInfo", order = 3)]
public class AtomInfo : ScriptableObject, IComparable<AtomInfo> {

    public enum AtomCategory {
        UNKNOWN,
        ALKALI_METAL,
        ALKALINE_EARTH_METAL,
        TRANSITION_METAL,
        POST_TRANSTION_METAL,
        METALLOID,
        LANTHANIDE,
        ACTINIDE,
        NON_METAL,
        HALOGEN,
        NOBLE_GAS,
    }

    [SerializeField] private Atom atom;
    [TextArea(3,10)]
    [SerializeField] private string description;    // Summary of Atom
    [SerializeField] private Sprite image;          // Image describing atom
    [SerializeField] private string[] placesToBeFound; // Where it can be found in the game
    [SerializeField] private string[] origins;      // Where the element came from
    [SerializeField] private string[] isotopes;     // Different varietes of the atom.

    [SerializeField] private AtomCategory category; 
    [SerializeField] private int group;             // Column 
    [SerializeField] private int period;            // Row 

    [SerializeField] private float weight; 
    [SerializeField] private float density;
    [SerializeField] private int neutrons;          // Protons and Electrons are Atomic Number

    [SerializeField] private float halfLife = 0f;   // Whether the atom is stable and its half-life
    private static readonly float minStableLife = 525600f; // 1 year
    private static readonly float maxStableLife = 525600000f; // 1000 years

    [SerializeField] private bool isDiscovered = false;


    public AtomInfo(Atom atom) {
        this.atom = atom;
    }

    // All values
    public AtomInfo(Atom atom, Sprite image, string description,
        string[] placesToBeFound, string[] origins, string[] isotopes,
        AtomCategory category, int group, int period,
        int neutrons, float weight, float density, float halfLife = 0f) {

        this.atom = atom;
        this.image = image;
        this.description = description;

        this.placesToBeFound = placesToBeFound;
        this.origins = origins;
        this.isotopes = isotopes;

        this.category = category;
        this.group= group;
        this.period= period;

        this.neutrons = neutrons;

        this.weight = weight;
        this.density = density;
        this.halfLife = halfLife;
    }

    // User Created
    public AtomInfo(Atom atom, AtomCategory category, int group, int period,
        int neutrons, float density, float halfLife = 0f) {

        this.atom = atom;
        //this.image = image; Default Sprite
        //this.description = description; Default Summary

        this.placesToBeFound = new string[] { "Labratory" };
        this.origins = new string[] { "Unknown", "Man-Made" };

        string tempIso = atom.GetAbbreviation() + "" + (atom.GetAtomicNumber() + neutrons) + " (Synthesized)";
        this.isotopes = new string[] { tempIso };

        this.category = category;
        this.group = group;
        this.period = period;

        this.neutrons = neutrons;

        this.weight = atom.GetAtomicNumber() + neutrons;
        this.density = density;
        this.halfLife = halfLife;
    }

    public void Reset() {
        isDiscovered = false;
    }

    public void SetIsDiscovered(bool value) { isDiscovered = value; }

    public Atom GetAtom() {  return atom;}
    public Sprite GetImage() { return image; }

    public string GetDescription() { return description; }

    public string[] GetPlacesToBeFound() { return placesToBeFound; }
    public string[] GetOrigins() { return origins; }
    public string[] GetIsotopes() { return isotopes; }

    public AtomCategory GetCategory() { return category; }
    public int GetPeriod() { return period; }
    public int GetGroup() { return group; }

    public int GetProtons() { return atom.GetAtomicNumber(); }
    public int GetElectrons() { return atom.GetAtomicNumber(); }
    public int GetNeutrons() { return neutrons; }

    public float GetWeight() { return weight; }
    public float GetDensity() { return density; }
    public float GetHalfLife() { return halfLife; }
    public bool IsStable() { return halfLife == 0f; }

    public bool IsMetal() { return !(category == AtomCategory.HALOGEN || category == AtomCategory.NOBLE_GAS || category == AtomCategory.NON_METAL); }
    public bool IsDiscovered() { return isDiscovered; }

    public string GetCategoryString() {
        string rtnStr = "";
        switch (category) {
            case AtomCategory.UNKNOWN:
                rtnStr = "Unknown";
                break;
            case AtomCategory.ALKALI_METAL:
                rtnStr = "Alkali Metal";
                break;
            case AtomCategory.ALKALINE_EARTH_METAL:
                rtnStr = "Alkaline Earth Metal";
                break;
            case AtomCategory.TRANSITION_METAL:
                rtnStr = "Transition Metal";
                break;
            case AtomCategory.POST_TRANSTION_METAL:
                rtnStr = "Post Transition Metal";
                break;
            case AtomCategory.METALLOID:
                rtnStr = "Metalloid";
                break;
            case AtomCategory.LANTHANIDE:
                rtnStr = "Lanthanide";
                break;
            case AtomCategory.ACTINIDE:
                rtnStr = "Actinide";
                break;
            case AtomCategory.NON_METAL:
                rtnStr = "Non Metal";
                break;
            case AtomCategory.HALOGEN:
                rtnStr = "Halogen";
                break;
            case AtomCategory.NOBLE_GAS:
                rtnStr = "Noble Gas";
                break;
        }
        return rtnStr;
    }
    public Color GetCategoryColor() {
        Color c = Color.white;
        switch (category) {
            case AtomCategory.UNKNOWN:
                c = Color.grey;
                break;
            case AtomCategory.ALKALI_METAL:
                c = Color.red; // Dark Red
                c.g = .2f;
                c.b = .2f;
                break;
            case AtomCategory.ALKALINE_EARTH_METAL:
                c = Color.red; // Pale Red
                c.g = .5f;
                c.b = .5f;
                break;
            case AtomCategory.TRANSITION_METAL:
                c = Color.cyan;
                break;
            case AtomCategory.POST_TRANSTION_METAL:
                c = new Color(0f, .5f, 1f); // Light Blue
                break;
            case AtomCategory.METALLOID:
                c = new Color(.8f, .05f, 1f); // Purple
                break;
            case AtomCategory.LANTHANIDE:
                c = new Color(.6f, .61f, 1f); // 151, 161, 255 Pale Blue
                break;
            case AtomCategory.ACTINIDE:
                c = new Color(.3f, 1f, .27f); // 79, 255, 69 Green
                break;
            case AtomCategory.NON_METAL:
                c = new Color(.7f, .3f, .3f); // Brown
                break;
            case AtomCategory.HALOGEN:
                c = Color.Lerp(Color.red, Color.yellow, .5f); // Orange
                break;
            case AtomCategory.NOBLE_GAS:
                c = Color.yellow;
                break;
        }
        return c;
    }

    public int CompareTo(AtomInfo other) {
        return atom.CompareTo(other.atom);
    }

    public static float GetStability(float halflife) {
        return Mathf.Clamp01((halflife - minStableLife) / (maxStableLife - minStableLife));
    }
}
