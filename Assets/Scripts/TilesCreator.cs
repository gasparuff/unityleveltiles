using UnityEngine;
using System;

[Serializable]
public class TilesCreator : MonoBehaviour {

	public GameObject[] tiles;
	public String[] aliases;

	public GameObject teleporter;
	public GameObject target;

	public string teleporterTag;
	public string targetTag;

	
	public TextAsset levelInput;
}