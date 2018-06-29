using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradeUI : MonoBehaviour {

    [SerializeField] TextMeshProUGUI radiusCostText;
    [SerializeField] TextMeshProUGUI speedCostText;
    [SerializeField] TextMeshProUGUI efficiencyCostText;
    [SerializeField] TextMeshProUGUI particleSpeedCostText;
    [SerializeField] TextMeshProUGUI particleStabilityCostText;

    [SerializeField] TextMeshProUGUI errorText;

    public void UpgradeRadius() {
        if (!Game.Instance.playerData.UpgradeAtomCollectorRadius()) {
            errorText.text = "You do not have enough Atoms!";
        } else {
            Reset();
            print("Radius is " + Game.Instance.playerData.GetAtomCollectorRadius());
        }
    }
    public void UpgradeSpeed() {
        if (!Game.Instance.playerData.UpgradeAtomCollectorSpeed()) {
            errorText.text = "You do not have enough Atoms!";
        } else {
            Reset();
            print("Speed is " + Game.Instance.playerData.GetAtomCollectorSpeed());
        }
    }
    public void UpgradeEfficiency() {
        if (!Game.Instance.playerData.UpgradeAtomCollectorEfficiency()) {
            errorText.text = "You do not have enough Atoms!";
        } else {
            Reset();
            print("Efficiency is " + Game.Instance.playerData.GetAtomCollectorEfficiency());
        }
    }
    public void UpgradeParticleSpeed() {
        if (!Game.Instance.playerData.UpgradeParticleSpeed()) {
            errorText.text = "You do not have enough Atoms!";
        } else {
            Reset();
            print("Particle Speed is " + Game.Instance.playerData.GetParticleSpeed());
        }
    }
    public void UpgradeParticleStability() {
        if (!Game.Instance.playerData.UpgradeParticleStabilization()) {
            errorText.text = "You do not have enough Atoms!";
        } else {
            Reset();
            print("Particle Stabilization is " + Game.Instance.playerData.GetParticleStabilization());
        }
    }

    private void OnEnable() {
        Reset();
    }

    public void Reset() {
        var radiusCost = Game.Instance.playerData.GetAtomCollectorRadiusCost();
        var speedCost = Game.Instance.playerData.GetAtomCollectorSpeedCost();
        var efficiencyCost = Game.Instance.playerData.GetAtomCollectorEfficiencyCost();
        var particleSpeedCost = Game.Instance.playerData.GetParticleSpeedCost();
        var particleStabilityCost = Game.Instance.playerData.GetParticleStabilizationCost();

        speedCostText.text = "Atom: " + speedCost.atom.GetName() + "\nCost: " + speedCost.amo;
        particleStabilityCostText.text = "Atom: " + particleStabilityCost.atom.GetName() + "\nCost: " + particleStabilityCost.amo;
        radiusCostText.text = "Atom: " + radiusCost.atom.GetName() + "\nCost: " + radiusCost.amo;
        particleSpeedCostText.text = "Atom: " + particleSpeedCost.atom.GetName() + "\nCost: " + particleSpeedCost.amo;
        efficiencyCostText.text = "Atom: " + efficiencyCost.atom.GetName() + "\nCost: " + efficiencyCost.amo;

        errorText.text = "";
    }

}
