using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

  private Rigidbody2D player;
  private SpriteRenderer playerSprite;

  private bool jumpRequest;
  private bool isGrounded;
  private float moveDirection;

  private float originXPosition;

  // Horizontal Move Parameters
  [Range(1f, 10f)] public float moveVelocity = 3.0f;
  [Range(1f, 10f)] public float returnVelocity = 1.0f;
  [Range(1f, 10f)] public float moveAeraRadius = 2.0f;
  public float returnThreshold = 0.02f;

  // Jump Parameters
  [Range(1f, 50f)] public float jumpForce = 10f;
  [Range(1f, 10f)] public float baseGravity = 1f;
  [Range(1f, 10f)] public float fallGravityScale = 2.5f;
  [Range(1f, 10f)] public float lowjumpGravityScale = 2.0f;

  private void Start() {
    player = GetComponent<Rigidbody2D>();
    playerSprite = transform.Find("PlayerSprite").GetComponent<SpriteRenderer>();
    originXPosition = player.position.x;
  }

  private void Update() {
    moveDirection = Input.GetAxisRaw("Horizontal");
    if (Input.GetButtonDown("Jump")) {
      jumpRequest = true;
    }
  }

  private void FixedUpdate() {
    MoveUpdate();
    JumpUpdate();
  }

  private void filp() {
    playerSprite.flipX = true;
  }

  private void unflip() {
    playerSprite.flipX = false;
  }

  private void MoveUpdate() {
    if (moveDirection != 0) {
      if (Mathf.Abs(originXPosition - player.position.x) < moveAeraRadius) {
        player.velocity = new Vector2(moveDirection * moveVelocity, player.velocity.y);
      } else {
        player.velocity = new Vector2(0f, player.velocity.y);
      }
    } else {
      if (Mathf.Abs(originXPosition - player.position.x) > returnThreshold) {
        player.velocity = new Vector2(Mathf.Sign(originXPosition - player.position.x) * returnVelocity, player.velocity.y);
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

  //private void OnCollisionStay2D(Collision2D collision) {
  //  if (collision.gameObject.tag == "Ground") {
  //    isGrounded = true;
  //  }
  //}

  //private void OnCollisionExit2D(Collision2D collision) {
  //  if (collision.gameObject.tag == "Ground") {
  //    isGrounded = false;
  //  }
  //}
}
