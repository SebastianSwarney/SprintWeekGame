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

    private PlayerMovementController m_movementController;

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
        UpdateScore();

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

        iTween.PunchScale(m_scoreText.gameObject, Vector3.one * 2, m_fadeInTime - 0.1f);


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

    public void UpdateScore()
    {
        
        if (m_movementController.m_lastHitPlayer != null)
        {
            m_movementController.m_lastHitPlayer.m_currentScore++;
        }
        else
        {
            m_currentScore--;
        }

        m_scoreText.text = m_currentScore.ToString();
        
    }
}
