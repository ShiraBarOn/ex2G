using UnityEngine;
using Infra;
using Infra.Gameplay;
using Infra.Utils;

namespace Gadget {

[RequireComponent(typeof(Animator), typeof(Rigidbody2D), typeof(OverlapChecker))]
/// <summary>
/// The player controller class.
/// </summary>
public class Player : MonoBehaviour {
    public int Health = 1;
    public float JumpHeight = 15f;
    public float Acceleration = 7f;
    public float Weight = 2f;
    public float ShootingAngle = 45f;
    public float ShotSpeed = 15f;

    // user input keys
    public KeyCode Right;
    public KeyCode Left;
    public KeyCode Jump;
    public KeyCode Shoot;

    // Unity components parameters

    public Transform ShootingArm;
    public Transform BulletImage;
    public Rigidbody2D Bullet;

    private readonly int PlayerIsAlive = Animator.StringToHash("Alive");
    private readonly int PlayerJumps = Animator.StringToHash("Jump");

    private Animator PlayerState;
    private Rigidbody2D PlayerObject;
    private OverlapChecker OverlapChecker;

    private bool CanJump {
        get {
            return OverlapChecker.isOverlapping;
        }
    }

    protected void Awake() {
        PlayerState = GetComponent<Animator>();
        PlayerObject = GetComponent<Rigidbody2D>();
        OverlapChecker = GetComponent<OverlapChecker>();

        PlayerState.SetBool(PlayerIsAlive, true);
    }

    protected void Update() {
        // sets the arm angle
        var var01 = ShootingArm.eulerAngles;
        var01.z = ShootingAngle;
        ShootingArm.eulerAngles = var01;
        
        // sets the object wieght
        PlayerObject.gravityScale = Weight;
        
        var var02 = PlayerObject.velocity;
        
        // checks if the user pushed the jump key and if so - makes the player jump
        if (Input.GetKeyDown(Jump) && CanJump) {
            var02.y = JumpHeight;
            PlayerObject.velocity = var02;
            PlayerState.SetTrigger(PlayerJumps);
        // checks if the user pushed the right key and if so - makes the player move right in the given acceleration
        } else if (Input.GetKey(Right)) {
            var02.x = Acceleration;
            PlayerObject.velocity = var02;
        // checks if the user pushed the left key and if so - makes the player move left in the given acceleration
        } else if (Input.GetKey(Left)) {
            var02.x = -Acceleration;
            PlayerObject.velocity = var02;
        // checks if the user pushed the shooting key and if so - shoots a bullet in the given arm angle and in the given speed
        } else if (Input.GetKey(Shoot)) {
            if (!Bullet.gameObject.activeInHierarchy) {
                Bullet.gameObject.SetActive(true);
                Bullet.position = BulletImage.position;
                Bullet.velocity = Vector2.right.Rotate(Mathf.Deg2Rad * ShootingAngle) * ShotSpeed;
            }
        }
    }

    protected void OnCollisionEnter2D(Collision2D collision) {
        
        // if the player is dead, there is nothing to check
        if (Health <= 0) return;

        // if the player reached the end vox it will print the winning message to the console log
        if (collision.gameObject.CompareTag("Victory")) {
            DebugUtils.Log("Great Job!");
            return;
        }

        // if the player an object which is not an enemy, nothing will happen
        if (!collision.gameObject.CompareTag("Enemy")) return;

        // if the player touched an enemy it will loose one health point
        --Health;
        
        // if after loosing health the player didn't die, it is ok and the function will return
        if (Health > 0) return;

        // if the Health reached 0 the players state will be updated to not alive, and the player object will change to being disabled.
        PlayerState.SetBool(PlayerIsAlive, false);
        PlayerObject.velocity = Vector2.zero;
        PlayerObject.gravityScale = 4f;
        enabled = false;
    }
}
}
