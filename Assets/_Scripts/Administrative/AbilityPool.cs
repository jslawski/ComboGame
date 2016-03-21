using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;

[System.Serializable]
public struct AbilityInfo {
	public Sprite abilityImage;             //Image for the ability
	public string abilityName;              //Name of the ability
	public string abilityDescription;       //Description of the ability
	public Type type;                       //Class type, used in AddComponent()

	//Returns a deep copy of the passed in struct
	public static AbilityInfo Clone(AbilityInfo other) {
		AbilityInfo returnInfo = new AbilityInfo();
		returnInfo.abilityName = other.abilityName;
		returnInfo.abilityDescription = other.abilityDescription;
		returnInfo.abilityImage = other.abilityImage;
		returnInfo.type = other.type;
		return returnInfo;
	}

	public override string ToString() {
		return "Name: " + abilityName + "\nDescription: " + abilityDescription + "\nType: " + type.ToString();
	}
}

public class AbilityPool : MonoBehaviour {
	public static AbilityPool S;
	List<AbilityInfo> abilitiesInPool = new List<AbilityInfo>();

	void Awake() {
		S = this;
	}

	// Use this for initialization
	void Start () {
		//Find all classes loaded into the assembler
		Type[] allActivatedAbilityTypes = Assembly.GetAssembly(typeof(ActivatedAbility)).GetTypes();
		//TODO: Add passive abilities too
		//Type[] allPassiveAbilityTypes = Assembly.GetAssembly(typeof(PassiveAbility)).GetTypes();
		foreach (var subclass in allActivatedAbilityTypes) {
			//Filter only those which derive from ActivatedAbility
			if (subclass.IsSubclassOf(typeof(ActivatedAbility))) {
				//Can only instantiate classes which derive from Monobehaviour through use of AddComponent,
				//so we attach it to the player and immediately remove it to get the ability information created on the ability's Awake()
				ActivatedAbility temp = GameManager.S.player.gameObject.AddComponent(subclass) as ActivatedAbility;
				//Remember all subclasses of ActivatedAbility
				abilitiesInPool.Add(AbilityInfo.Clone(temp.info));
				//Destroy(temp);
			}
		}

		//DEBUG Prints
		foreach (var item in abilitiesInPool) {
			print(item.ToString());
		}
	}
	
	public List<AbilityInfo> GetRandomAbilitiesFromPool(int count) {
		//Start with a deep copy of the pool
		List<AbilityInfo> list = new List<AbilityInfo>();
		foreach (var item in abilitiesInPool) {
			list.Add(item);
		}

		//Remove random items until we are left with size == count
		while (abilitiesInPool.Count > count) {
			int randIndex = UnityEngine.Random.Range(0, abilitiesInPool.Count);
			abilitiesInPool.RemoveAt(randIndex);
		}

		return list;
	}

	public void RemoveAbilityFromPool(AbilityInfo item) {
		abilitiesInPool.Remove(item);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
