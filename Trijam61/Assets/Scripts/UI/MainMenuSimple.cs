using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using NaughtyAttributes;

public class MainMenuSimple : MonoBehaviour {
	[SerializeField] CanvasGroup canvasGroup;
	[Space]
	[SerializeField] Player player;
	[Space]
	[SerializeField] string[] layerMaskMenu;
	[SerializeField] string[] layerMaskGame;
	[SerializeField] Camera camera;
	[SerializeField] CinemachineVirtualCamera menuCamera;
	[SerializeField] CinemachineVirtualCamera gameCamera;

	private void Awake() {
		menuCamera.enabled = true;
		gameCamera.enabled = false;
		camera.cullingMask = LayerMask.GetMask(layerMaskMenu);
	}

	public void OnPlayClick() {
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = true;
		LeanTweenEx.ChangeCanvasGroupAlpha(canvasGroup, 0.0f, 0.25f);

		menuCamera.enabled = false;
		gameCamera.enabled = true;
		camera.cullingMask = LayerMask.GetMask(layerMaskGame);
	}

	public void OnLevelsClick() {

	}

	public void OnSettingsClick() {

	}
}
