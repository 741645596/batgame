using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic;


/// <summary>
/// 发兵过程
/// </summary>
/// <author>zhulin</author>
public class SoldierFire
{

    private static bool m_IsSmooth = true ;
    private static int m_taskid;
    private static int m_step;
    private static bool m_IsTaskGuide  = false;
    
    /// <summary>
    /// 设置引导模式
    /// </summary>
    public static void SetTaskGruide(int taskid, int step)
    {
        m_IsTaskGuide = true;
        m_taskid = taskid;
        m_step = step;
    }
    
    
    
    /// <summary>
    /// 发送炮弹兵
    /// </summary>
    public static bool Fire(List<Vector3> lFlyPoint, int RoleID, SoldierInfo Soldier, bool IsPlayer)
    {
        if (Soldier == null || lFlyPoint == null || lFlyPoint.Count < 3) {
            return false;
        }
        
        Role SoldierRole = null;
        int sceneID = CM.DataID2SceneIDInSoldier(RoleID);
        Life life = CM.GetLifeM(sceneID, LifeMType.SOLDIER) ;
        
        if (life == null) {
            int RoleType = Soldier.m_modeltype;
            if (RoleType == 102003) {
                RoleType = 1020032;
            }
            if (RoleType == 200009) {
                RoleType = 2000092;
            }
            string SoldierName = Soldier.m_name;
            Transform FlyNode = BattleEnvironmentM.GetLifeMBornNode(false);
            FlyNode.position = new Vector3(FlyNode.position.x, FlyNode.position.y, -0.5f);
            //IGameRole i = GameRoleFactory.Create(FlyNode, RoleType, SoldierName, AnimatorState.Fly00000);
            SoldierRole = LifeFactory.CreateRole(FlyNode, AnimatorState.Fly00000, Soldier, LifeMCamp.ATTACK, RoleID, MoveState.Fly, Int2.zero, LifeEnvironment.Combat);
            GameObject go = SoldierRole.RoleSkinCom.tRoot.gameObject;
            RoleColider col = SoldierRole.RoleSkinCom.tBody.gameObject.GetComponent<RoleColider>();
            col.EnableColider(ColiderType.Fire, true);
            Vector3 dir = lFlyPoint[1] - lFlyPoint[0];
            if (dir.x <= 0) {
                SoldierRole.RoleSkinCom.SetMirror(1, 1, 1);
            } else {
                SoldierRole.RoleSkinCom.SetMirror(-1, 1, 1);
            }
            
        } else {
            life.SetActive(true);
            SoldierRole = life as Role;
            SoldierRole.m_bReBorn = false;
        }
        
        if (SoldierRole != null) {
            if (IsPlayer) {
                GodSkillWnd gsw = WndManager.FindDialog<GodSkillWnd>();
                if (gsw != null) {
                    gsw.ChangeBiaoqing((int)CaptionExpress.fire);
                }
            }
            
            if (SoldierRole.MoveAI != null && SoldierRole.MoveAI is Fly) {
                (SoldierRole.MoveAI as Fly).SetTrailFly(lFlyPoint);
                PlayFlyMusic(Soldier.m_modeltype);
            }
        }
        
        return true;
    }
    /// <summary>
    /// 播放炮弹兵发送的音乐
    /// </summary>
    private static void PlayFlyMusic(int roleID)
    {
        bool soundExit = SoundPlay.Play(roleID.ToString() + "_fly", false, false);
        if (!soundExit) {
            SoundPlay.Play("fly", false, false);
        }
    }
    
    /// <summary>
    /// 确认飞行轨迹有效。
    /// </summary>
    public  static bool CheckFlyLine(ref List<Vector3> lFlyPoint)
    {
        if (lFlyPoint == null || lFlyPoint.Count < 0) {
            return false;
        }
        //if( lFlyPoint.Count < 2) return false;
        //if (lFlyPoint.Count==2 && lFlyPoint[0] == lFlyPoint[1])
        //	return false;
        lFlyPoint = PathUtil.SmoothSimple(lFlyPoint);
        if (m_IsSmooth) {
            if (lFlyPoint == null) {
                return false;
            }
            if (GenerateShip.pointInRejectPolygon(lFlyPoint [0], GenerateShip.GetOutRejectPolygon())) {
                Vector3 posRaySrc = lFlyPoint [0];
                Vector3 posRayTo0 = lFlyPoint [0] + Vector3.left * 6f;
                Vector3 posReturn0 = new Vector3(posRayTo0.x, posRayTo0.y, posRayTo0.z);
                GenerateShip.RayToRejectPolygon(posRaySrc, posRayTo0, ref posReturn0, GenerateShip.GetOutRejectPolygon());
                
                Vector3 posRayTo1 = lFlyPoint [0] + Vector3.right * 6f;
                Vector3 posReturn1 = new Vector3(posRayTo1.x, posRayTo1.y, posRayTo1.z);
                GenerateShip.RayToRejectPolygon(posRaySrc, posRayTo1, ref posReturn1, GenerateShip.GetOutRejectPolygon());
                
                Vector3 posRayTo2 = lFlyPoint [0] + Vector3.up * 6f;
                Vector3 posReturn2 = new Vector3(posRayTo2.x, posRayTo2.y, posRayTo2.z);
                GenerateShip.RayToRejectPolygon(posRaySrc, posRayTo2, ref posReturn2, GenerateShip.GetOutRejectPolygon());
                
                float fDistance0 = Vector2.Distance(posRaySrc, posReturn0);
                float fDistance1 = Vector2.Distance(posRaySrc, posReturn1);
                float fDistance2 = Vector2.Distance(posRaySrc, posReturn2);
                lFlyPoint.Remove(posRaySrc);
                if (fDistance0 <= fDistance1)
                    if (fDistance2 < fDistance0) {
                        lFlyPoint.Insert(0, posReturn2);
                    } else {
                        lFlyPoint.Insert(0, posReturn0);
                    } else {
                    if (fDistance2 < fDistance1) {
                        lFlyPoint.Insert(0, posReturn2);
                    } else {
                        lFlyPoint.Insert(0, posReturn1);
                    }
                }
            }
            //把进入船的点去掉
            for (int i = 0; i < lFlyPoint.Count; i++) {
            
                if (GenerateShip.pointInRejectPolygon(lFlyPoint [i], GenerateShip.GetRejectPolygon()) && lFlyPoint.Count - i - 1 > 0) {
                    lFlyPoint.RemoveRange(i + 1, lFlyPoint.Count - i - 1);
                    break;
                }
            }
        }
        
        if (lFlyPoint.Count > 1) {
            if (GenerateShip.pointInRejectPolygon(lFlyPoint [0], GenerateShip.GetOutRejectPolygon())) {
                Vector3 posRaySrc = lFlyPoint [0];
                Vector3 posRayTo0 = lFlyPoint [0] + Vector3.left * 6f;
                Vector3 posReturn0 = new Vector3(posRayTo0.x, posRayTo0.y, posRayTo0.z);
                GenerateShip.RayToRejectPolygon(posRaySrc, posRayTo0, ref posReturn0, GenerateShip.GetOutRejectPolygon());
            }
            
            int nPoint = lFlyPoint.Count;
            Vector3 posLast = lFlyPoint[nPoint - 1] + (lFlyPoint[nPoint - 1] - lFlyPoint[nPoint - 2]) * 30f;
            lFlyPoint.Add(posLast);
            return true;
        }
        return false;
    }
}
