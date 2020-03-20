#if !UNITY_FLASH
using UnityEngine;
using System.Collections;
using DentedPixel;

public class GeneralCameraShake : MonoBehaviour {

	[SerializeField] float force1 = 9.5f;
	[SerializeField] float force2 = 10.0f;

	[SerializeField] float time = 1.6f;
	[SerializeField] float shakePeriodTime = 0.42f;

	private AudioClip boomAudioClip;

	// Use this for initialization
	void Start() {
		AnimationCurve volumeCurve = new AnimationCurve(new Keyframe(8.130963E-06f, 0.06526042f, 0f, -1f), new Keyframe(0.0007692695f, 2.449077f, 9.078861f, 9.078861f), new Keyframe(0.01541314f, 0.9343268f, -40f, -40f), new Keyframe(0.05169491f, 0.03835937f, -0.08621139f, -0.08621139f));
		AnimationCurve frequencyCurve = new AnimationCurve(new Keyframe(0f, 0.003005181f, 0f, 0f), new Keyframe(0.01507768f, 0.002227979f, 0f, 0f));
		boomAudioClip = LeanAudio.createAudio(volumeCurve, frequencyCurve, LeanAudio.options().setVibrato(new Vector3[] { new Vector3(0.1f, 0f, 0f) }));
	}

	public void ShakeCamera(Vector3 side, bool isUp) {
		LeanTween.cancel(gameObject);
		transform.localRotation = Quaternion.Euler(Vector3.zero);

		float height = Mathf.PerlinNoise(force1, 0f) * force2;
		height = height * height * 0.3f;

		/**************
		* Camera Shake
		**************/

		float shakeAmt = height * 0.2f * (isUp ? -1 : 1); // the degrees to shake the camera
		LTDescr shakeTween = LeanTween.rotateAroundLocal(gameObject, side, shakeAmt, shakePeriodTime)
		.setEase(LeanTweenType.easeShake) // this is a special ease that is good for shaking
		.setLoopClamp()
		.setRepeat(-1);

		// Slow the camera shake down to zero
		LeanTween.value(gameObject, shakeAmt, 0f, time).setOnUpdate(
			(float val) => {
				shakeTween.setTo(side * val);
			}
		).setEase(LeanTweenType.easeOutQuad);

		// Play BOOM!
		LeanAudio.play(boomAudioClip, transform.position, height * 0.2f); // Like this sound? : http://leanaudioplay.dentedpixel.com/?d=a:fvb:8,0,0.003005181,0,0,0.01507768,0.002227979,0,0,8~8,8.130963E-06,0.06526042,0,-1,0.0007692695,2.449077,9.078861,9.078861,0.01541314,0.9343268,-40,-40,0.05169491,0.03835937,-0.08621139,-0.08621139,8~0.1,0,0,~44100
	}

}
#endif