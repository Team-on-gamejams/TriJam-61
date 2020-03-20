using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelPortal : MonoBehaviour {
	[SerializeField] Level nextLevel;

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.tag == "Player") {
			Player pl = collision.GetComponent<Player>();
			pl.OnNewLevel(nextLevel);
			pl.Win();
		}
	}
}
