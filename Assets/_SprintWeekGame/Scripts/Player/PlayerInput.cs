using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerInput : MonoBehaviour
{
    public int m_playerId;

    private PlayerMovementController m_playerMovementController;
    private Player m_playerInputController;

    private bool m_doTheLookyLook;

    private void Start()
    {
        m_playerMovementController = GetComponent<PlayerMovementController>();
        m_playerInputController = ReInput.players.GetPlayer(m_playerId);
    }

    private void Update()
    {
        GetInput();
    }

    public void GetInput()
    {
        Vector2 aimInput = new Vector2(m_playerInputController.GetAxisRaw("Aim Horizontal"), m_playerInputController.GetAxisRaw("Aim Vertical"));
        m_playerMovementController.SetAimInput(aimInput);

        Vector2 moveInput = new Vector2(m_playerInputController.GetAxisRaw("Move Horizontal"), m_playerInputController.GetAxisRaw("Move Vertical"));
        m_playerMovementController.SetMoveInput(moveInput);

        if (m_playerInputController.GetButtonDown("Launch"))
        {
            m_playerMovementController.OnLaunchInputDown();
        }

        if (m_playerInputController.GetButtonDown("Slow Down"))
        {
            m_playerMovementController.OnSlowInputDown();
        }

        if (m_playerInputController.GetButtonUp("Slow Down"))
        {
            m_playerMovementController.OnSlowInputUp();
        }

    }
}