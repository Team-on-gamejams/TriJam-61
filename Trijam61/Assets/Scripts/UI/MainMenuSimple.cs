using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using NaughtyAttributes;
using System;
using UnityEngine.UI;

public class MainMenuSimple : MonoBehaviour {
	[NonSerialized] public bool isInMenu;
	[NonSerialized] public bool isInGameMenu;

	[SerializeField] CanvasGroup canvasGroup;
	[Space]
	[SerializeField] Player player;
	[Space]
	[SerializeField] string[] layerMaskMenu;
	[SerializeField] string[] layerMaskGame;
	[SerializeField] Camera camera;
	[SerializeField] CinemachineVirtualCamera menuCamera;
	[SerializeField] CinemachineVirtualCamera gameCamera;
	[Space]
	[SerializeField] Button playButton;

	float alpha;

	private void Awake() {
		alpha = canvasGroup.alpha;

		ShowMainMenu(true);
	}

	public void OnPlayClick() {
		HideGameMenu();
	}

	public void OnLevelsClick() {
		HideGameMenu();
	}

	public void OnSettingsClick() {
		HideGameMenu();
	}

	public void ShowMainMenu(bool isForce) {
		canvasGroup.interactable = true;
		canvasGroup.blocksRaycasts = true;
		if (isForce)
			canvasGroup.alpha = alpha;
		else
			LeanTweenEx.ChangeCanvasGroupAlpha(canvasGroup, alpha, 0.33f);

		menuCamera.enabled = true;
		gameCamera.enabled = false;
		camera.cullingMask = LayerMask.GetMask(layerMaskMenu);
		playButton.gameObject.SetActive(true);
		isInMenu = true;
	}

	public void HideGameMenu() {
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
		LeanTweenEx.ChangeCanvasGroupAlpha(canvasGroup, 0.0f, 0.33f);

		menuCamera.enabled = false;
		gameCamera.enabled = true;
		camera.cullingMask = LayerMask.GetMask(layerMaskGame);

		isInMenu = false;
	}

	public void ShowInGameMenu() {
		isInGameMenu = true;
		ShowMainMenu(false);
		playButton.gameObject.SetActive(false);
	}

	public void HideInGameMenu() {
		isInGameMenu = false;
		HideGameMenu();
	}
}
