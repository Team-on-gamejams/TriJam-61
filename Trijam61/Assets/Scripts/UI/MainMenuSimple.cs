using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSimple : MonoBehaviour {
	[SerializeField] CanvasGroup canvasGroup;

	public void OnPlayClick() {
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = true;
		LeanTweenEx.ChangeCanvasGroupAlpha(canvasGroup, 0.0f, 0.25f);
	}
}
