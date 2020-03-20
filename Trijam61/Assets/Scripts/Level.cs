using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Level : MonoBehaviour {
	public Transform respawnPos;
	public World[] worlds;

	[Serializable]
	public struct World {
		public GameObject parent;
		public Tilemap[] grids;
	}
}
