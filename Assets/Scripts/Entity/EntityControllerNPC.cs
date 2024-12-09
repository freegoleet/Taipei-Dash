using System;
using System.Collections.Generic;
using UnityEngine;

public class EntityControllerNPC
{
    public float MovementSpeed { get; private set; } = 0.5f;

    private float m_Timer = 0.0f;
    private Entity m_Entity = null;
    private TileGameplay m_NextTile = null;
    private TileGameplay m_StartTile = null;
    private TileGameplay[] m_Path = null;
    private int m_CurrentPathIndex = -1;
    private bool m_IsMoving = false;

    public Action StepCompleted = null;
    public Action PathFinished = null;

    public EntityControllerNPC(Entity entity) {
        m_Entity = entity;
        StepCompleted += MoveNext;
        PathFinished += FinishedMoving;
        m_Timer = MovementSpeed;
    }

    public void Tick(float dt) {
        Step(dt);
    }

    public bool CancelAction() {
        if (m_IsMoving == false) {
            return false;
        }

        StopMoving();

        return true;
    }

    public void MoveEntity(List<TileGameplay> path) {
        if (m_IsMoving == true) {
            return;
        }

        m_StartTile = path[0];

        for (int i = 0; i < m_Path.Length; i++) {
            m_Path[i] = path[i];
        }

        m_IsMoving = true;
        m_CurrentPathIndex = m_Path.Length - 1;
        MoveNext();
    }

    private void MoveNext() {
        if (m_StartTile != null) {
            m_StartTile.Entity = null;
        }

        m_StartTile = m_Entity.Tile;
        m_StartTile.Entity = m_Entity;

        if (m_CurrentPathIndex < 0) {
            PathFinished?.Invoke();
            return;
        }

        if (m_Path[m_CurrentPathIndex] != null) {
            m_NextTile = m_Path[m_CurrentPathIndex];
            m_Entity.Tile = m_NextTile;
            m_Timer = 0f;
            m_CurrentPathIndex--;
            return;
        }
    }

    private void Step(float dt) {
        if (m_IsMoving == false) {
            return;
        }

        if (m_Timer < MovementSpeed) {
            m_Timer += dt;
            m_Entity.gameObject.transform.position = Vector3.Lerp(m_StartTile.transform.position, m_NextTile.transform.position, m_Timer / MovementSpeed);
            return;
        }

        StepCompleted?.Invoke();
    }

    private void StopMoving() {
        m_Path = null;
        m_CurrentPathIndex = -1;
    }

    private void FinishedMoving() {
        m_IsMoving = false;

        m_StartTile.Entity = null;
        m_StartTile = m_Entity.Tile;
        m_StartTile.Entity = m_Entity;
    }
}
