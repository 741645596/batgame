using UnityEngine;
using System.Collections;

public class PetFly1002 : PetFly
{

    private float m_fFallSpeed = 12;
    private float m_fCounter00300 = 0.5f;
    private bool m_bBeginKick = false;
    private float m_fKickCounter = 0f;
    private bool m_bBeginFall = false;
    public float m_fLiveCounter = 0f;
    private const float m_fKickDuration = 0.9f;
    private  float m_fKickSlerpCounter = 0.0f;
    private Vector3 m_bKickDir = Vector3.zero;
    private AnimatorState m_flyState = AnimatorState.FlyClick00300;
    public Pet1002State m_petState = Pet1002State.fly;
    public bool m_bHitFloor = false;
    /// <summary>
    /// 是否被踢过
    /// </summary>
    private bool m_bBeKicked = false;
    // Update is called once per frame
    public override void FixUpdate()
    {
        if (m_bBeginFall) {
            m_fLiveCounter += Time.deltaTime;
            m_Owner.m_Skin.Move(m_flyState, Vector3.down * m_fFallSpeed * Time.deltaTime, ActionMode.Delta);
            if (m_fLiveCounter > 5.0f) {
                m_Owner.Dead();
            }
        } else {
            m_Owner.m_Skin.Move(m_flyState, Vector3.zero, ActionMode.Delta);
        }
        
        if (m_fCounter00300 > 0) {
            m_fCounter00300 -= Time.deltaTime;
        } else {
            SetFallDown();
            m_fCounter00300 = float.MaxValue;
        }
        if (m_bBeginKick) {
        
            if (m_fKickCounter > 0) {
                m_fKickCounter -= Time.deltaTime;
                m_flyState = AnimatorState.FlyFallStand01200;//蹦蹦被踢飞动作
                
                GameObject posgo =  m_Owner.m_Parent.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.petFollowPos);
                Vector3 pos = posgo.transform.position;
                Vector3 pos1;
                if (m_fKickCounter < 1.25f) { //分支 蹦蹦开始被踢的时间
                    m_fKickSlerpCounter += Time.deltaTime;
                    pos1 = Vector3.Slerp(m_Owner.m_Skin.tRoot.position, pos, m_fKickSlerpCounter / 1.25f);//分支
                } else {
                    pos1 = m_Owner.m_Skin.tRoot.position;
                }
                m_Owner.m_Skin.SetMirror(m_bKickDir == Vector3.left ? WalkDir.WALKLEFT : WalkDir.WALKRIGHT);
                m_Owner.m_Skin.Move(m_flyState, pos1, ActionMode.Set, false);
                m_bBeKicked = true;
            } else {
                //Debug.Log("KickFlyEnd");
                m_petState = Pet1002State.Follow;
                m_bBeginFall = false;
                m_bBeginKick = false;
                
                m_flyState = AnimatorState.Walk;
                m_Owner.SetMoveState(MoveState.Walk);
                //m_Owner.m_Core.m_MoveState = MoveState.Walk;
                Vector3 pos = Vector3.zero;
                //Vector3 pos = new Vector3(MapGrid.m_width *1* (m_bKickDir == Vector3.left ? -1 : 1), 0, 0);
                m_Owner.m_Skin.Move(m_flyState, pos, ActionMode.Delta);
                m_Owner.m_Skin.tRoot.localPosition = U3DUtil.SetZ(m_Owner.m_Skin.tRoot.localPosition, 0);
            }
        }
    }
    
    /// <summary>
    /// 蹦蹦掉落
    /// </summary>
    public void SetFallDown()
    {
        m_flyState = AnimatorState.FlyFall00400;
        m_bBeginFall = true;
    }
    /// <summary>
    /// 蹦蹦被踢飞
    /// </summary>
    public void KickBomb(WalkDir dir)
    {
        //NGUIUtil.DebugLog("蹦蹦被踢飞后方向="+dir,"red");
        m_fKickCounter = 2.533f;
        m_bBeginKick = true;
        m_bKickDir = (dir == WalkDir.WALKLEFT) ? Vector3.left : Vector3.right;
    }
    // Update is called once per frame
    public virtual void Update()
    {
    
    }
    
    public  override void ColliderProc(Collision collision)
    {
        if (m_bBeKicked) {
            return;
        }
        LifeProperty lp  = collision.gameObject.GetComponent<LifeProperty>();
        if (lp == null) {
            return;
        }
        IggFloor floor = collision.gameObject.GetComponent<LifeProperty>().GetLife() as IggFloor;
        if (floor != null) {
            m_fLiveCounter = 0;
            if (floor.isDead) {
                return;
            }
            
            if (m_Owner.m_Skill != null) {
                Life lifeM = m_Owner.GetCollisionGo(collision);
                FlyCollisionInfo Info = new FlyCollisionInfo();
                (m_Owner.m_Skill as FireSkill).FlyInfo = Info;
                Info = (m_Owner.m_Skill as FireSkill).FireTrigger(collision, lifeM, m_Owner, true, true, m_Owner.m_listCollisionGoID.Count, FlyDir.Bottom);
                if (!floor.isDead && lifeM != null) {
                    m_fFallSpeed = 0;
                    m_Owner.m_Skin.tRoot.parent = BattleEnvironmentM.GetLifeMBornNode(true);
                    m_Owner.m_Skin.tRoot.localPosition = U3DUtil.SetZ(m_Owner.m_Skin.tRoot.localPosition, 0.2f);
                    //设置蹦蹦的Y位置
                    m_Owner.m_Skin.tRoot.position = U3DUtil.SetY(m_Owner.m_Skin.tRoot.position, floor.GetMapGrid().WorldPos.y);
                    m_bHitFloor = true;
                    m_petState = Pet1002State.waitKick;
                    m_flyState = AnimatorState.FlyAttack01100;
                }
            }
        }
    }
}
