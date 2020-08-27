using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelsSelectionMenu : MonoBehaviour {
	[SerializeField] MainMenuSimple mainMenu;
	[Space]
	[SerializeField] CanvasGroup canvasGroup;

	bool isShowed = false;

	void Awake() {
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
		canvasGroup.alpha = 0.0f;
	}

	public void Show() {
		if (isShowed)
			return;
		isShowed = true;

		canvasGroup.interactable = true;
		canvasGroup.blocksRaycasts = true;
		LeanTweenEx.ChangeCanvasGroupAlpha(gameObject, canvasGroup, 1.0f, 0.33f);
		mainMenu.OnOtherMenuEnter();
	}

	public void HideCanvas() {
		if (!isShowed)
			return;
		isShowed = false;

		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
		LeanTweenEx.ChangeCanvasGroupAlpha(gameObject, canvasGroup, 0.0f, 0.33f);
	}

	public void Hide() {
		HideCanvas();
		mainMenu.OnOtherMenuExit();
	}
}
