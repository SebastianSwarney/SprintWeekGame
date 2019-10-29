using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    public LayerMask m_wallMask;

    Rigidbody2D m_rigidbody;

    private float m_launchForce;

    private Vector2 m_aimInput;

    public bool m_hasBounced;

    public float m_maxLaunchForce;

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

    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (m_aimInput.normalized.magnitude > 0)
        {
            if (!m_isAiming)
            {
                OnAimInput();
            }
        }

        if (m_aimInput.normalized.magnitude == 0)
        {
            if (m_isAiming)
            {
                ReleaseAimInput();
            }
        }

        AimCrosshair();

        ResetBounce();
    }

    public void SetAimInput(Vector2 p_input)
    {
        m_aimInput = p_input;
    }

    public void OnLaunchInputDown()
    {
        //Launch();
    }

    private void AimCrosshair()
    {
        float theta = Mathf.Atan2(-m_aimInput.y, -m_aimInput.x);

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

    private void OnAimInput()
    {
        m_isAiming = true;

        StartCoroutine(ChargeMove());
    }

    private void ReleaseAimInput()
    {
        m_isAiming = false;
    }

    private IEnumerator ChargeMove()
    {
        float t = 0;

        float currentLaunchForce = 0;

        while (m_isAiming)
        {
            Debug.Log(currentLaunchForce);

            t += Time.deltaTime;

            float progress = m_chargeUpCurve.Evaluate(t / m_speedChargeUpTime);

            currentLaunchForce = Mathf.Lerp(0, m_maxLaunchForce, progress);

            m_lastAim = -m_aimInput;

            yield return null;
        }

        Launch(currentLaunchForce, m_lastAim);
    }

    private void Launch(float p_launchForce, Vector2 p_direction)
    {
        m_rigidbody.velocity = Vector2.zero;

        m_rigidbody.angularVelocity = 0f;

        m_rigidbody.AddForce(p_direction * p_launchForce, ForceMode2D.Impulse);
    }

    private void ResetBounce()
    {
        if (m_hasBounced)
        {
            m_bounceResetTimer += Time.deltaTime;

            if (m_bounceResetTimer >= m_bounceResetTime)
            {
                m_hasBounced = false;
                m_bounceResetTimer = 0;
            }
        }
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
            m_hasBounced = true;
        }
    }
}
