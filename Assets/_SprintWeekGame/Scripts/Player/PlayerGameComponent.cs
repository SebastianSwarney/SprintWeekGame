using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGameComponent : MonoBehaviour
{
    public int m_currentScore;

    public float m_respawnTime;

    private Vector3 m_spawnPosition;

    private PlayerMovementController m_movementController;

    private void Start()
    {
        m_movementController = GetComponent<PlayerMovementController>();

        m_spawnPosition = transform.position;
    }

    public void Respawn()
    {
        transform.position = m_spawnPosition;
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
    }
}
