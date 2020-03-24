using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FingerM : MonoBehaviour
{
    public enum GestureType
    {
        DRAG = 0,
        PINCH = 1
    };
    /// <summary>
    /// 手势类型
    /// </summary>
    protected GestureType m_eOpStyle;
    /// <summary>
    /// 触摸持续时间
    /// </summary>
    protected float m_fTouchDuration = 0.0f;
    /// <summary>
    /// 触摸点列表
    /// </summary>
    protected List<Vector3> m_v3TouchPostions = new List<Vector3>();
    /// <summary>
    /// 拖动手指的ID
    /// </summary>
    private int m_iDragFingerIndex = -1;
    /// <summary>
    /// 是否处于Pinch状态
    /// </summary>
    protected bool m_bIsPinch = false;
    /// <summary>
    /// 是否处于Drag状态
    /// </summary>
    protected bool m_bIsDrag = false;

    
    

    public void OnDrag(DragGesture gesture)
    {
        //GuiTextDebug.Debug("OnDrag");
        m_bIsPinch = false;
        m_bIsDrag = true;
        
        FingerGestures.Finger finger = gesture.Fingers[0];
        if (gesture.Phase == ContinuousGesturePhase.Started)
        {
            m_iDragFingerIndex = finger.Index;
            ClearData();
            FingerStart(gesture);
        }
        else if (finger.Index == m_iDragFingerIndex)
        {
            if (gesture.Phase == ContinuousGesturePhase.Updated)
            {
                FingerMove(gesture);
            }
            else
            {
                m_iDragFingerIndex = -1;
                //if (m_v3TouchPostions.Count >= 2)
                {
                    FingerUp(gesture);
                }
            }
        }
    }

    public virtual void FingerStart(DragGesture gesture)
    {

    }

    public virtual void FingerMove(DragGesture gesture)
    {

    }

    public virtual void FingerUp(Gesture gesture)
    {
        ClearData();
        m_fTouchDuration = 0;
    }

   /// <summary>
   /// 处理Pinch
   /// </summary>
   /// <param name="gesture"></param>
    public void OnPinch(PinchGesture gesture)
    {
        m_bIsDrag = false;
        m_bIsPinch = true;
        if (gesture.Fingers.Count > 1)
        {
            PinchMove(gesture.Delta.Centimeters());
        }
    }
    /// <summary>
    /// 清空触摸点数据
    /// </summary>
    protected void ClearData()
    {
        m_v3TouchPostions.Clear();
    }

    public virtual void PinchMove(float data)
    {

    }
    /// <summary>
    /// 由屏幕坐标得到z=1的世界坐标
    /// </summary>
    /// <param name="screenPos"></param>
    /// <returns></returns>
    public static Vector3 GetWorldPos(Vector2 screenPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        // we solve for intersection with z = 0 plane
        float t = -ray.origin.z / ray.direction.z;
        return ray.GetPoint(t + 1f);
    }
   
}
