using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour {
	[SerializeField] float moveSpeed = 5.0f;
	[SerializeField] float jumpForce = 20.0f;

	[HideInInspector] [SerializeField] Rigidbody2D rb;
	[HideInInspector] [SerializeField] Animator anim;

	Vector2 moveInput = Vector3.zero;
	Vector3 m_Velocity = Vector3.zero;
	bool isGrounded = true;
	bool isFacingRight = true;

	void OnValidate() {
		if(rb == null)
			rb = GetComponent<Rigidbody2D>();
		if (anim == null)
			anim = GetComponent<Animator>();
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		if (collision.transform.tag == "Floor") {
			isGrounded = true;
		}
	}

	void Update() {
		moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
	}

	void FixedUpdate() {
		Move(moveInput);
	}

	private void Move(Vector2 direction) {
		Vector3 targetVelocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
		rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref m_Velocity, .05f);

		if (direction.x > 0 && !isFacingRight) {
			Flip();
		}
		else if (direction.x < 0 && isFacingRight) {
			Flip();
		}

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
	}
}
