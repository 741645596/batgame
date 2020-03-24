using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Corner : MonoBehaviour
{
    List<LifeProperty> m_beams = new List<LifeProperty>();
    // Use this for initialization
    public Int2 pos;
    public void AddLife(LifeProperty l)
    {
        m_beams.Add(l);
        if (l.GetLife() is IggFloor) {
            IggFloor f = l.GetLife()  as IggFloor;
            f.AddCorner(this);
        }
        if (l.GetLife()  is IggWall) {
            IggWall w = l.GetLife()  as IggWall;
            
            w.AddCorner(this);
        }
    }
    public void RemoveLife(LifeProperty l)
    {
        if (m_beams.Contains(l)) {
            m_beams.Remove(l);
        }
        if (m_beams.Count == 0) {
            Destroy(gameObject);
        }
    }
}
