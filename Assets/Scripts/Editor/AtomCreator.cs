using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AtomCreator : MonoBehaviour {

    public static void CreateAtom(string name, string abbreviation, int atomicNumber) {
        Atom a = new Atom(name, abbreviation, atomicNumber);

        AtomInfo aI = ScriptableObject.CreateInstance<AtomInfo>();
        AtomData aD = ScriptableObject.CreateInstance<AtomData>();

        string atomPathName = AssetDatabase.GenerateUniqueAssetPath("Assets/Prefabs/ScriptableObjects/Atom/" + a.GetAtomicNumber() + a.GetName() + ".asset");
        string atomInfoPathName = AssetDatabase.GenerateUniqueAssetPath("Assets/Prefabs/ScriptableObjects/AtomInfo/" + a.GetAtomicNumber() + a.GetName() + "Info.asset");
        string atomDataPathName = AssetDatabase.GenerateUniqueAssetPath("Assets/Prefabs/ScriptableObjects/AtomData/" + a.GetAtomicNumber() + a.GetName() + "Data.asset");

        AssetDatabase.CreateAsset(a, atomPathName);
        AssetDatabase.CreateAsset(aI, atomInfoPathName);
        AssetDatabase.CreateAsset(aD, atomDataPathName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
    }
    public static void CreateAtomInfo(Atom a) {
        AtomInfo aI = new AtomInfo(a);

        string atomInfoPathName = AssetDatabase.GenerateUniqueAssetPath("Assets/Prefabs/ScriptableObjects/AtomInfo/" + a.GetAtomicNumber() + a.GetName() + "Info.asset");

        AssetDatabase.CreateAsset(aI, atomInfoPathName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
    }
    public static void CreateAtomData(Atom a) {
        AtomData aD = new AtomData(a);

        string atomDataPathName = AssetDatabase.GenerateUniqueAssetPath("Assets/Prefabs/ScriptableObjects/AtomData/" + a.GetAtomicNumber() + a.GetName() + "Data.asset");
        AssetDatabase.CreateAsset(aD, atomDataPathName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
    }
}
