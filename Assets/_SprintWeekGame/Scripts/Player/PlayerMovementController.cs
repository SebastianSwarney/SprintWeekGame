using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    public enum MovementControllState {MovementEnabled, MovementDisabled}
    public MovementControllState m_movementControll;

    public LayerMask m_wallMask;

    [HideInInspector]
    public Rigidbody2D m_rigidbody;

    private float m_launchForce;

    private Vector2 m_aimInput;

    public bool m_hasBounced;

    public float m_maxLaunchForce;
    public float m_minLaunchForce;

    public AnimationCurve m_chargeUpCurve;
    public float m_speedChargeUpTime;
    private float m_speedChargeUpTimer;

    public float m_bounceResetTime;
    private float m_bounceResetTimer;


    public Transform m_aimObject;
    public float m_crosshairDistance;

    private bool m_isAiming;

    private Vector2 m_lastAim;
    private Vector3 m_lastPos;

    public float m_chargeDistance;

    public LayerMask m_playerMask;

    [HideInInspector]
    public PlayerGameComponent m_lastHitPlayer;

    public float m_pushForce;

    public float m_pushRadius;

    private LineRenderer m_targetLine;

    public GameObject m_partical;

    public float m_launchBufferTime;
    private float m_launchBufferTimer;

    private Coroutine m_launchBufferCoroutine;

    public LerpColor m_bounceRechargeVisual;

    public GameObject m_hitPlayerEffect;

    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();

        m_targetLine = GetComponent<LineRenderer>();

        m_targetLine.enabled = false;

        m_launchBufferCoroutine = StartCoroutine(RunBufferTimer((x) => m_launchBufferTimer = (x), m_launchBufferTime));
    }

    private void Update()
    {
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

        ResetBounce();
    }

    public void SetAimInput(Vector2 p_input)
    {
        m_aimInput = p_input;
    }

    public void OnLaunchInputDown()
    {
        Push();
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

    private void AimCrosshair()
    {
        float theta = Mathf.Atan2(m_aimInput.y, m_aimInput.x);

        float aimDegrees = theta * Mathf.Rad2Deg;

        Vector3 pCircle = new Vector3(Mathf.Cos(theta), Mathf.Sin(theta), 0) * m_crosshairDistance;

        if (m_aimInput.normalized.magnitude != 0)
        {
            m_aimObject.rotation = Quaternion.Euler(0, 0, aimDegrees);
            m_aimObject.position = transform.position + pCircle;
            m_lastPos = pCircle;
        }
        else
        {
            m_aimObject.position = transform.position + m_lastPos;
        }
    }

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

    private IEnumerator ChargeMove()
    {
        m_targetLine.enabled = true;

        float t = 0;

        float currentLaunchForce = 0;

        LerpScale arrowLerp = m_aimObject.GetComponent<LerpScale>();

        Vector2 startVelocity = m_rigidbody.velocity;

        while (m_isAiming)
        {
            t += Time.deltaTime;

            float progress = m_chargeUpCurve.Evaluate(t / m_speedChargeUpTime);
            currentLaunchForce = Mathf.Lerp(m_minLaunchForce, m_maxLaunchForce, progress);

            Vector3 aimTarget = ((Vector3)m_lastAim.normalized * m_chargeDistance) + transform.position;
            Vector3 targetPos = Vector3.Lerp(transform.position, aimTarget, progress);

            //m_rigidbody.velocity = Vector3.Lerp(startVelocity, Vector2.zero, progress);

            m_aimObject.position = targetPos;

            m_targetLine.SetPosition(0, transform.position);
            m_targetLine.SetPosition(1, targetPos);

            arrowLerp.FindLerpProgress(progress);

            yield return null;
        }

        arrowLerp.ResetScale();

        m_targetLine.enabled = false;

        Launch(currentLaunchForce, m_lastAim);
    }

    private void Launch(float p_launchForce, Vector2 p_direction)
    {
        m_rigidbody.velocity = Vector2.zero;
        m_rigidbody.angularVelocity = 0f;
        m_rigidbody.AddForce(p_direction * p_launchForce, ForceMode2D.Impulse);
    }

    private void Push()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, m_pushRadius, m_playerMask);

        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject != gameObject)
            {
                collider.GetComponentInParent<Rigidbody2D>().AddForce(-(transform.position - collider.transform.position) * m_pushForce, ForceMode2D.Impulse);
            }
        }

        //m_rigidbody.velocity = Vector2.zero;
        //m_rigidbody.angularVelocity = 0f;
    }

    private void ResetBounce()
    {
        if (m_hasBounced)
        {
            m_bounceResetTimer += Time.deltaTime;

            m_bounceRechargeVisual.FindReverseProgress((m_bounceResetTimer / m_bounceResetTime));

            if (m_bounceResetTimer >= m_bounceResetTime)
            {
                m_hasBounced = false;
                m_bounceResetTimer = 0;
            }
        }
    }

    public void KillPlayer()
    {
        m_isAiming = false;

        m_rigidbody.velocity = Vector2.zero;
        m_rigidbody.angularVelocity = 0f;

        m_rigidbody.simulated = false;

        m_movementControll = MovementControllState.MovementDisabled;

    }

    public void Respawn()
    {
        m_rigidbody.simulated = true;
        m_movementControll = MovementControllState.MovementEnabled;
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
            iTween.ShakePosition(Camera.main.gameObject, Vector3.one / 2, 0.5f);

            Instantiate(m_partical, transform.position, Quaternion.identity);
            m_hasBounced = true;
        }

        if (CheckCollisionLayer(m_playerMask, collision.gameObject))
        {
            iTween.ShakePosition(Camera.main.gameObject, Vector3.one, 0.5f);

            Instantiate(m_hitPlayerEffect, transform.position, Quaternion.identity);
            m_lastHitPlayer = collision.gameObject.GetComponent<PlayerGameComponent>();

        }
    }
}
