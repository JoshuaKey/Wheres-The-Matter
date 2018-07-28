using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AtomCreator : MonoBehaviour {

    public static void CreateAtom(int atomicNumber) {
        int number = atomicNumber;

        int firstNumber = number / 100;
        string first = GetValue(firstNumber); 
        string firstSymbol = GetPrefix(firstNumber).ToUpper();
        number -= number / 100 * 100;

        first = char.ToUpper(first[0]) + first.Substring(1);// To Upper

        int secondNumber = number / 10;
        string second = GetValue(secondNumber);
        string secondSymbol = GetPrefix(secondNumber);
        number -= number / 10 * 10;

        if (secondNumber == 0 && firstNumber == 9) { // 90
            first = "en";
        }

        string third = GetValue(number);
        string thirdSymbol = GetPrefix(number);

        if (number == 0 && secondNumber == 9) { // 90
            second = "en";
        }
        string end = third[third.Length - 1] == 'i' ? "um" : "ium";

        CreateAtom(first + second + third + end, firstSymbol + secondSymbol + thirdSymbol, atomicNumber);
    }
    public static string GetValue(int num) {
        switch (num) {
            case 0:
                return "nil";
            case 1:
                return "un";
            case 2:
                return "bi";
            case 3:
                return "tri";
            case 4:
                return "quad";
            case 5:
                return "pent";
            case 6:
                return "hex";
            case 7:
                return "sept";
            case 8:
                return "oct";
            case 9:
                return "enn";
        }
        return "";
    }
    public static string GetPrefix(int num) {
        switch (num) {
            case 0:
                return "n";
            case 1:
                return "u";
            case 2:
                return "b";
            case 3:
                return "t";
            case 4:
                return "q";
            case 5:
                return "p";
            case 6:
                return "h";
            case 7:
                return "s";
            case 8:
                return "o";
            case 9:
                return "e";
        }
        return "";
    }

    public static void CreateAtom(string name, string abbreviation, int atomicNumber) {
        Atom a = Atom.CreateNewAtom(name, abbreviation, atomicNumber);

        AtomInfo aI = AtomInfo.CreateNewAtomInfo(a);
        AtomData aD = AtomData.CreateNewAtomData(a);

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
        AtomInfo aI = AtomInfo.CreateNewAtomInfo(a);

        string atomInfoPathName = AssetDatabase.GenerateUniqueAssetPath("Assets/Prefabs/ScriptableObjects/AtomInfo/" + a.GetAtomicNumber() + a.GetName() + "Info.asset");

        AssetDatabase.CreateAsset(aI, atomInfoPathName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
    }
    public static void CreateAtomData(Atom a) {
        AtomData aD = AtomData.CreateNewAtomData(a);

        string atomDataPathName = AssetDatabase.GenerateUniqueAssetPath("Assets/Prefabs/ScriptableObjects/AtomData/" + a.GetAtomicNumber() + a.GetName() + "Data.asset");
        AssetDatabase.CreateAsset(aD, atomDataPathName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
    }
}
