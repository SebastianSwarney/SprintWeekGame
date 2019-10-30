using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float m_winZoomTime;
    public float m_cameraZoomAmount;

    public static CameraController m_instance;

    private Camera m_camera;

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
    }

    public void WinZoomToPlayer(int p_playerId)
    {
        PlayerGameComponent player = PlayerManager.m_instance.m_players[p_playerId];

        iTween.MoveTo(gameObject, player.transform.position + Vector3.forward * -10, m_winZoomTime);

        StartCoroutine(ZoomCamera());
    }

    private IEnumerator ZoomCamera()
    {
        float t = 0;

        float m_startCameraSize = m_camera.orthographicSize;

        while (t < m_winZoomTime)
        {
            t += Time.deltaTime;
            float progress = t / m_winZoomTime;

            m_camera.orthographicSize = Mathf.Lerp(m_startCameraSize, m_cameraZoomAmount, progress);

            yield return null;
        }

        SceneManager.m_instance.LoadScene(SceneManager.m_instance.m_mainScene);
    }
}
