using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : MonoBehaviour, IDamage, IBoop
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] GameObject speedLinesPrefab;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;

    [Header("----- Player Stats -----")]
    [Range(1, 20)][SerializeField] float playerSpeed;
    [Range(1, 50)][SerializeField] float jumpHeight;
    [Range(-10, 50)][SerializeField] float gravityValue;
    [Range(0, 1)][SerializeField] float frictionAir;
    [Range(0, 1)][SerializeField] float frictionGround;
    [SerializeField] float speedLineRotSpeed;
    [SerializeField] float speedFadeSpeed;
    [SerializeField] float speedFadeIn;
    [SerializeField] float speedFadeOut;

    [Header("----- Dash Stats -----")]
    [SerializeField] float dashSpeed;
    [SerializeField] float dashDuration;
    [SerializeField] int dashCooldown;

    [Header("----- Slam Stats -----")]
    [SerializeField] float slamSpeed;

    [Header("----- Groove Stats -----")]
    [Range(1, 30)][SerializeField] float groovePlayerSpeed;

    [Header("----- Gun Stats -----")]
    [SerializeField] GameObject boopCard;
    [SerializeField] LayerMask coneLayerMask;
    [SerializeField] int boopForce;
    [SerializeField] int rayCount;
    [SerializeField] int rayDistance;
    [SerializeField] int coneOuterAngle;
    [SerializeField] int coneInnerAngle;
    [SerializeField] int coneOuterInnerAngle;

    [Header("----- Audio -----")]
    [SerializeField] AudioClip boopSFX;
    [SerializeField] AudioClip blastPenaltySFX;
    [SerializeField] AudioClip jumpSFX;
    [SerializeField] AudioClip dashSFX;
    [SerializeField] AudioClip slamSFX;
    [SerializeField] AudioClip slamStartSFX;
    [SerializeField] AudioClip deathSFX;
    [SerializeField] AudioClip damageSFX;

    [Range(100, 2500)] public float sensitivity;

    GameObject ghost;
    GameObject speedLines;

    Material speedLineMat;

    Transform startPos;

    Vector3 translation;
    Vector3 move;
    Vector3 playerVelocity;
    Vector3 boopVelocity;
    Vector3 boopVelocityOg;
    Vector3 jumpVelocity;
    Vector3 jumpVelocityOg;
    Vector3 dashVelocity;
    Vector3 dashVelocityOg;

    bool groundedPlayer;
    bool canBeat = false;
    bool hitBeat = false;
    bool hitPenalty = false;
    bool slamming = false;
    bool dashing = false;
    int HP = 1;
    int dashCounter;
    int grooveMeter;
    int grooveTimer;
    int timesGrounded;
    float originalGravity;
    float frictionForce;
    float boopElapsedTime;
    float jumpElapsedTime;
    float dashElapsedTime;
    float currentPlayerSpeed;

    void Start()
    {
        GameManager.instance.OnRestartEvent += Restart;
        GameManager.instance.OnBeatEvent += DoBeat;

        startPos = GameManager.instance.GetPlayerSpawn().transform;
        ghost = new GameObject("ghost");
        ghost.transform.position = startPos.position;

        GameManager.instance.playerDead = false;
        SpawnPlayer();

        speedLines = Instantiate(speedLinesPrefab, transform.position, Quaternion.Euler(Vector3.zero));
        speedLineMat = speedLines.GetComponent<Renderer>().material;

        originalGravity = gravityValue;
        currentPlayerSpeed = playerSpeed;
    }

    void Update()
    {
        if (!GameManager.instance.isPaused)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            if (GameManager.instance.beatWindow)
            {
                if (!canBeat) // First update in window
                {
                    if (dashCounter > 0)
                    {
                        dashCounter--;
                    }
                }

                canBeat = true;
            }
            else
            {
                if (canBeat) // First update off window
                {
                    hitPenalty = false;
                    hitBeat = false;
                }

                canBeat = false;
            }
        }
        else if (HP <= 0)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }

        // DebugRaycastCone();
    }

    void LateUpdate()
    {
        if (!GameManager.instance.isPaused)
        {
            ShootInput();
            MovePlayer();
            DashInput();
            SlamInput();
            UpdateSpeedLines();

            ghost.transform.position = transform.position;
        }
    }

    void UpdateSpeedLines()
    {
        speedLines.transform.position = transform.position;
        Vector3 direction = ((controller.velocity) + (dashVelocity) + (move * currentPlayerSpeed));
        if (direction != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction) * Quaternion.Euler(-90f, 0f, 0f);
            speedLines.transform.rotation = Quaternion.Slerp(speedLines.transform.rotation, targetRot, speedLineRotSpeed * Time.deltaTime);
        }

        float magnitude = direction.magnitude;

        float targetAlpha = Mathf.InverseLerp(speedFadeIn, speedFadeOut, magnitude);

        Color materialColor = speedLineMat.color;
        materialColor.a = Mathf.Lerp(materialColor.a, targetAlpha, speedLineRotSpeed * Time.deltaTime);
        speedLineMat.color = materialColor;
    }

    public void Ground(Transform ground)
    {
        timesGrounded++;

        ghost.transform.parent = ground;
        ghost.transform.position = transform.position;

        groundedPlayer = true;
        frictionForce = frictionGround;
    }

    public void Unground()
    {
        timesGrounded--;
        
        ghost.transform.parent = null;
        
        if (timesGrounded <= 0)
        {
            timesGrounded = 0;
            
            groundedPlayer = false;
            frictionForce = frictionAir; 
        }
    }

    void Restart()
    {
        Destroy(ghost);
        ghost = new GameObject("ghost");
        SpawnPlayer();

        playerVelocity = Vector3.zero;
        jumpVelocity = Vector3.zero;
        boopVelocity = Vector3.zero;
        jumpElapsedTime = 1;
        boopElapsedTime = 1;

        slamming = false;
        gravityValue = originalGravity;

        BoopPenalty();
    }

    void MovePlayer()
    {
        move = Input.GetAxis("Horizontal") * transform.right +
               Input.GetAxis("Vertical") * transform.forward;

        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            if (!hitBeat && !hitPenalty)
            {
                hitBeat = true;
                AudioManager.instance.audioSource.PlayOneShot(jumpSFX);

                jumpVelocity.y += jumpHeight;
                jumpVelocityOg = jumpVelocity;
                jumpElapsedTime = 0f;

                if (boopVelocity.y < 0f)
                {
                    boopVelocityOg.y = 0f;
                }

                CheckGroove();
            }
            else
            {
                BoopPenalty();
            }
        }

        boopElapsedTime += Time.deltaTime;
        boopVelocity = Vector3.Lerp(boopVelocityOg, Vector3.zero, boopElapsedTime);
        boopVelocity.x *= frictionForce;
        boopVelocity.z *= frictionForce;
        boopVelocityOg.x *= frictionForce;
        boopVelocityOg.z *= frictionForce;
        Vector3 boopVector = new Vector3(boopVelocity.x, 0, boopVelocity.z);

        // Cancel boop velocity if you move in the opposite direction or stop moving
        if (move.normalized.magnitude > 0)
        {
            if (move.x == 0f || Mathf.Sign(move.x) != Mathf.Sign(boopVector.x))
            {
                boopVelocityOg.x = 0f;
            }

            if (move.z == 0f || Mathf.Sign(move.z) != Mathf.Sign(boopVector.z))
            {
                boopVelocityOg.z = 0f;
            }
        }

        if (!dashing && !slamming)
        {
            playerVelocity.x *= frictionForce;
            playerVelocity.z *= frictionForce;

            translation = (ghost.transform.position - transform.position);

            dashElapsedTime += Time.deltaTime * 4;
            dashVelocity = Vector3.Lerp(dashVelocityOg, Vector3.zero, dashElapsedTime);

            controller.Move(((move * currentPlayerSpeed + playerVelocity + boopVector + dashVelocity) * Time.deltaTime) + translation);
        }
        else if (dashing)
        {
            controller.Move(dashVelocity * Time.deltaTime);
        }

        CheckHeadHit();

        if (!groundedPlayer)
        {
            playerVelocity.y += gravityValue * Time.deltaTime;
        }
        else
        {
            playerVelocity.y = 0f;
        }

        boopVector = new Vector3(0, boopVelocity.y, 0);

        jumpElapsedTime += Time.deltaTime;
        jumpVelocity = Vector3.Lerp(jumpVelocityOg, Vector3.zero, jumpElapsedTime);
        Vector3 verticalVelocity = (playerVelocity + boopVector + jumpVelocity) * Time.deltaTime;

        controller.Move(verticalVelocity);
    }

    void CheckHeadHit()
    {
        RaycastHit hit;
        if (Physics.Raycast(headPos.transform.position, Vector3.up, out hit, 0.4f))
        {
            if (hit.transform.position != transform.position && !hit.collider.isTrigger)
            {
                playerVelocity.y = -playerVelocity.y;
            }
        }
    }

    void ShootInput()
    {
        if (Input.GetButtonDown("Shoot"))
        {
            if (!hitBeat && !hitPenalty)
            {
                hitBeat = true;
                Boop();

                CheckGroove();
            }
            else
            {
                BoopPenalty();
            }
        }
    }

    void Boop()
    {
        AudioManager.instance.audioSource.PlayOneShot(boopSFX);

        List<Vector3> points = new List<Vector3>();
        List<IBoop> targets = new List<IBoop>();

        // Make raycast cone
        Vector3 origin = Camera.main.transform.position;
        Quaternion rotation = Camera.main.transform.rotation * Quaternion.Euler(90f, 0f, 0f);

        void MakeCone(int coneAngle)
        {
            for (int i = 0; i < (rayCount); i++)
            {
                float angle = i * (360f / rayCount); // Calculate angle for each ray

                // Convert angle to radians
                float angleRad = Mathf.Deg2Rad * angle;

                // Calculate spherical coordinates
                float x = Mathf.Sin(angleRad) * Mathf.Cos(Mathf.Deg2Rad * coneAngle);
                float y = Mathf.Sin(Mathf.Deg2Rad * coneAngle);
                float z = Mathf.Cos(angleRad) * Mathf.Cos(Mathf.Deg2Rad * coneAngle);

                // Combine coordinates to get the direction vector
                Vector3 direction = new Vector3(x, y, z);

                // Rotate the direction using the camera's rotation matrix
                direction = rotation * direction;

                // Perform the raycast
                Ray ray = new Ray(origin, direction.normalized);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, rayDistance, coneLayerMask))
                {
                    Debug.DrawLine(ray.origin, hit.point, Color.red);

                    points.Add(hit.point);

                    IBoop boopable = hit.collider.GetComponent<IBoop>();
                    if (hit.transform != transform && boopable != null)
                    {
                        if (!targets.Contains(boopable))
                        {
                            targets.Add(boopable);
                        }
                    }
                }
                else
                {
                    Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.green);
                }
            }
        }

        MakeCone(coneOuterAngle);
        MakeCone(coneInnerAngle);
        MakeCone(coneOuterInnerAngle);

        // Center raycast
        Ray centerRay = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit centerHit;
        if (Physics.Raycast(centerRay, out centerHit, rayDistance, coneLayerMask))
        {
            Debug.DrawLine(centerRay.origin, centerHit.point, Color.magenta);

            points.Add(centerHit.point);

            IBoop boopable = centerHit.collider.GetComponent<IBoop>();
            if (centerHit.transform != transform && boopable != null)
            {
                if (!targets.Contains(boopable))
                {
                    targets.Add(boopable);
                }
            }
        }
        else
        {
            Debug.DrawRay(centerRay.origin, centerRay.direction * rayDistance, Color.blue);
        }

        Vector3 midpoint = Vector3.zero;

        if (points.Count > 0)
        {
            // Initialize the sum
            Vector3 sum = Vector3.zero;

            // Sum up all the points
            foreach (Vector3 point in points)
            {
                sum += point;
            }

            // Divide by the number of points to get the average (midpoint)
            midpoint = sum / points.Count;

            // Boop player
            DoBoop(midpoint, boopForce);
        }

        Debug.DrawLine(midpoint, origin, Color.yellow);

        foreach (IBoop target in targets)
        {
            target.DoBoop(transform.position, boopForce);
        }

        Instantiate(boopCard, shootPos.position, shootPos.rotation);
    }

    void DebugRaycastCone()
    {
        List<Vector3> points = new List<Vector3>();

        // Make raycast cone
        Vector3 origin = Camera.main.transform.position;
        Quaternion rotation = Camera.main.transform.rotation * Quaternion.Euler(90f, 0f, 0f);
        
        void MakeCone(int coneAngle)
        {
            for (int i = 0; i < (rayCount); i++)
            {
                float angle = i * (360f / rayCount); // Calculate angle for each ray

                // Convert angle to radians
                float angleRad = Mathf.Deg2Rad * angle;

                // Calculate spherical coordinates
                float x = Mathf.Sin(angleRad) * Mathf.Cos(Mathf.Deg2Rad * coneAngle);
                float y = Mathf.Sin(Mathf.Deg2Rad * coneAngle);
                float z = Mathf.Cos(angleRad) * Mathf.Cos(Mathf.Deg2Rad * coneAngle);

                // Combine coordinates to get the direction vector
                Vector3 direction = new Vector3(x, y, z);

                // Rotate the direction using the camera's rotation matrix
                direction = rotation * direction;

                // Perform the raycast
                Ray ray = new Ray(origin, direction.normalized);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, rayDistance, coneLayerMask))
                {
                    Debug.DrawLine(ray.origin, hit.point, Color.red);

                    points.Add(hit.point);
                }
                else
                {
                    Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.green);
                }
            }
        }

        MakeCone(coneOuterAngle);
        MakeCone(coneInnerAngle);
        MakeCone(coneOuterInnerAngle);

        // Center raycast
        Ray centerRay = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit centerHit;
        if (Physics.Raycast(centerRay, out centerHit, rayDistance, coneLayerMask))
        {
            Debug.DrawLine(centerRay.origin, centerHit.point, Color.magenta);

            points.Add(centerHit.point);
        }
        else
        {
            Debug.DrawRay(centerRay.origin, centerRay.direction * rayDistance, Color.blue);
        }

        Vector3 midpoint = Vector3.zero;

        if (points.Count > 0)
        {
            // Initialize the sum
            Vector3 sum = Vector3.zero;

            // Sum up all the points
            foreach (Vector3 point in points)
            {
                sum += point;
            }

            // Divide by the number of points to get the average (midpoint)
            midpoint = sum / points.Count;
        }

        Debug.DrawLine(midpoint, origin, Color.yellow);
    }

    void BoopPenalty()
    {
        AudioManager.instance.playOnce(blastPenaltySFX);

        hitPenalty = true;
        grooveMeter = 0;
        grooveTimer = 0;

        currentPlayerSpeed = playerSpeed;
        GameManager.instance.ToggleGrooveEdge(false);
    }

    public void takeDamage(int amount)
    {
        HP -= amount;

        if (damageSFX != null)
        {
            AudioManager.instance.audioSource.PlayOneShot(damageSFX);
        }

        if (HP <= 0)
        {
            AudioManager.instance.audioSource.PlayOneShot(deathSFX);
            GameManager.instance.playerDead = true;
            GameManager.instance.PopupLose();

            if (GameManager.instance.doubleTimeActive)
            {
                GameManager.instance.DeactivateDoubleTimePowerUp();
            }
        }
    }

    public void SpawnPlayer()
    {
        controller.enabled = false;

        ghost.transform.position = startPos.position;
        transform.position = startPos.position;
        transform.rotation = startPos.rotation;

        controller.enabled = true;

        HP = 1;
    }

    void DashInput()
    {
        if (Input.GetButtonDown("Dash") && move.normalized.magnitude > 0)
        {
            if (!hitBeat && !hitPenalty && dashCounter <= 0)
            {
                if (slamming)
                {
                    slamming = false;
                    gravityValue = originalGravity;
                }

                hitBeat = true;
                AudioManager.instance.playOnce(dashSFX);
                //StartCoroutine(GameManager.instance.FlashLines(dashDuration));
                StartCoroutine(DoDash());

                CheckGroove();
            }
            else
            {
                BoopPenalty();
            }
        }
    }

    IEnumerator DoDash()
    {
        dashing = true;

        Vector3 dashDirection = move.normalized;

        playerVelocity.y = 0;
        boopVelocityOg = Vector3.zero;
        jumpVelocityOg = Vector3.zero;

        dashCounter = dashCooldown;
        dashVelocity = dashDirection * dashSpeed;
        dashVelocityOg = dashVelocity;
        dashElapsedTime = 0;

        yield return new WaitForSeconds(dashDuration);

        dashing = false;
    }

    void SlamInput()
    {
        if (Input.GetButtonDown("Slam") && !groundedPlayer && !slamming)
        {
            if (!hitBeat && !hitPenalty)
            {
                hitBeat = true;
                AudioManager.instance.audioSource.PlayOneShot(slamStartSFX);
                StartCoroutine(DoSlam());

                CheckGroove();
            }
            else
            {
                BoopPenalty();
            }
        }
    }

    IEnumerator DoSlam()
    {
        slamming = true;

        // Disable player horizontal control and gravity influence here if desired
        gravityValue = 0;
        boopVelocityOg = Vector3.zero;
        jumpVelocityOg = Vector3.zero;

        // Apply the ground pound speed directly downwards
        playerVelocity.y = -slamSpeed;

        // Wait until the player hits the ground
        while (!groundedPlayer)
        {
            yield return null;
        }

        // Apply the slam impact logic (e.g., damage enemies, create an impact effect, etc.)
        if (slamming)
        {
            AudioManager.instance.audioSource.PlayOneShot(slamSFX);
            SlamImpact(); 
        }

        // Reset gravity influence
        gravityValue = originalGravity;

        slamming = false;
    }

    void SlamImpact()
    {
        RaycastHit hit;
        if (Physics.BoxCast(transform.position - Vector3.down, Vector3.one, Vector3.down, out hit))
        {
            if (!hit.collider.isTrigger)
            {
                IBoop boopable = hit.collider.GetComponent<IBoop>();

                if (hit.transform != transform && boopable != null)
                {
                    boopable.DoBoop(hit.normal, boopForce, true);
                    DoBoop(hit.normal, boopForce);
                }
            }
        }
    }

    void CheckGroove()
    {
        if (grooveMeter < 4)
        {
            grooveMeter++;
            if (grooveMeter == 4) // Is Grooving
            {
                // Apply Groove Effects
                currentPlayerSpeed = groovePlayerSpeed;
                GameManager.instance.ToggleGrooveEdge(true);
            }
        }

        grooveTimer = 0;

        if (!canBeat)
        {
            BoopPenalty();
        }
    }

    void DoBeat()
    {
        if (grooveMeter > 0)
        {
            grooveTimer++;
        }

        if (grooveTimer > 4 && grooveMeter == 4)
        {
            BoopPenalty();
        }
        else if (grooveTimer > 4)
        {
            grooveMeter = 0;
            grooveTimer = 0;
        }
    }

    public void DoBoop(Vector3 origin, float force, bool slam = false)
    {
        if (slamming)
        {
            slamming = false;
            gravityValue = originalGravity;
        }

        playerVelocity.y = 0;
        boopVelocity = -(origin - transform.position).normalized * force;
        boopVelocityOg = boopVelocity;
        boopElapsedTime = 0f;
    }
}