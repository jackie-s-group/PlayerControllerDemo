using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
  // [TODO] [IMPORTANT] Ground check

  // [TODO] Member too chaotic
  private Rigidbody2D player;
  private SpriteRenderer playerSprite;
  private Transform groundDetector;

  private bool jumpRequest;
  private bool isGrounded;
  private float moveDirection;
  private float originXPosition;
  private Vector3 smoothVelocity = Vector3.zero;

  // Horizontal Move Parameters
  [Range(1f, 10f)] public float moveVelocity = 3.0f;
  [Range(1f, 10f)] public float returnVelocity = 1.0f;
  [Range(0f, 1f)] public float smoothTime = .05f;
  [Range(1f, 10f)] public float leftBoundary = 2.0f;
  [Range(1f, 10f)] public float rightBoundary = 2.0f;
  public float returnThreshold = 0.02f;

  // Jump Parameters
  [Range(1f, 50f)] public float jumpForce = 10f;
  [Range(1f, 10f)] public float baseGravity = 1f;
  [Range(1f, 10f)] public float fallGravityScale = 2.5f;
  [Range(1f, 10f)] public float lowjumpGravityScale = 2.0f;

  private void Start() {
    player = GetComponent<Rigidbody2D>();
    playerSprite = GetComponent<SpriteRenderer>();
    originXPosition = player.position.x;
  }

  private void Flip() {
    if (!playerSprite.flipX) {
      playerSprite.flipX = true;
    }
  }

  private void Unflip() {
    if (playerSprite.flipX) {
      playerSprite.flipX = false;
    }
  }

  private void Update() {
    // Get horizontal input
    moveDirection = Input.GetAxisRaw("Horizontal");
    // Flip based on user input
    if (moveDirection < 0) {
      Flip();
    } else {
      Unflip();
    }

    if (Input.GetButtonDown("Jump")) {
      jumpRequest = true;
    }
  }

  private void FixedUpdate() {
    MoveUpdate();
    JumpUpdate();
  }

  private void SmoothMove(float velocity) {
    Vector3 targetVelocity = new Vector2(velocity, player.velocity.y);
    player.velocity = Vector3.SmoothDamp(player.velocity, targetVelocity, ref smoothVelocity, smoothTime);
  }

  private void MoveUpdate() {
    // [TODO ]Need optimization
    // 1 Press inverse key in boundary will stop moving
    // 2 Rest time before return?
    // 3 Sometims shaking in origin
    if (moveDirection != 0) {
      if ((originXPosition - player.position.x < leftBoundary) && (player.position.x - originXPosition < rightBoundary)) {
        SmoothMove(moveDirection * moveVelocity);
      } else {
        player.velocity = new Vector2(0f, player.velocity.y);
      }
    } else {
      if (Mathf.Abs(originXPosition - player.position.x) > returnThreshold) {
        SmoothMove(Mathf.Sign(originXPosition - player.position.x) * returnVelocity);
      } else {
        player.velocity = new Vector2(0f, player.velocity.y);
      }
    }
  }

  private void JumpUpdate() {
    // Jump Request Handle
    if (jumpRequest) {
      player.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

      jumpRequest = false;
    }

    // Jump Optimize
    if (player.velocity.y < 0) {
      player.gravityScale = baseGravity * fallGravityScale;
    } else if (player.velocity.y > 0 && !Input.GetButton("Jump")) {
      player.gravityScale = baseGravity * lowjumpGravityScale;
    } else {
      player.gravityScale = baseGravity;
    }
  }
}
