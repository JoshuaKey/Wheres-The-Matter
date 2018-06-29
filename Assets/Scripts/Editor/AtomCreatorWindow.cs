using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AtomCreatorWindow : EditorWindow {

    private int creationType = 0;

    [MenuItem("Window/Atom Creator")]
    public static void ShowWindow() {
        EditorWindow.GetWindow<AtomCreatorWindow>("Atom Creator");
    }

    private void OnGUI() {
        GUILayout.Label("Creation Type", EditorStyles.boldLabel);

        creationType = GUILayout.SelectionGrid(creationType, new string[] { "Atom", "AtomInfo", "AtomData" }, 3);

        switch (creationType) {
            case 0:
                AtomGUI();
                break;
            case 1:
                AtomInfoGUI();
                break;
            case 2:
                AtomDataGUI();
                break;
        }

    }
    
    new string name;
    string abbreviation;
    int number;
    public void AtomGUI() {
        EditorGUILayout.Separator();
        name = EditorGUILayout.TextField("Name: ", name);

        abbreviation = EditorGUILayout.TextField("Abbreviation: ", abbreviation);

        number = EditorGUILayout.IntField("Atomic Number: ", number);

        EditorGUILayout.Separator();
        if (GUILayout.Button("Create")) {
            AtomCreator.CreateAtom(name, abbreviation, number);
        }
    }

    int amountType = 0;
    Vector2Int range;

    public void AtomInfoGUI() {
        EditorGUILayout.Separator();

        amountType = GUILayout.SelectionGrid(amountType, new string[] { "Single", "Range" }, 2);
        EditorGUILayout.Separator();

        if (amountType == 0) {

        } else {
            range = EditorGUILayout.Vector2IntField("Range (Atomic Number): ", range);

            EditorGUILayout.Separator();
            if (GUILayout.Button("Create")) {

                // All objects of type Atom
                var obj = AssetDatabase.FindAssets("t: Atom", new string[] { "Assets/Prefabs/ScriptableObjects/Atom" });

                List<Atom> atoms = new List<Atom>();

                // Load them
                for (int i = 0; i < obj.Length; i++) {
                    var path = AssetDatabase.GUIDToAssetPath(obj[i]);

                    Atom atom = AssetDatabase.LoadAssetAtPath<Atom>(path);
                    atoms.Add(atom);
                }

                for (int i = range.x; i <= range.y; i++) {
                    Atom a = atoms.Find((x) => x.GetAtomicNumber() == i);
                    if (a != null) {
                        AtomCreator.CreateAtomInfo(a);
                    } else {
                        Debug.Log("Atomic Number " + i + " Is null");
                    }
                }
            }
        }

    }
    public void AtomDataGUI() {
        EditorGUILayout.Separator();

        amountType = GUILayout.SelectionGrid(amountType, new string[] { "Single", "Range" }, 2);
        EditorGUILayout.Separator();

        if (amountType == 0) {

        } else {
            range = EditorGUILayout.Vector2IntField("Range (Atomic Number): ", range);

            EditorGUILayout.Separator();
            if (GUILayout.Button("Create")) {

                // All objects of type Atom
                var obj = AssetDatabase.FindAssets("t: Atom", new string[] { "Assets/Prefabs/ScriptableObjects/Atom" });

                List<Atom> atoms = new List<Atom>();

                // Load them
                for (int i = 0; i < obj.Length; i++) {
                    var path = AssetDatabase.GUIDToAssetPath(obj[i]);

                    Atom atom = AssetDatabase.LoadAssetAtPath<Atom>(path);
                    atoms.Add(atom);
                }

                for (int i = range.x; i <= range.y; i++) {
                    Atom a = atoms.Find((x) => x.GetAtomicNumber() == i);
                    if (a != null) {
                        AtomCreator.CreateAtomData(a);
                    } else {
                        Debug.Log("Atomic Number " + i + " Is null");
                    }
                }
            }
        }
    }


    // Ability to Edit???
}
