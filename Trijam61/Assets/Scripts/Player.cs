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
	[SerializeField] float fallMultiplier = 2.5f;
	[SerializeField] float lowJumpMultiplier = 2f;

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

	[Header("Refs")]
	[SerializeField] MainMenuSimple mainMenu;

	[HideInInspector] [SerializeField] Rigidbody2D rb;
	[HideInInspector] [SerializeField] Animator anim;

	Vector2 moveInput = Vector3.zero;
	Vector3 m_Velocity = Vector3.zero;
	bool isGrounded = true;
	bool isFacingRight = true;
	[NonSerialized] public bool isCanControl = false;
	bool canSwitchWorld = true;
	bool isDownSwitch = false;

	Coroutine changeWorldRoutine;
	Coroutine changeLevelTextColorRoutine;

	void OnValidate() {
		if (rb == null)
			rb = GetComponent<Rigidbody2D>();
		if (anim == null)
			anim = GetComponent<Animator>();
	}

	void Update() {
		moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (mainMenu.isInMenu)
				mainMenu.HideInGameMenu();
			else
				mainMenu.ShowInGameMenu();
		}

		if (!isCanControl || mainMenu.isInMenu)
			return;

		if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.K) || Input.GetKeyDown(KeyCode.Z))
			isDownSwitch = true;
		else if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.K) || Input.GetKeyUp(KeyCode.Z))
			isDownSwitch = false;
		Switch();

		if (Input.GetKeyDown(KeyCode.R)) {
			Die();
		}
	}

	void FixedUpdate() {
		//Check is can jump
		isGrounded = false;
		Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, 0.2f, groundMask);
		for (int i = 0; i < colliders.Length; i++) {
			if (!colliders[i].isTrigger) {
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
		mainMenu.ToNormalWorldLens(changeTime, true);
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
		if (changeWorldRoutine != null)
			StopCoroutine(changeWorldRoutine);
		transform.position = level.respawnPos.position;
		isCanControl = false;
		rb.gravityScale = 0.0f;
		rb.velocity = Vector3.zero;
		Time.timeScale = timescale.y;
		currWorld = 0;
		mainMenu.ToNormalWorldLens(changeTime, true);
		level.Awake();

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

	void Switch() {
		CheckIsCanSwitch();
		if (canSwitchWorld && isDownSwitch) {
			isDownSwitch = false;
			cameraShake.ShakeCamera(Vector3.right, currWorld == 0 ? false : true);
			if (changeWorldRoutine != null)
				StopCoroutine(changeWorldRoutine);
			changeWorldRoutine = StartCoroutine(ChangeWorldCoroutine());
		}
	}

	private void Move(Vector2 direction) {
		if (direction.x > 0 && !isFacingRight) {
			Flip();
		}
		else if (direction.x < 0 && isFacingRight) {
			Flip();
		}

		if (!isCanControl || mainMenu.isInMenu) {
			rb.velocity = Vector3.SmoothDamp(rb.velocity, new Vector2(0, rb.velocity.y), ref m_Velocity, .05f);
			anim.SetBool("IsMoving", moveInput != Vector2.zero);
			return;
		}

		Vector3 targetVelocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
		rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref m_Velocity, .05f);

		bool isPressJump = direction.y >= 0.5f;
		if (isGrounded && isPressJump) {
			isGrounded = false;
			rb.velocity = new Vector2(rb.velocity.x, 0);
			rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
		}

		if (rb.velocity.y < 0) {
			rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
		}
		else if (rb.velocity.y > 0 && !isPressJump) {
			rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
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

		if (currWorld == 0)
			mainMenu.ToNormalWorldLens(changeTime, false);
		else
			mainMenu.ToNegativeWorldLens(changeTime, false);

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
				g.color = Color.Lerp(startColorCurr, Level.invinsibleColor, realTime / realChangeTime);
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

			if (t > timeq1 && next.compositeColliders[0].isTrigger) {
				foreach (var c in next.compositeColliders)
					c.isTrigger = false;
			}
			else if (t > timeq3 && !curr.compositeColliders[0].isTrigger) {
				foreach (var c in curr.compositeColliders)
					c.isTrigger = true;
			}

			if (t <= timehalf)
				Time.timeScale = Mathf.Lerp(timescale.y, timescale.x, Mathf.Clamp01(t / timehalf));
			else
				Time.timeScale = Mathf.Lerp(timescale.x, timescale.y, Mathf.Clamp01((t - timehalf) / timehalf));

			yield return null;
		}

		foreach (var g in curr.grids)
			g.color = Level.invinsibleColor;
		foreach (var g in next.grids)
			g.color = Color.white;
		foreach (var g in curr.sprites)
			g.color = Level.invinsibleColor;
		foreach (var g in next.sprites)
			g.color = Color.white;
		Time.timeScale = timescale.y;

		yield return null;
		changeWorldRoutine = null;
	}

	void CheckIsCanSwitch() {
		bool findOverlapTrigger = false;
		var colliders = Physics2D.OverlapCircleAll(transform.position, 0.25f, groundMask);
		for (int i = 0; i < colliders.Length; i++) {
			if (colliders[i].isTrigger)
				findOverlapTrigger = true;
		}
		canSwitchWorld = !findOverlapTrigger;
	}

	public void ChangeLevelTextAlpha(float time, float endAlpha, bool isForce) {
		Level.World curr = level.worlds[currWorld];
		if (curr.groups == null || curr.groups.Length == 0)
			return;

		if (isForce) {
			foreach (var g in curr.groups)
				g.alpha = endAlpha;
		}
		else {
			changeLevelTextColorRoutine = StartCoroutine(ChangeTextAlpha());
		}

		IEnumerator ChangeTextAlpha() {
			float t = 0;
			float startColorCurr = curr.groups[0].alpha;

			while ((t += Time.deltaTime) <= time) {
				foreach (var g in curr.groups)
					g.alpha = Mathf.Lerp(startColorCurr, endAlpha, t / time);
				yield return null;
			}
			changeLevelTextColorRoutine = null;
		}
	}

	public void StopChangeLevelTextAlpha() {
		if (changeLevelTextColorRoutine != null) {
			StopCoroutine(changeLevelTextColorRoutine);
		}
	}
}

[Serializable]
struct DieData {
	public Transform dialog;
	public TextMeshProUGUI[] texts;
	public float[] time;
	public float[] speed;
}