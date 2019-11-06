using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PlayerMovementEvent : UnityEvent { }

public class PlayerMovementController : MonoBehaviour
{
    public enum MovementControllState {MovementEnabled, MovementDisabled}
    public MovementControllState m_movementControll;

    public enum VulnerableState { Vulnerable, Invulnerable }
    public VulnerableState m_vunerableState;

    #region Events
    [System.Serializable]
    public struct Events
    {
        [Header("Movement Events")]
        public PlayerMovementEvent m_onLaunchChargeEvent;
        public PlayerMovementEvent m_onLaunchEvent;

        [Header("Bounce Events")]
        public PlayerMovementEvent m_onWallBounceEvent;

        [Header("Push Events")]
        public PlayerMovementEvent m_onPushEvent;

        [Header("Drift Events")]
        public PlayerMovementEvent m_onDriftBeginEvent;
        public PlayerMovementEvent m_onDriftEndEvent;

        [Header("Player Death Event")]
        public PlayerMovementEvent m_onPlayerDied;
    }

    public Events m_events;
    #endregion

    #region Movement Properties
    [Header("Movement Properties")]
    public float m_maxLaunchForce;
    public float m_minLaunchForce;
    public AnimationCurve m_chargeUpCurve;
    public float m_speedChargeUpTime;
    public float m_chargeVisualDistance;

    public AnimationCurve m_slowDownCurve;
    public float m_breakSlowSpeed;
    public float m_driftSpeed;
    public float m_maxBreakSlowTime;
    public float m_minBreakSlowTime;
    public float m_breakBufferTime;

    private float m_breakBufferTimer;
    private float m_breakSlowTimer;

    private float m_speedChargeUpTimer;
    private Vector2 m_moveInput;
    private Coroutine m_breakBufferCoroutine;
    [Space]
    #endregion

    #region Bounce Properties
    [Header("Bounce Properties")]
    public LayerMask m_playerMask;
    public LayerMask m_wallMask;
    public int m_playerVulnerableLayer;
    public int m_playerInvulnerableLayer;

    [HideInInspector]
    public PlayerGameComponent m_lastHitPlayer;
    [Space]
    #endregion

    #region Push Properties
    [Header("Push Properties")]
    public float m_maxPushForce;
    public float m_minPushForce;

    public float m_maxPushRadius;
    public float m_minPushRadius;

    public float m_pushBufferTime;
    public float m_pushTime;
    public AnimationCurve m_pushCurve;

    public float m_maxPushStunTime;
    public float m_minPushStunTime;

    public float m_maxPushStunShake;
    public float m_minPushStunShake;

    public LerpScale m_pushVisual;

    public float m_critTreshhold;

    private float m_pushBufferTimer;
    private Coroutine m_pushBufferCorutine;
    private LerpColor m_pushVisualLerp;
    [Space]
    #endregion

    #region Aim Properties
    [Header("Aim Properties")]
    public Transform m_crosshair;
    public float m_crosshairDistance;
    public float m_aimSlowDownSpeed;
    public float m_aimSlowDownTime;
    private float m_aimSlowDownTimer;

    private Vector2 m_lastAim;
    private Vector3 m_lastPos;
    private Vector2 m_aimInput;
    private bool m_isAiming;
    [Space]
    #endregion

    #region Visual Properties
    [Header("Visual Properties")]
    public GameObject m_wallPartical;
    public GameObject m_hitPlayerEffect;
    public GameObject m_playerDeathEffect;

    private LineRenderer m_targetLine;
    [Space]
    #endregion

    public float m_maxSpeed;
    private float m_currentSpeed;

    [HideInInspector]
    public Rigidbody2D m_rigidbody;
    private PlayerGameComponent m_gameComponent;
    [HideInInspector]
    public bool m_hasBeenPushed;

    private bool m_isSlowingDown;
    private bool m_resetAfterLaunch;

    private Collider2D m_collider;

    private void Start()
    {
        m_gameComponent = GetComponent<PlayerGameComponent>();
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_targetLine = GetComponent<LineRenderer>();
        m_pushVisualLerp = m_pushVisual.GetComponent<LerpColor>();
        m_collider = GetComponentInChildren<Collider2D>();

        m_targetLine.enabled = false;
        m_pushBufferCorutine = StartCoroutine(RunBufferTimer((x) => m_pushBufferTimer = (x), m_pushBufferTime));

        m_breakBufferCoroutine = StartCoroutine(RunBufferTimer((x) => m_breakBufferTimer = (x), m_breakBufferTime));
    }

    private void Update()
    {
        if (m_vunerableState == VulnerableState.Vulnerable)
        {
            m_collider.gameObject.layer = m_playerVulnerableLayer;
        }
        else
        {
            m_collider.gameObject.layer = m_playerInvulnerableLayer;
        }

        if (m_movementControll == MovementControllState.MovementEnabled)
        {
            if (m_aimInput != Vector2.zero)
            {
                if (!m_isAiming)
                {
                    OnAimInput();
                }

                if (m_aimInput.magnitude > 0.7f)
                {
                    m_lastAim = m_aimInput;
                }
            }

            if (m_aimInput == Vector2.zero)
            {
                if (m_isAiming)
                {
                    ReleaseAimInput();
                }
            }

            AimCrosshair();
        }

        CalculateSpeed();
    }

    #region Input Code
    public void SetAimInput(Vector2 p_input)
    {
        m_aimInput = p_input;
    }

    public void SetMoveInput(Vector2 p_input)
    {
        m_moveInput = p_input;
    }

    public void OnLaunchInputDown()
    {
        if (m_movementControll == MovementControllState.MovementEnabled)
        {
            if (CheckOverBuffer(ref m_pushBufferTimer, ref m_pushBufferTime, m_pushBufferCorutine))
            {
                StartCoroutine(RunPush());
            }
        }
    }

    private void OnAimInput()
    {
        m_isAiming = true;
        StartCoroutine(ChargeMove());
    }

    private void ReleaseAimInput()
    {
        m_isAiming = false;
    }

    public void OnSlowInputDown()
    {
        if (m_movementControll == MovementControllState.MovementEnabled)
        {
            m_isSlowingDown = true;

            if (CheckOverBuffer(ref m_breakBufferTimer, ref m_breakBufferTime, m_breakBufferCoroutine) || m_resetAfterLaunch)
            {
                StartCoroutine(SlowDown());
                m_resetAfterLaunch = false;
            }
        }
    }

    public void OnSlowInputUp()
    {
        m_isSlowingDown = false;
    }

    private void AimCrosshair()
    {
        float theta = Mathf.Atan2(m_aimInput.y, m_aimInput.x);

        float aimDegrees = theta * Mathf.Rad2Deg;

        Vector3 pCircle = new Vector3(Mathf.Cos(theta), Mathf.Sin(theta), 0) * m_crosshairDistance;

        if (m_aimInput.normalized.magnitude != 0)
        {
            m_crosshair.rotation = Quaternion.Euler(0, 0, aimDegrees);
            m_crosshair.position = transform.position + pCircle;
            m_lastPos = pCircle;
        }
        else
        {
            m_crosshair.position = transform.position + m_lastPos;
        }
    }
    #endregion

    #region Input Buffering Code

    private bool CheckBuffer(ref float p_bufferTimer, ref float p_bufferTime, Coroutine p_bufferTimerRoutine)
    {
        if (p_bufferTimer < p_bufferTime)
        {
            if (p_bufferTimerRoutine != null)
            {
                StopCoroutine(p_bufferTimerRoutine);
            }

            p_bufferTimer = p_bufferTime;

            return true;
        }
        else if (p_bufferTimer >= p_bufferTime)
        {
            return false;
        }

        return false;
    }

    private bool CheckOverBuffer(ref float p_bufferTimer, ref float p_bufferTime, Coroutine p_bufferTimerRoutine)
    {
        if (p_bufferTimer >= p_bufferTime)
        {
            p_bufferTimer = p_bufferTime;

            return true;
        }

        return false;
    }

    private IEnumerator RunBufferTimer(System.Action<float> m_bufferTimerRef, float p_bufferTime)
    {
        float t = 0;

        while (t < p_bufferTime)
        {
            t += Time.deltaTime;
            m_bufferTimerRef(t);
            yield return null;
        }

        m_bufferTimerRef(p_bufferTime);
    }

    #endregion

    #region Basic Movement Code
    private void CalculateSpeed()
    {
        m_currentSpeed = m_rigidbody.velocity.magnitude / m_maxSpeed;
    }

    private IEnumerator ChargeMove()
    {
        m_crosshair.gameObject.SetActive(true);

        m_events.m_onLaunchChargeEvent.Invoke();

        m_targetLine.enabled = true;

        float t = 0;

        float currentLaunchForce = 0;

        LerpScale arrowLerp = m_crosshair.GetComponent<LerpScale>();

        m_aimSlowDownTimer = 0;

        while (m_isAiming)
        {
            t += Time.deltaTime;

            m_aimSlowDownTimer += Time.deltaTime;

            float progress = m_chargeUpCurve.Evaluate(t / m_speedChargeUpTime);
            currentLaunchForce = Mathf.Lerp(m_minLaunchForce, m_maxLaunchForce, progress);

            Vector3 aimTarget = ((Vector3)m_lastAim.normalized * m_chargeVisualDistance) + transform.position;
            Vector3 targetPos = Vector3.Lerp(transform.position, aimTarget, progress);

            float progress2 = m_chargeUpCurve.Evaluate(m_aimSlowDownTimer / m_aimSlowDownTime);

            float aimSlowDownSpeed = Mathf.Lerp(0, m_aimSlowDownSpeed, progress2);

            m_rigidbody.AddForce(-m_rigidbody.velocity * aimSlowDownSpeed, ForceMode2D.Force);

            m_crosshair.position = targetPos;

            m_targetLine.SetPosition(0, transform.position);
            m_targetLine.SetPosition(1, targetPos);

            arrowLerp.FindLerpProgress(progress);

            yield return null;
        }

        arrowLerp.ResetScale();

        m_targetLine.enabled = false;

        Launch(currentLaunchForce, m_lastAim);

        m_resetAfterLaunch = true;

        m_crosshair.gameObject.SetActive(false);
    }

    private void Launch(float p_launchForce, Vector2 p_direction)
    {
        m_events.m_onLaunchEvent.Invoke();

        m_rigidbody.AddForce(p_direction * p_launchForce, ForceMode2D.Impulse);
    }

    private IEnumerator SlowDown()
    {
        m_breakSlowTimer = 0;

        float m_currentBreakTime = Mathf.Lerp(m_minBreakSlowTime, m_maxBreakSlowTime, m_currentSpeed);

        m_events.m_onDriftBeginEvent.Invoke();

        while (m_isSlowingDown)
        {
            m_breakSlowTimer += Time.deltaTime;

            float progress = m_slowDownCurve.Evaluate(m_breakSlowTimer / m_currentBreakTime);

            float driftSpeed = Mathf.Lerp(m_breakSlowSpeed, 0, progress);

            float breakSpeed = Mathf.Lerp(0, m_breakSlowSpeed, progress);

            m_rigidbody.AddForce(-m_rigidbody.velocity * breakSpeed, ForceMode2D.Force);

            m_rigidbody.AddForce(m_moveInput * driftSpeed * 30, ForceMode2D.Force);

            if (m_breakSlowTimer > m_currentBreakTime)
            {
                m_events.m_onDriftEndEvent.Invoke();
            }

            yield return null;
        }

        m_breakBufferCoroutine = StartCoroutine(RunBufferTimer((x) => m_breakBufferTimer = (x), m_breakBufferTime));

        m_events.m_onDriftEndEvent.Invoke();
    }
    #endregion

    #region Push Code
    private IEnumerator RunPush()
    {
        m_events.m_onPushEvent.Invoke();

        float pushRadius = Mathf.Lerp(m_minPushRadius, m_maxPushRadius, m_currentSpeed);

        m_pushVisualLerp.ResetColor();

        float t = 0;

        List<PlayerMovementController> hitPlayers = new List<PlayerMovementController>();

        while (t < m_pushTime)
        {
            t += Time.deltaTime;

            float progress = m_pushCurve.Evaluate(t / m_pushTime);

            float currentRaidus = Mathf.Lerp(0, pushRadius, progress);

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, currentRaidus, m_playerMask);

            foreach (Collider2D collider in colliders)
            {
                if (collider.gameObject.transform.parent.gameObject != gameObject)
                {
                    PlayerMovementController player = collider.GetComponentInParent<PlayerMovementController>();

                    if (!hitPlayers.Contains(player))
                    {
                        hitPlayers.Add(player);

                        if (!player.m_hasBeenPushed)
                        {
                            player.OnPushHit((m_rigidbody.velocity.normalized), m_currentSpeed);
                            //player.OnPushHit(-(transform.position - collider.transform.position), pushForce);
                            player.m_lastHitPlayer = m_gameComponent;
                        }
                    }
                }
            }

            m_pushVisual.SetScaleRadius(currentRaidus);

            yield return null;
        }

        foreach (PlayerMovementController player in hitPlayers)
        {
            player.m_hasBeenPushed = false;
        }

        m_pushVisualLerp.FindFadeProgress(1);

        m_pushVisual.ResetScale();

        m_pushBufferCorutine = StartCoroutine(RunBufferTimer((x) => m_pushBufferTimer = (x), m_pushBufferTime));
    }

    public void OnPushHit(Vector3 p_hitDir, float p_incomingSpeed)
    {
        OnPlayerHit();

        m_hasBeenPushed = true;

        StartCoroutine(RunPushHitStun(p_hitDir, p_incomingSpeed));
    }

    private IEnumerator RunPushHitStun(Vector3 p_hitDir, float p_incomingSpeed)
    {
        if (p_incomingSpeed >= m_critTreshhold)
        {
            CameraController.m_instance.RunCritCamera(transform);
        }

        float stunTime = Mathf.Lerp(m_minPushStunTime, m_maxPushStunTime, p_incomingSpeed);
        float effectAmount = Mathf.Lerp(m_minPushStunShake, m_maxPushStunShake, p_incomingSpeed);
        float pushForce = Mathf.Lerp(m_minPushForce, m_maxPushForce, p_incomingSpeed);

        float t = 0;

        m_movementControll = MovementControllState.MovementDisabled;

        iTween.ShakePosition(gameObject, Vector3.one * effectAmount, stunTime);

        while (t < stunTime)
        {
            m_rigidbody.velocity = Vector2.zero;
            m_rigidbody.angularVelocity = 0f;

            t += Time.deltaTime;

            yield return null;
        }

        m_rigidbody.AddForce(p_hitDir * pushForce, ForceMode2D.Impulse);

        m_movementControll = MovementControllState.MovementEnabled;
    }
    #endregion

    #region Score and Hit Code
    public void OnPlayerHit()
    {
        m_aimSlowDownTimer = 0;
        //m_breakSlowTimer = 0;
    }

    public void KillPlayer()
    {
        Vector2 dir = Vector3.zero - transform.position;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion spawnRot = Quaternion.AngleAxis(angle, Vector3.forward);
        Instantiate(m_playerDeathEffect, transform.position, spawnRot);

        m_isAiming = false;

        m_events.m_onPlayerDied.Invoke();

        m_rigidbody.velocity = Vector2.zero;
        m_rigidbody.angularVelocity = 0f;

        m_rigidbody.simulated = false;

        m_movementControll = MovementControllState.MovementDisabled;
        m_vunerableState = VulnerableState.Invulnerable;

        m_crosshair.gameObject.SetActive(false);

    }

    public void Respawn()
    {
        m_rigidbody.simulated = true;
        m_crosshair.gameObject.SetActive(false);
        m_movementControll = MovementControllState.MovementEnabled;
    }

    public void SetPlaying()
    {
        m_vunerableState = VulnerableState.Vulnerable;
    }

    public bool CheckCollisionLayer(LayerMask p_layerMask, GameObject p_object)
    {
        if (p_layerMask == (p_layerMask | (1 << p_object.layer)))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (CheckCollisionLayer(m_wallMask, collision.gameObject))
        {
            m_events.m_onWallBounceEvent.Invoke();

            Instantiate(m_wallPartical, transform.position, Quaternion.identity);
        }

        if (CheckCollisionLayer(m_playerMask, collision.gameObject))
        {
            Instantiate(m_hitPlayerEffect, transform.position, Quaternion.identity);

            PlayerMovementController player = collision.gameObject.GetComponent<PlayerMovementController>();
            m_lastHitPlayer = player.gameObject.GetComponent<PlayerGameComponent>();
            player.m_lastHitPlayer = m_gameComponent;

            OnPlayerHit();
        }
    }
    #endregion
}
