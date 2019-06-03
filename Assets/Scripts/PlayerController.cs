using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
  // [TODO] Member too chaotic
  private Rigidbody2D playerRb;
  private Vector2 playerSize;
  private SpriteRenderer playerSprite;

  private bool jumpRequest;
  private bool isGrounded;
  private float moveDirection;
  private float originXPosition;
  private Vector3 smoothVelocity = Vector3.zero;
  private Vector2 groundDetectSize;

  // Ground Detect Parameters
  public Transform groundDetector;
  public LayerMask groundLayer;
  [Range(0.01f, 0.05f)] public float groundDetectThickness;

  // Horizontal Move Parameters
  [Range(1f, 10f)] public float moveVelocity = 3.0f;
  [Range(1f, 10f)] public float returnVelocity = 1.0f;
  [Range(0f, 1f)] public float smoothTime = 0.05f;
  [Range(1f, 10f)] public float leftBoundary = 2.0f;
  [Range(1f, 10f)] public float rightBoundary = 2.0f;
  [Range(0.01f, 0.05f)] public float returnThreshold = 0.02f;

  // Jump Parameters
  [Range(1f, 50f)] public float jumpForce = 10f;
  [Range(1f, 10f)] public float baseGravity = 1f;
  [Range(1f, 10f)] public float fallGravityScale = 2.5f;
  [Range(1f, 10f)] public float lowjumpGravityScale = 2.0f;

  private void Start() {
    playerRb = GetComponent<Rigidbody2D>();
    playerSize = GetComponent<BoxCollider2D>().size;
    playerSprite = GetComponent<SpriteRenderer>();
    originXPosition = playerRb.position.x;
    groundDetectSize = new Vector2(playerSize.x * 0.9f, groundDetectThickness);
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

    if (Input.GetButtonDown("Jump") && isGrounded) {
      jumpRequest = true;
      isGrounded = false;
    }
  }

  private void FixedUpdate() {
    // Ground detect
    isGrounded = Physics2D.OverlapBox(groundDetector.position, groundDetectSize, 0f, groundLayer);

    MoveUpdate();
    JumpUpdate();
  }

  private void SmoothMove(float velocity) {
    Vector3 targetVelocity = new Vector2(velocity, playerRb.velocity.y);
    playerRb.velocity = Vector3.SmoothDamp(playerRb.velocity, targetVelocity, ref smoothVelocity, smoothTime);
  }

  private void MoveUpdate() {
    // [TODO] Need optimization
    // 1 Press inverse key in boundary will stop moving ##
    // 2 Rest time before return?
    // 3 Sometimes shaking in origin (rarely)
    if (moveDirection != 0) {
      if ((originXPosition - playerRb.position.x < leftBoundary) && (playerRb.position.x - originXPosition < rightBoundary)) {
        SmoothMove(moveDirection * moveVelocity);
      } else {
        playerRb.velocity = new Vector2(0f, playerRb.velocity.y);
      }
    } else {
      if (Mathf.Abs(originXPosition - playerRb.position.x) > returnThreshold) {
        SmoothMove(Mathf.Sign(originXPosition - playerRb.position.x) * returnVelocity);
      } else {
        playerRb.velocity = new Vector2(0f, playerRb.velocity.y);
      }
    }
  }

  private void JumpUpdate() {
    // Jump Request Handle
    if (jumpRequest) {
      playerRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

      jumpRequest = false;
    }

    // Jump Optimize
    if (playerRb.velocity.y < 0) {
      playerRb.gravityScale = baseGravity * fallGravityScale;
    } else if (playerRb.velocity.y > 0 && !Input.GetButton("Jump")) {
      playerRb.gravityScale = baseGravity * lowjumpGravityScale;
    } else {
      playerRb.gravityScale = baseGravity;
    }
  }
}
