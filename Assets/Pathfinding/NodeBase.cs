using UnityEngine;

public abstract class NodeBase : MonoBehaviour
{
    private NodeBase m_Connection = null;
    public NodeBase Connection { get { return m_Connection; } set { m_Connection = value; } }

    private float m_G = 0;
    private float m_H = 0;
    public float G { get { return m_G; } set { m_G = value; } }
    public float H { get { return m_H; } set { m_H = value; } }

    public float F => m_G + m_H;

    protected int m_Row = -1;
    protected int m_Col = -1;
    public int Row { get { return m_Row; } }
    public int Col { get { return m_Col; } }

    public Vector2Int GridPosition { get { return new Vector2Int(m_Col, m_Row); } }

    public void SetConnection(NodeBase nodeBase) => m_Connection = nodeBase;
    public virtual void SetG(float g) => G = g;
    public virtual void SetH(float h) => H = h;

    //public abstract void Initialize<T>(T data, int col = -1, int row = -1);
}
