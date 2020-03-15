using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelPortal : MonoBehaviour {
	[SerializeField] Transform newLevelPosition;

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.tag == "Player") {
			Player pl = collision.GetComponent<Player>();
			pl.transform.position = newLevelPosition.position;
			pl.respawnPoint = newLevelPosition;
			pl.Win();
		}
	}
}
