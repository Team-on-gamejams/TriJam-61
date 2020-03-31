using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Level : MonoBehaviour {
	public static Color invinsibleColor = new Color(1, 1, 1, 0.05f);

	public Transform respawnPos;
	public World[] worlds;

#if UNITY_EDITOR
	[ContextMenu("Get auto reference")]
	void ForceValidate() {
		for (byte i = 0; i < worlds.Length; ++i) {
			worlds[i].grids = null;
			worlds[i].compositeColliders = null;
			worlds[i].groups = null;
			worlds[i].colliders = null;
			worlds[i].sprites = null;
		}

		OnValidate();
	}

	private void OnValidate() {
		for(byte i = 0; i < worlds.Length; ++i) {
			if(worlds[i].colliders == null || worlds[i].colliders.Length == 0) {
				List<Collider2D> colliders = new List<Collider2D>();
				List<CompositeCollider2D> collidersComp = new List<CompositeCollider2D>();

				Collider2D[] collidersAll = worlds[i].parent.GetComponentsInChildren<Collider2D>();
				foreach (Collider2D c in collidersAll) {
					if (!(c is CompositeCollider2D))
						colliders.Add(c);
					else
						collidersComp.Add(c as CompositeCollider2D);
				}

				worlds[i].colliders = colliders.ToArray();
				worlds[i].compositeColliders = collidersComp.ToArray();
			}

			if (worlds[i].sprites == null || worlds[i].sprites.Length == 0) {
				List<SpriteRenderer> sprites = new List<SpriteRenderer>();

				sprites.AddRange(worlds[i].parent.GetComponentsInChildren<SpriteRenderer>());

				worlds[i].sprites = sprites.ToArray();
			}

			if (worlds[i].grids == null || worlds[i].grids.Length == 0) {
				List<Tilemap> grids = new List<Tilemap>();

				grids.AddRange(worlds[i].parent.GetComponentsInChildren<Tilemap>());

				worlds[i].grids = grids.ToArray();
			}

			if (worlds[i].groups == null || worlds[i].groups.Length == 0) {
				List<CanvasGroup> groups = new List<CanvasGroup>();

				groups.AddRange(worlds[i].parent.GetComponentsInChildren<CanvasGroup>());

				worlds[i].groups = groups.ToArray();
			}
		}
	}
#endif

	public void Awake() {
		foreach (var g in worlds[0].grids)
			g.color = Color.white;
		foreach (var g in worlds[0].groups)
			g.alpha = 1.0f;
		foreach (var c in worlds[0].compositeColliders)
			c.isTrigger = false;
		foreach (var s in worlds[0].sprites)
			s.color = Color.white;

		foreach (var g in worlds[1].grids)
			g.color = invinsibleColor;
		foreach (var g in worlds[1].groups)
			g.alpha = 0.0f;
		foreach (var c in worlds[1].compositeColliders)
			c.isTrigger = true;
		foreach (var s in worlds[1].sprites)
			s.color = invinsibleColor;

		worlds[0].parent.SetActive(true);
		worlds[1].parent.SetActive(true);
	}

	[Serializable]
	public struct World {
		public GameObject parent;

		public Tilemap[] grids;
		public CanvasGroup[] groups;
		public Collider2D[] colliders;
		public CompositeCollider2D[] compositeColliders;
		public SpriteRenderer[] sprites;
	}
}
