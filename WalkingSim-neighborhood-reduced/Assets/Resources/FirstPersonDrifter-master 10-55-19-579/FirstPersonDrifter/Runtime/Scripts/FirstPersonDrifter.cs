// original by Eric Haines (Eric5h5)
// adapted by @torahhorse
// modified by @igaryhe
// http://wiki.unity3d.com/index.php/FPSWalkerEnhanced

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonDrifter : MonoBehaviour
{
    public float walkSpeed = 6.0f;
    public float runSpeed = 10.0f;

    public bool enableRunning = false;

    public float jumpSpeed = 4.0f;
    public float gravity = 10.0f;

    // Units that player can fall before a falling damage function is run. To disable, type "infinity" in the inspector
    private float fallingDamageThreshold = 10.0f;

    // If the player ends up on a slope which is at least the Slope Limit as set on the character controller, then he will slide down
    public bool slideWhenOverSlopeLimit = false;

    // If checked and the player is on an object tagged "Slide", he will slide down it regardless of the slope limit
    public bool slideOnTaggedObjects = false;

    public float slideSpeed = 5.0f;

    // If checked, then the player can change direction while in the air
    public bool airControl = true;

    // Small amounts of this results in bumping when walking down slopes, but large amounts results in falling too fast
    public float antiBumpFactor = .75f;

    // Player must be grounded for at least this many physics frames before being able to jump again; set to 0 to allow bunny hopping
    public int antiBunnyHopFactor = 1;

    private Vector3 moveDirection = Vector3.zero;
    private bool grounded;
    private CharacterController controller;
    private Transform myTransform;
    private float speed;
    private RaycastHit hit;
    private float fallStartLevel;
    private bool falling;
    private float slideLimit;
    private float rayDistance;
    private Vector3 contactPoint;
    private bool playerControl;
    private int jumpTimer;

    private float inputX;
    private float inputY;
    private bool isRunning;
    private bool isJumping;

    //added myself
    public float seaHeight = 57.8f;
    private Color seaColor = new Color(0, 32, 68);
    private Color normalColor = new Color(0, 0, 220);
    private bool isUnderWater = true;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        myTransform = transform;
        speed = walkSpeed;
        rayDistance = controller.height * .5f + controller.radius;
        slideLimit = controller.slopeLimit - .1f;
        jumpTimer = antiBunnyHopFactor;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void FixedUpdate()
    {
        if(SceneManager.GetActiveScene().name == "WalkSimSadHouse")
        {
            //if (UnityEditor.PlayerSettings.colorSpace == ColorSpace.Linear)
            //{
            //    UnityEditor.PlayerSettings.colorSpace = ColorSpace.Gamma;
            //}

            //added myself
            if (myTransform.position.y <= 57.8 && myTransform.position.y > -60)
            {
                gravity = 1.7f;

                jumpSpeed = 6.0f;
                Debug.Log("working");
                Debug.Log(jumpSpeed);
                Debug.Log("gravity" + gravity);


            }
            else if (myTransform.position.y < -60)
            {
                gravity = 1.0f;

                jumpSpeed = 30.0f;
                Debug.Log("jumping high");

            }
            else if (myTransform.position.y > 57.8)
            {
                gravity = 10.0f;

                jumpSpeed = 6.0f;

            }


            if ((myTransform.position.y < seaHeight) != isUnderWater)
            {
                isUnderWater = myTransform.position.y < seaHeight;
                if (isUnderWater) { SetUnderwater(); }
                if (!isUnderWater) { SetNormal(); }

            }
        }
        //else
        //{
        //    if (UnityEditor.PlayerSettings.colorSpace == ColorSpace.Gamma)
        //    {
        //        UnityEditor.PlayerSettings.colorSpace = ColorSpace.Linear;
        //    }
        //}


        if (grounded)
        {
            var sliding = false;
            // See if surface immediately below should be slid down. We use this normally rather than a ControllerColliderHit point,
            // because that interferes with step climbing amongst other annoyances
            if (Physics.Raycast(myTransform.position, -Vector3.up, out hit, rayDistance))
            {
                if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
                    sliding = true;
            }
            // However, just raycasting straight down from the center can fail when on steep slopes
            // So if the above raycast didn't catch anything, raycast down from the stored ControllerColliderHit point instead
            else
            {
                Physics.Raycast(contactPoint + Vector3.up, -Vector3.up, out hit);
                if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
                    sliding = true;
            }

            // If we were falling, and we fell a vertical distance greater than the threshold, run a falling damage routine
            if (falling)
            {
                falling = false;
                if (myTransform.position.y < fallStartLevel - fallingDamageThreshold)
                    FallingDamageAlert(fallStartLevel - myTransform.position.y);
            }

            if (enableRunning)
            {
                speed = isRunning ? runSpeed : walkSpeed;
            }

            // If sliding (and it's allowed), or if we're on an object tagged "Slide", get a vector pointing down the slope we're on
            if (sliding && slideWhenOverSlopeLimit || slideOnTaggedObjects && hit.collider.CompareTag("Slide"))
            {
                var hitNormal = hit.normal;
                moveDirection = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
                Vector3.OrthoNormalize(ref hitNormal, ref moveDirection);
                moveDirection *= slideSpeed;
                playerControl = false;
            }
            // Otherwise recalculate moveDirection directly from axes, adding a bit of -y to avoid bumping down inclines
            else
            {
                moveDirection = new Vector3(inputX, -antiBumpFactor, inputY);
                moveDirection = myTransform.TransformDirection(moveDirection) * speed;
                playerControl = true;
            }

            // Jump! But only if the jump button has been released and player has been grounded for a given number of frames
            if (!isJumping) jumpTimer++;
            else if (jumpTimer >= antiBunnyHopFactor)
            {
                moveDirection.y = jumpSpeed;
                jumpTimer = 0;
            }
        }
        else
        {
            // If we stepped over a cliff or something, set the height at which we started falling
            if (!falling)
            {
                falling = true;
                fallStartLevel = myTransform.position.y;
            }

            // If air control is allowed, check movement but don't touch the y component
            if (airControl && playerControl)
            {
                moveDirection.x = inputX * speed;
                moveDirection.z = inputY * speed;
                moveDirection = myTransform.TransformDirection(moveDirection);
            }
        }

        // Apply gravity
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller, and set grounded true or false depending on whether we're standing on something
        grounded = (controller.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;
    }

    // Store point that we're in contact with for use in FixedUpdate if needed
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        contactPoint = hit.point;
    }

    // If falling damage occured, this is the place to do something about it. You can make the player
    // have hitpoints and remove some of them based on the distance fallen, add sound effects, etc.
    void FallingDamageAlert(float fallDistance)
    {
        //print ("Ouch! Fell " + fallDistance + " units!");   
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        inputX = ctx.ReadValue<Vector2>().x;
        inputY = ctx.ReadValue<Vector2>().y;
    }

    public void OnRun(InputAction.CallbackContext ctx)
    {
        isRunning = ctx.performed;
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        isJumping = ctx.performed;
    }

    void SetUnderwater()
    {
        RenderSettings.fogColor = seaColor;
        RenderSettings.fogDensity = 0.005f;
    }

    void SetNormal()
    {
        RenderSettings.fogColor = normalColor;
        RenderSettings.fogDensity = 0.002f;
    }
}