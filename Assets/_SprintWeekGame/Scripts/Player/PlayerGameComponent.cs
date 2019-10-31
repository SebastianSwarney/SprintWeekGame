using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGameComponent : MonoBehaviour
{
    public int m_currentScore;

    public float m_respawnTime;

    public float m_fadeInTime;

    private Vector3 m_spawnPosition;

    [HideInInspector]
    public PlayerMovementController m_movementController;

    private Text m_scoreText;

    public LerpColor m_mainSprite;

    public List<GameObject> m_componentsToHide;

    private void Start()
    {
        m_scoreText = GetComponentInChildren<Text>();
        m_movementController = GetComponent<PlayerMovementController>();

        m_scoreText.gameObject.SetActive(false);

        m_spawnPosition = transform.position;
    }

    private void OnGUI()
    {
        m_scoreText.rectTransform.position = transform.position;
    }

    public void KillPlayer()
    {
        if (m_movementController.m_lastHitPlayer != null)
        {
            m_movementController.m_lastHitPlayer.IncreaseScore();
        }

        m_movementController.KillPlayer();

        foreach (GameObject item in m_componentsToHide)
        {
            item.SetActive(false);
        }
    }

    public void Respawn()
    {
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        m_movementController.m_rigidbody.velocity = Vector2.zero;
        m_movementController.m_rigidbody.angularVelocity = 0;

        transform.position = m_spawnPosition;

        foreach (GameObject item in m_componentsToHide)
        {
            item.SetActive(true);
        }

        float t = 0;

        while (t < m_fadeInTime)
        {
            t += Time.deltaTime;

            m_mainSprite.FindReverseProgress(t / m_fadeInTime);

            yield return null;
        }

        m_scoreText.gameObject.SetActive(false);

        m_movementController.Respawn();
    }

    public void IncreaseScore()
    {
        m_currentScore++;
    }

    public void DecreaseScore()
    {
        m_currentScore--;
    }
}
