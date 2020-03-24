using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// life 创建工厂
/// </summary>
/// <author>zhulin</author>
public class LifeFactory
{

    /// <summary>
    /// 创建房间
    /// </summary>
    /// <param name="Info">建筑数据</param>
    /// <param name="DataID">战斗环境中的DataID，其他环境填0 就可</param>
    /// <param name="Parent">挂载点</param>
    /// <param name="Parent">环境</param>
    /// <returns></returns>
    public static Building CreateBuilding(BuildInfo Info, int DataID, Transform Parent, Vector3 WorldPos, LifeEnvironment Environment)
    {
        Life.Environment = Environment ;
        if (Info == null || Parent == null) {
            return null;
        }
        LifeProperty Property = CreateBuildSkin(Info, Parent, WorldPos);
        if (Property == null) {
            return null;
        }
        Building buildlife = ConnectBuildingLife(Property, Info, DataID, Environment);
        LifeObj building = Property.transform.parent.gameObject.AddComponent<LifeObj>();
        if (Property != null && building != null) {
            building.SetLife(buildlife as Life, Property);
        }
        
        return buildlife;
    }
    
    /// <summary>
    /// 创建房间对象
    /// </summary>
    /// <param name="Info">建筑数据</param>
    /// <param name="Parent">挂载点</param>
    /// <returns>LifeProperty</returns>
    private static LifeProperty CreateBuildSkin(BuildInfo Info, Transform Parent, Vector3 WorldPos)
    {
        if (Info == null || Parent == null) {
            return null;
        }
        
        string name = Info.m_modeltype.ToString() + "@Skin";
        GameObject m_objRoleRoot = new GameObject();
        m_objRoleRoot.transform.parent = Parent;
        m_objRoleRoot.transform.localPosition =  new Vector3(0, 0, 0);
        m_objRoleRoot.name = Info.m_name;
        m_objRoleRoot.transform.position = WorldPos;
        m_objRoleRoot.transform.localScale = Vector3.one;
        GameObject build = GameObjectLoader.LoadPath("Prefabs/Buildings/", name, m_objRoleRoot.transform);
        
        if (build == null) {
            GameObject.Destroy(m_objRoleRoot);
            Debug.Log("buildname: " + name + ",类型为" + Info.BuildType + "建筑资源不存在");
            return null;
        }
        LifeProperty property = build.GetComponent<LifeProperty>();
        return property ;
    }
    
    /// <summary>
    /// 加入buildlife 与lifeobj的关联
    /// </summary>
    /// <param name="BuildSkin">建筑物对象表现实体</param>
    /// <param name="Info">建筑物信息</param>
    /// <param name="DataID">战斗环境中的DataID，其他环境填0 就可</param>
    /// <param name="Environment">对象所出环境</param>
    private static Building ConnectBuildingLife(LifeProperty SkinProperty, BuildInfo Info, int DataID, LifeEnvironment Environment)
    {
        if (SkinProperty == null) {
            return null;
        }
        Building buildlife = GetBuildingLife(Info, SkinProperty, Environment);
        if (SkinProperty != null) {
            if (Environment == LifeEnvironment.Combat) {
                bool IsPlayer = CmCarbon.GetCamp2Player(LifeMCamp.DEFENSE);
                buildlife.SetLifeCore(new LifeMCore(DataID, IsPlayer, LifeMType.BUILD, LifeMCamp.DEFENSE, MoveState.Static));
            }
        }
        return buildlife;
    }
    
    private static Building GetBuildingLife(BuildInfo Info, LifeProperty SkinProperty, LifeEnvironment Environment)
    {
        if (Info == null || SkinProperty == null) {
            return null;
        }
        Building life = null ;
        switch (Info.BuildType) {
            case 1201:
                life = new Building1201();
                break;
            case 1300:
                life = new Building1300();
                break;
            case 1601:
                life = new Building1601();
                break;
            case 1602:
                life = new Building1602();
                break;
            case 1603:
                life = new Building1603();
                break;
            case 1604:
                life = new Building1604();
                break;
            case 1605:
                life = new Building1605();
                break;
            case 1606:
                life = new Building1606();
                break;
            case 1607:
                life = new Building1607();
                break;
            case 1608:
                life = new Building1608();
                break;
            case 1609:
                life = new Building1609();
                break;
            case 1610:
                life = new Building1610();
                break;
            case 1611:
                life = new Building1611();
                break;
            case 1612:
                life = new Building1612();
                break;
            case 1613:
                life = new Building1613();
                break;
            case 1619:
                life = new Building1619();
                break;
            case 9999:
                life = new Building9999();
                break;
            case 9998:
                life = new Building9998();
                break;
            case 9997:
                life = new Building9997();
                break;
            default:
                break;
                
        }
        if (life != null) {
            life.SetBuildLife(Info, SkinProperty, Environment);
            life.InitBuildModel();
        } else {
            Debug.Log("create building fail:" + Info.BuildType);
        }
        return life;
    }
    /// <summary>
    /// 创建房间
    /// </summary>
    /// <param name="Info">炮弹兵信息</param>
    /// <param name="Camp">阵营</param>
    /// <param name="DataID">战斗环境中的DataID，其他环境填0 就可</param>
    /// <param name="moveState">运动状态</param>
    /// <param name="iRole">iRole</param>
    /// <returns></returns>
    public static Role CreateRole(Transform Tstart, AnimatorState state, SoldierInfo Info, LifeMCamp Camp, int DataID, MoveState moveState, Int2 BornPos, LifeEnvironment Environment)
    {
        if (Info == null) {
            return null;
        }
        
        return ConnectRoleLife(Tstart, state, Info, Camp, DataID, moveState, BornPos, Environment);
    }
    
    /// <summary>
    /// Role 与lifeobj的关联
    /// </summary>
    /// <param name="t">角色对象的Transform</param>
    /// <param name="Info">角色信息</param>
    /// <param name="Camp">阵营</param>
    /// <param name="DataID">战斗环境中的DataID，其他环境填0 就可</param>
    /// <param name="moveState">运动状态</param>
    /// <param name="Environment">对象所出环境</param>
    public static Role ConnectRoleLife(Transform Tstart, AnimatorState state, SoldierInfo Info, LifeMCamp Camp, int DataID, MoveState moveState, Int2 BornPos, LifeEnvironment Environment)
    {
        Role RoleLife = new Role();
        bool IsPlayer = CmCarbon.GetCamp2Player(Camp);
        int RoleType = Info.m_modeltype;
        if (RoleType == 102003) {
            RoleType = 1020032;
        }
        if (RoleType == 200009) {
            RoleType = 2000092;
        }
        RoleLife.CreateSkin(Tstart, RoleType, Info.m_name, state, IsPlayer);
        RoleLife.SetRoleLife(Info, RoleLife.RoleSkinCom.ProPerty, Environment);
        LifeObj building = RoleLife.RoleSkinCom.tRoot.gameObject.AddComponent<LifeObj>();
        if (building != null) {
            if (Environment == LifeEnvironment.Combat) {
                RoleLife.SetLifeCore(new LifeMCore(DataID, IsPlayer, LifeMType.SOLDIER, Camp, moveState));
                RoleLife.SetSkin();
            }
            if (moveState == MoveState.Walk) {
                RoleLife.MoveAI.SetBornPos(BornPos, 0);
            }
            building.SetLife(RoleLife as Life, RoleLife.RoleSkinCom.ProPerty);
            
        }
        return RoleLife;
    }
    public static Role CreateRole(Transform Tstart, SoldierInfo s, bool isPlayer, Vector3 wordpos, AnimatorState state, bool isplayer)
    {
    
        //Transform t	= BattleEnvironmentM.GetLifeMBornNode(true);
        //IGameRole iRole= GameRoleFactory.Create(t, s.m_modeltype, s.m_name, state);
        //iRole.GetRolePropertyM().SetCampModel(isPlayer,LifeMCamp.ATTACK);
        Role RoleLife = new Role();
        RoleLife.CreateSkin(Tstart, s.m_modeltype, s.m_name, state, isplayer);
        RoleLife.RoleSkinCom.tRoot.parent = Tstart;
        RoleLife.RoleSkinCom.tRoot.position = wordpos;
        
        RoleLife.SetRoleLife(s, RoleLife.RoleSkinCom.GetRolePropertyM(), LifeEnvironment.Combat);
        LifeObj building = RoleLife.RoleSkinCom.tRoot.gameObject.AddComponent<LifeObj>();
        building.SetLife(RoleLife as Life, RoleLife.RoleSkinCom.ProPerty);
        return RoleLife;
    }
}
