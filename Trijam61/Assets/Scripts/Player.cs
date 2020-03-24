using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour {
	[Header("Moving")]
	[SerializeField] float moveSpeed = 5.0f;
	[SerializeField] float jumpForce = 20.0f;
	[SerializeField] LayerMask groundMask;
	[SerializeField] Transform groundCheck;

	[Header("Worlds")]
	[SerializeField] GeneralCameraShake cameraShake = null;
	[SerializeField] float changeTime = 1.0f;
	[SerializeField] Vector2 timescale = new Vector2(0.5f, 1.0f);
	[SerializeField] Level level;
	public byte currWorld = 0;

	[Header("Die")]
	[SerializeField] Transform aroundCanvas;
	[SerializeField] DieData[] dieDatas;
	byte currDieDialog = 0;

	[Header("Win")]
	[SerializeField] TextMeshProUGUI winText;
	[SerializeField] float winTextTime;

	[HideInInspector] [SerializeField] Rigidbody2D rb;
	[HideInInspector] [SerializeField] Animator anim;

	Vector2 moveInput = Vector3.zero;
	Vector3 m_Velocity = Vector3.zero;
	bool isGrounded = true;
	bool isFacingRight = true;
	bool isCanControl = true;

	Coroutine changeWorldRoutine;

	void OnValidate() {
		if (rb == null)
			rb = GetComponent<Rigidbody2D>();
		if (anim == null)
			anim = GetComponent<Animator>();
	}

	void Update() {
		moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

		if (!isCanControl)
			return;

		if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.K)) {
			cameraShake.ShakeCamera(Vector3.right, currWorld == 0 ? false : true);
			if (changeWorldRoutine != null)
				StopCoroutine(changeWorldRoutine);
			changeWorldRoutine = StartCoroutine(ChangeWorldCoroutine());
		}

		if (Input.GetKeyDown(KeyCode.R)) {
			Die();
		}
	}

	void FixedUpdate() {
		isGrounded = false;
		Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, 0.2f, groundMask);
		for (int i = 0; i < colliders.Length; i++) {
			if (colliders[i].gameObject != gameObject) {
				isGrounded = true;
				isCanControl = true;
			}
		}

		Move(moveInput);
	}

	public void OnNewLevel(Level l) {
		level = l;
		transform.position = level.respawnPos.position;
		currWorld = 0;
	}

	public void Win() {
		isCanControl = false;
		rb.gravityScale = 0.0f;
		rb.velocity = Vector3.zero;

		winText.gameObject.SetActive(true);
		LeanTween.delayedCall(winTextTime, () => {
			winText.gameObject.SetActive(false);
			rb.gravityScale = 2.0f;
		});
	}

	public void Die() {
		transform.position = level.respawnPos.position;
		isCanControl = false;
		rb.gravityScale = 0.0f;
		rb.velocity = Vector3.zero;
		level.Awake();
		if(changeWorldRoutine != null)
			StopCoroutine(changeWorldRoutine);
		Time.timeScale = timescale.y;

		DieData dieData = dieDatas[currDieDialog];

		float allTime = 0.0f;
		for (byte i = 0; i < dieData.time.Length; ++i) {
			int curr = i;
			LeanTween.delayedCall(allTime, () => {
				LeanTween.cancel(dieData.dialog.gameObject);
				dieData.texts[curr].gameObject.SetActive(true);
				if (curr != 0)
					dieData.texts[curr - 1].gameObject.SetActive(false);
				dieData.dialog.localPosition = Vector3.zero;
				LeanTween.moveLocalY(dieData.dialog.gameObject, dieData.time[curr] * dieData.speed[curr], dieData.time[curr]);
			});

			allTime += dieData.time[i];
		}

		LeanTween.delayedCall(allTime, () => {
			dieData.texts[dieData.time.Length - 1].gameObject.SetActive(false);
			rb.gravityScale = 2.0f;
			if (currDieDialog != dieDatas.Length - 1) {
				++currDieDialog;
			}
			else {
				dieData.texts[0].text += "?";
			}
		});
	}

	private void Move(Vector2 direction) {
		if (direction.x > 0 && !isFacingRight) {
			Flip();
		}
		else if (direction.x < 0 && isFacingRight) {
			Flip();
		}

		if (!isCanControl) {
			rb.velocity = Vector3.SmoothDamp(rb.velocity, new Vector2(0, rb.velocity.y), ref m_Velocity, .05f);
			anim.SetBool("IsMoving", moveInput != Vector2.zero);
			return;
		}

		Vector3 targetVelocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
		rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref m_Velocity, .05f);

		if (isGrounded && direction.y >= 0.5f) {
			isGrounded = false;
			rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
		}

		if (isGrounded) {
			anim.SetBool("IsMoving", moveInput != Vector2.zero);
		}
		else {
			anim.SetBool("IsMoving", false);
		}
	}

	private void Flip() {
		isFacingRight = !isFacingRight;

		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;

		theScale = aroundCanvas.localScale;
		theScale.x *= -1;
		aroundCanvas.localScale = theScale;
	}

	IEnumerator ChangeWorldCoroutine() {
		Level.World curr = level.worlds[currWorld];
		if (++currWorld >= level.worlds.Length)
			currWorld = 0;
		Level.World next = level.worlds[currWorld];

		float t = 0;
		float timeq1 = changeTime / 4;
		float timehalf = changeTime / 2;
		float timeq3 = changeTime / 4 * 3;

		if (next.grids[0].color.a >= 0.75f)
			t = timeq3;
		else if (next.grids[0].color.a >= 0.5f)
			t = timehalf;
		else if (next.grids[0].color.a >= 0.25f)
			t = timeq1;

		Color startColorCurr = curr.grids[0].color;
		Color startColorNext = next.grids[0].color;

		float realTime = 0;
		float realChangeTime = changeTime - t;

		while ((t += Time.deltaTime) <= changeTime) {
			realTime += Time.deltaTime;
			foreach (var g in curr.grids) 
				g.color = Color.Lerp(startColorCurr, Color.clear, realTime / realChangeTime);
			foreach (var g in curr.groups)
				g.alpha = curr.grids[0].color.a;
			foreach (var s in curr.sprites)
				s.color = curr.grids[0].color;

			foreach (var g in next.grids)
				g.color = Color.Lerp(startColorNext, Color.white, realTime / realChangeTime);
			foreach (var g in next.groups)
				g.alpha = next.grids[0].color.a;
			foreach (var s in next.sprites)
				s.color = next.grids[0].color;

			if (t > timeq1 && !next.colliders[0].enabled) {
				foreach (var c in next.colliders)
					c.enabled = true;
			}
			else if (t > timeq3 && curr.colliders[0].enabled) {
				foreach (var c in curr.colliders)
					c.enabled = false;
			}

			if(t <= timehalf)
				Time.timeScale = Mathf.Lerp(timescale.y, timescale.x, Mathf.Clamp01(t / timehalf));
			else
				Time.timeScale = Mathf.Lerp(timescale.x, timescale.y, Mathf.Clamp01((t - timehalf) / timehalf));

			yield return null;
		}

		foreach (var g in curr.grids)
			g.color = Color.clear;
		foreach (var g in next.grids)
			g.color = Color.white;
		foreach (var g in curr.sprites)
			g.color = Color.clear;
		foreach (var g in next.sprites)
			g.color = Color.white;
		Time.timeScale = timescale.y;

		yield return null;
		changeWorldRoutine = null;
	}
}

[Serializable]
struct DieData {
	public Transform dialog;
	public TextMeshProUGUI[] texts;
	public float[] time;
	public float[] speed;
}