using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.PostProcessing;
using Cinemachine;
using NaughtyAttributes;

public class MainMenuSimple : MonoBehaviour {
	[NonSerialized] public bool isInMenu;
	[NonSerialized] public bool isInGameMenu;

	[SerializeField] CanvasGroup canvasGroup;
	[Space]
	[SerializeField] Player player;
	[Space]
	[SerializeField] Camera camera;
	[SerializeField] CinemachineVirtualCamera virtualCamera;
	[SerializeField] Transform mainMenuCamereTarget;
	[Space]
	[SerializeField] Button playButton;
	[SerializeField] Button levelsButton;
	[SerializeField] Button settingsButton;
	[SerializeField] Button exitButton;

	[Header("Post Processing")]
	[Space]
	[SerializeField] PostProcessVolume postProcessVolume;
	[SerializeField] [ColorUsage(false, true)] Color bloomColorPositive;
	[SerializeField] [ColorUsage(false, true)] Color bloomColorNegative;
	Grain grain;
	LensDistortion lensDistortion;
	ChromaticAberration chromaticAberration;
	Bloom bloom;

	float menuDefaultAlpha;
	float defaultLensDistortionIntensity;
	float defaultGrainIntensity;
	float defaultChromaticAberrationIntensity;

	private void Awake() {
		postProcessVolume.profile.TryGetSettings(out grain);
		postProcessVolume.profile.TryGetSettings(out lensDistortion);
		postProcessVolume.profile.TryGetSettings(out chromaticAberration);
		postProcessVolume.profile.TryGetSettings(out bloom);

		menuDefaultAlpha = canvasGroup.alpha;
		defaultLensDistortionIntensity = lensDistortion.intensity.value;
		defaultGrainIntensity = grain.intensity.value;
		defaultChromaticAberrationIntensity = chromaticAberration.intensity.value;

		bloom.color.value = bloomColorPositive;
	}

	private void Start() {
		ShowMainMenu(true);

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
		LeanTween.cancel(gameObject, false);
		player.StopChangeLevelTextAlpha();
		isInMenu = true;

		canvasGroup.interactable = true;
		canvasGroup.blocksRaycasts = true;

		virtualCamera.Follow = mainMenuCamereTarget;
		

		if (isForce) {
			grain.intensity.value = defaultGrainIntensity;
			chromaticAberration.intensity.value = 1.0f;
			canvasGroup.alpha = menuDefaultAlpha;
			player.ChangeLevelTextAlpha(0.0f, 0.0f, true);
			virtualCamera.m_Lens.OrthographicSize = 2;
		}
		else {
			LeanTween.value(gameObject, grain.intensity.value, defaultGrainIntensity, 0.33f)
			.setOnUpdate((float intensity) => {
				grain.intensity.value = intensity;
			});

			LeanTween.value(gameObject, chromaticAberration.intensity.value, 1.0f, 0.33f)
			.setOnUpdate((float intensity) => {
				chromaticAberration.intensity.value = intensity;
			});

			LeanTweenEx.ChangeCanvasGroupAlpha(gameObject, canvasGroup, menuDefaultAlpha, 0.33f);

			player.ChangeLevelTextAlpha(0.33f, 0.0f, false);

			LeanTween.value(gameObject, virtualCamera.m_Lens.OrthographicSize, 2.0f, 1.0f)
			.setOnUpdate((float size) => {
				virtualCamera.m_Lens.OrthographicSize = size;
			});
		}

		playButton.gameObject.SetActive(true);
		playButton.Select();
	}

	public void HideGameMenu() {
		LeanTween.cancel(gameObject, false);
		player.StopChangeLevelTextAlpha();
		LeanTweenEx.InvokeNextFrame(exitButton.gameObject, () => isInMenu = false);
		
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;

		virtualCamera.Follow = player.transform;
		LeanTween.value(gameObject, virtualCamera.m_Lens.OrthographicSize, 5.0f, 1.0f)
		.setOnUpdate((float size) => { 
			virtualCamera.m_Lens.OrthographicSize = size;
		});

		LeanTween.value(gameObject, chromaticAberration.intensity.value, defaultChromaticAberrationIntensity, 0.33f)
		.setOnUpdate((float intensity) => {
			chromaticAberration.intensity.value = intensity;
		});

		LeanTween.value(gameObject, grain.intensity.value, 0, 0.33f)
		.setOnUpdate((float intensity) => {
			grain.intensity.value = intensity;
		});

		LeanTweenEx.ChangeCanvasGroupAlpha(gameObject, canvasGroup, 0.0f, 0.33f);

		player.ChangeLevelTextAlpha(0.33f, 1.0f, false);
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

	public void ToNormalWorldLens(float time, bool isForce) {
		if (isForce) {
				lensDistortion.intensity.value = defaultLensDistortionIntensity;
				bloom.color.value = bloomColorPositive;
			return;
		}

		LeanTween.value(gameObject, lensDistortion.intensity.value, defaultLensDistortionIntensity, time)
		.setOnUpdate((float intensity) => {
			lensDistortion.intensity.value = intensity;
		});

		LeanTween.value(gameObject, bloom.color.value, bloomColorPositive, time)
		.setOnUpdate((Color c) => {
			bloom.color.value = c;
		});
	}

	public void ToNegativeWorldLens(float time, bool isForce) {
		if (isForce) {
			lensDistortion.intensity.value = -defaultLensDistortionIntensity;
				bloom.color.value = bloomColorNegative;
			return;
		}

		LeanTween.value(gameObject, lensDistortion.intensity.value, -defaultLensDistortionIntensity, time)
		.setOnUpdate((float intensity) => {
			lensDistortion.intensity.value = intensity;
		});

		LeanTween.value(gameObject, bloom.color.value, bloomColorNegative, time)
		.setOnUpdate((Color c) => {
			bloom.color.value = c;
		});
	}
}
