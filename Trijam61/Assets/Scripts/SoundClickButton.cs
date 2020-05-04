using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundClickButton : MonoBehaviour {
	[SerializeField] Image SoundImage = null;
	[SerializeField] Sprite[] SoundImageState = null;

	public void OnSoundClick() {
		AudioManager.Instance.IsEnabled = !AudioManager.Instance.IsEnabled;
		SoundImage.sprite = SoundImageState[AudioManager.Instance.IsEnabled ? 1 : 0];
	}
}
