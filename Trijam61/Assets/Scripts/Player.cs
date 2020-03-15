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
	[SerializeField] GameObject[] worlds;
	byte currWorld = 0;

	[Header("Die")]
	[SerializeField] Transform aroundCanvas;
	[SerializeField] Transform respawnPoint;
	[SerializeField] DieData[] dieDatas;
	byte currDieDialog = 0;

	[HideInInspector] [SerializeField] Rigidbody2D rb;
	[HideInInspector] [SerializeField] Animator anim;

	Vector2 moveInput = Vector3.zero;
	Vector3 m_Velocity = Vector3.zero;
	bool isGrounded = true;
	bool isFacingRight = true;
	bool isCanControl = true;

	void OnValidate() {
		if(rb == null)
			rb = GetComponent<Rigidbody2D>();
		if (anim == null)
			anim = GetComponent<Animator>();
	}

	void Update() {
		moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

		if (!isCanControl) 
			return;

		if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.K)) {
			worlds[currWorld].SetActive(false);
			if (++currWorld >= worlds.Length)
				currWorld = 0;
			worlds[currWorld].SetActive(true);
		}

		if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.K)) {
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

	public void Die() {
		transform.position = respawnPoint.position;
		isCanControl = false;
		rb.gravityScale = 0.0f;
		rb.velocity = Vector3.zero;
		DieData dieData = dieDatas[currDieDialog];


		float allTime = 0.0f;
		for(byte i = 0; i < dieData.time.Length; ++i) {
			int curr = i;
			LeanTween.delayedCall(allTime, () => {
				LeanTween.cancel(dieData.dialog.gameObject);
				dieData.texts[curr].gameObject.SetActive(true);
				if(curr != 0)
					dieData.texts[curr - 1].gameObject.SetActive(false);
				dieData.dialog.localPosition = Vector3.zero;
				LeanTween.moveLocalY(dieData.dialog.gameObject, dieData.time[curr] * dieData.speed[curr], dieData.time[curr]);
			});

			allTime += dieData.time[i];
		}

		LeanTween.delayedCall(allTime, () => {
			dieData.texts[dieData.time.Length - 1].gameObject.SetActive(false);
			rb.gravityScale = 1.0f;
			if(currDieDialog != dieDatas.Length - 1) {
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
}

[Serializable]
struct DieData {
	public Transform dialog;
	public TextMeshProUGUI[] texts;
	public float[] time;
	public float[] speed;
}