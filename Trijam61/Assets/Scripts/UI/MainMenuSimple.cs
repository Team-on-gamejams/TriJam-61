using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Cinemachine;
using NaughtyAttributes;

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
	[SerializeField] Button levelsButton;
	[SerializeField] Button settingsButton;
	[SerializeField] Button exitButton;

	float alpha;

	private void Awake() {
		alpha = canvasGroup.alpha;

		ShowMainMenu(true);
	}

	private void Start() {
		playButton.Select();
	}

	private void Update() {
		if (isInMenu && EventSystem.current.currentSelectedGameObject == null && new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).magnitude >= 0.5f) {
			if(isInGameMenu)
				levelsButton.Select();
			else
				playButton.Select();
		}
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
		isInMenu = true;

		if (isForce)
			canvasGroup.alpha = alpha;
		else
			LeanTweenEx.ChangeCanvasGroupAlpha(canvasGroup, alpha, 0.33f);
		canvasGroup.interactable = true;
		canvasGroup.blocksRaycasts = true;

		menuCamera.enabled = true;
		gameCamera.enabled = false;
		camera.cullingMask = LayerMask.GetMask(layerMaskMenu);

		playButton.gameObject.SetActive(true);
		playButton.Select();
		//playButton.Select();
	}

	public void HideGameMenu() {
		isInMenu = false;
		
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
		LeanTweenEx.ChangeCanvasGroupAlpha(canvasGroup, 0.0f, 0.33f);

		menuCamera.enabled = false;
		gameCamera.enabled = true;
		camera.cullingMask = LayerMask.GetMask(layerMaskGame);
	}

	public void ShowInGameMenu() {
		isInGameMenu = true;

		ShowMainMenu(false);

		playButton.gameObject.SetActive(false);
		levelsButton.Select();
	}

	public void HideInGameMenu() {
		isInGameMenu = false;

		HideGameMenu();
	}
}
