using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerGameComponent : MonoBehaviour
{
    public int m_lives;

    public float m_respawnTime;

    public float m_fadeInTime;

    private Vector3 m_spawnPosition;

    [HideInInspector]
    public PlayerMovementController m_movementController;

    private TextMeshProUGUI m_scoreText;

    public LerpColor m_mainSprite;

    public List<GameObject> m_componentsToHide;

    public GameObject m_scoreShakeObject;

    public float m_deathShakeAmount;
    public float m_deathScaleAmount;
    public float m_deathShakeTime;

    [HideInInspector]
    public bool m_isDead;

    public void PlayerSetup()
    {
        m_scoreText = GetComponentInChildren<TextMeshProUGUI>();
        m_movementController = GetComponent<PlayerMovementController>();
        m_spawnPosition = transform.position;

        m_scoreText.text = m_lives.ToString();
    }

    public void KillPlayer()
    {
        m_isDead = true;

        DecreaseLives();

        iTween.PunchScale(m_scoreShakeObject, Vector3.one * m_deathScaleAmount, m_deathShakeTime);
        iTween.PunchRotation(m_scoreShakeObject, Vector3.one * m_deathShakeAmount, m_deathShakeTime);

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

        m_movementController.Respawn();
        m_isDead = false;

        m_movementController.SetPlaying();

    }

    public void DecreaseLives()
    {
        m_lives--;

        m_scoreText.text = m_lives.ToString();
    }
}
