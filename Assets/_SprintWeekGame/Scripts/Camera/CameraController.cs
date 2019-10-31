using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float m_winZoomTime;
    public float m_cameraZoomAmount;

    public AnimationCurve m_winZoomCurve;

    public static CameraController m_instance;

    public CameraFade m_fadeImage;

    public AnimationCurve m_fadeCurve;

    public float m_fadeInTime;

    private Camera m_camera;

    public List<GameObject> m_gameWonHideObjects;

    public AnimationCurve m_critZoomCurve;

    public float m_critZoomAmount;

    public float m_critZoomTime;
    private float m_critZoomTimer;

    public float m_cirtZoomOutTime;

    private bool m_isCritZooming;

    private float m_startCameraSize;

    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        

        m_camera = GetComponent<Camera>();

        m_startCameraSize = m_camera.orthographicSize;

        StartCoroutine(FadeIn());

        foreach (PlayerGameComponent item in PlayerManager.m_instance.m_players)
        {
            m_gameWonHideObjects.Add(item.GetComponentInChildren<Canvas>().gameObject);
        }
    }

    public void WinZoomToPlayer(int p_playerId)
    {
        PlayerGameComponent player = PlayerManager.m_instance.m_players[p_playerId];

        StartCoroutine(ZoomCamera(player));
    }

    private IEnumerator ZoomCamera(PlayerGameComponent p_player)
    {
        foreach (GameObject item in m_gameWonHideObjects)
        {
            item.SetActive(false);
        }

        float t = 0;

        float m_startCameraSize = m_camera.orthographicSize;

        while (t < m_winZoomTime)
        {
            t += Time.deltaTime;


            float progress = m_winZoomCurve.Evaluate(t / m_winZoomTime);

            transform.position = Vector3.Lerp(transform.position, p_player.transform.position + Vector3.forward * -10, progress);

            m_camera.orthographicSize = Mathf.Lerp(m_startCameraSize, m_cameraZoomAmount, progress);

            yield return null;
        }

        StartCoroutine(FadeOut());
    }

    public void RunCritCamera(Transform p_player)
    {

        if (!m_isCritZooming)
        {
            StartCoroutine(CritZoomCamera(p_player));
        }
    }

    private IEnumerator CritZoomCamera(Transform p_player)
    {
        transform.position = Vector3.zero + Vector3.forward * -10;

        m_isCritZooming = true;

        TimeManager.m_instance.RunCritSlowMo();

        m_critZoomTimer = 0;

        Vector3 startPos = transform.position;

        while (m_critZoomTimer < m_critZoomTime)
        {
            m_critZoomTimer += Time.deltaTime;

            float progress = m_critZoomCurve.Evaluate(m_critZoomTimer / m_critZoomTime);

            transform.position = Vector3.Lerp(startPos, p_player.position + Vector3.forward * -10, progress);

            m_camera.orthographicSize = Mathf.Lerp(m_startCameraSize, m_critZoomAmount, progress);

            yield return null;
        }

        float t = 0;

        while (t < m_cirtZoomOutTime)
        {
            t += Time.deltaTime;

            float progress = m_critZoomCurve.Evaluate(t / m_critZoomTime);

            transform.position = Vector3.Lerp(p_player.position + Vector3.forward * -10, startPos, progress);

            m_camera.orthographicSize = Mathf.Lerp(m_critZoomAmount, m_startCameraSize, progress);

            yield return null;
        }

        m_camera.orthographicSize = m_startCameraSize;

        transform.position = Vector3.zero + Vector3.forward * -10;

        m_isCritZooming = false;
    }

    private IEnumerator FadeIn()
    {
        float t = 0;

        while (t < m_fadeInTime)
        {
            t += Time.deltaTime;

            float progress = m_fadeCurve.Evaluate(t / m_fadeInTime);

            m_fadeImage.FindFadeProgress(progress);

            yield return null;
        }
    }

    private IEnumerator FadeOut()
    {
        float t = 0;

        while (t < m_fadeInTime)
        {
            t += Time.deltaTime;

            float progress = m_fadeCurve.Evaluate(t / m_fadeInTime);

            m_fadeImage.FindReverseProgress(progress);

            yield return null;
        }

        GameManager.m_instance.RoundWon();
    }
}
