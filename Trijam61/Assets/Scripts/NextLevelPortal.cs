using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelPortal : MonoBehaviour {
	[SerializeField] Transform newLevelPosition;
	[SerializeField] GameObject[] worlds;

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.tag == "Player") {
			Player pl = collision.GetComponent<Player>();
			pl.transform.position = newLevelPosition.position;
			pl.worlds = worlds;
			pl.currWorld = 0;
			pl.respawnPoint = newLevelPosition;
			pl.Win();
		}
	}
}
