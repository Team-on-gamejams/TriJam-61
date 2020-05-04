using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MenuBase {
	public void OnPlayClick() {
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = true;
		LeanTweenEx.ChangeCanvasGroupAlpha(canvasGroup, 0.0f, 0.25f);
	}

	public void OnLevelsClick() {
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = true;
		LeanTweenEx.ChangeCanvasGroupAlpha(canvasGroup, 0.0f, 0.25f);
	}

	public void OnSettingsClick() {
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = true;
		LeanTweenEx.ChangeCanvasGroupAlpha(canvasGroup, 0.0f, 0.25f);
	}
}
