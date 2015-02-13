using UnityEngine;
using System;

[Serializable]
public class TilesCreator : MonoBehaviour {

	public GameObject[] tiles;
	public String[] aliases;

	public int[] weights;

	public GameObject teleporter;
	public GameObject target;

	public string teleporterTag;
	public string targetTag;

	
	public TextAsset levelInput;

	public bool randomSection;

	public string length;
	public string height;

	public string pattern;
	public string patternAlias;

	public string template;


	public GameObject[] walls;
	public String[] wallsAliases;

}