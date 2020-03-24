using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using sdata ;
using battle ;
public enum ResourceType{
	Gold = 1,
	Wood =2,
	Stone = 3,
	Steel = 4,
	Box = 5,
    Diamond = 6,
    Physics = 7,//体力
}
/// 0,满足条件，1等级不足。2材料不足,3最高级.
public enum CanQualityResult
{
	CanUp = 0,//0,满足条件
	LevelLimit = 1,//1等级不足
	MaterialLimit = 2,//2材料不足
	QualityMax = 3,//3最高级
}

//能否升级.0能升级.1金币不足.2其他材料不足.3满级.
public enum CanStarResult
{
	CanUp = 0,
	LessCoin = 1,
	LessMater = 2,
	StarMax = 3,
}

//能否升级.0能升级.1金币不足.2木材材料不足,3等级限制，4等级最高.
public enum CanLevelResult
{
	CanUp = 0,
	LessCoin = 1,
	LessMater = 2,
	LevelLimit = 3,
	LevelMax = 4,
}


public class buildingM {
    private static List<s_building_typeInfo> m_lType = new List<s_building_typeInfo>();
    private static List<s_buildresourceInfo> m_lResource = new List<s_buildresourceInfo>();
	private static List<s_shapetypeInfo> m_lShapeType = new List<s_shapetypeInfo>();
	private static List<s_buildqualityInfo> m_lBuildQuality = new List<s_buildqualityInfo>();
	private static List<s_buildstarInfo> m_lBuildStar = new List<s_buildstarInfo>();
	private static List<s_buildupInfo> m_lBuildUp = new List<s_buildupInfo>();
	private static List<s_stagebuildInfo> m_lStageBuild = new List<s_stagebuildInfo>();
	private static List<s_buildanalyzeInfo> m_lBuildannalyze = new List<s_buildanalyzeInfo>();

    

	public static void Init (object obj)
	{
        System.Diagnostics.Debug.Assert(obj is sdata.StaticDataResponse);

        sdata.StaticDataResponse sdrsp = obj as sdata.StaticDataResponse;

        m_lType = sdrsp.s_building_type_info;
        m_lResource = sdrsp.s_buildresource_info;
		m_lShapeType = sdrsp.s_shapetype_info ;
		m_lBuildQuality = sdrsp.s_buildquality_info ;
		m_lBuildStar = sdrsp.s_buildstar_info ;
		m_lBuildUp = sdrsp.s_buildup_info;
		m_lStageBuild = sdrsp.s_stagebuild_info;
		m_lBuildannalyze = sdrsp.s_buildanalyze_info;
	}
	/// <summary>
	/// 获取建筑数据
	/// </summary>
	/// <param name="item">战役副本摆设</param>
	/// <returns>建筑数据,null 获取失败</returns>
	public static BuildInfo  GetBuildInfo( build.BuildInfo Info)
	{
		if (Info == null ) return null;
		
		BuildInfo b = GetBuildInfo(Info.buildtype ,Info.level,Info.quality,Info.starlevel);
		if(b != null)
		{
			b.ID = Info.id;
			b.m_cx = 0;
			b.m_cy = 0;
			b.m_ShipPutdata0 = 0;
			b.m_ShipPutdata1 = 0;
		}
		return b;
	}

	/// <summary>
	/// 获取建筑数据
	/// </summary>
	/// <param name="item">战役副本摆设</param>
	/// <returns>建筑数据,null 获取失败</returns>
	public static void  UpdateBuildInfo( build.BuildInfo Info ,ref BuildInfo buildInfo)
	{
		if (Info == null  || buildInfo == null) return ;
		buildInfo.BuildType =  Info.buildtype ;
		buildInfo.Level =  Info.level ;
		buildInfo.Quality =  Info.quality ;
		buildInfo.StarLevel =  Info.starlevel ;
		UpdateBuildInfo(ref buildInfo);
	}




	/// <summary>
	/// 获取建筑数据
	/// </summary>
	/// <param name="item">战役副本摆设</param>
	/// <returns>建筑数据,null 获取失败</returns>
	public static BuildInfo  GetBuildInfo(s_countershipputInfo item )
	{
		if (item == null ) return null;
		int buildType = item.objid ;
		
		BuildInfo Info = GetBuildInfo(buildType ,item.level ,item.quality,item.starlevel);
		if(Info != null)
		{
			Info.ID = item.objid * 1000 + item.level;
			Info.m_cx = item.cx;
			Info.m_cy = item.cy;
			Info.m_ShipPutdata0 = item.data0;
			Info.m_ShipPutdata1 = item.data1;
			return Info;
		}
		else return null ;
	}


	/// <summary>
	/// 获取PVE建筑数据
	/// </summary>
	/// <param name="item">战役副本摆设</param>
	/// <returns>建筑数据,null 获取失败</returns>
	public static BuildInfo GetStageBuildInfo(s_countershipputInfo item)
	{
		if (item == null) return null;
		int id = item.objid;

		BuildInfo Info = GetStageBuildInfo(id, item.level, item.quality, item.starlevel);
		if (Info != null)
		{
			Info.ID = item.objid;
			Info.m_cx = item.cx;
			Info.m_cy = item.cy;
			Info.m_ShipPutdata0 = item.data0;
			Info.m_ShipPutdata1 = item.data1;
			return Info;
		}
		else return null;
	}

	/// <summary>
	/// 获取建筑数据
	/// </summary>
	/// <param name="Type">建筑类型</param>
	/// <param name="Level">建筑等级</param>
	/// <param name="Quality">建筑品质</param>
	/// <param name="Star">建筑星级</param>
	/// <returns>建筑数据,null 获取失败</returns>
	private static BuildInfo GetStageBuildInfo(int id, int Level, int Quality, int Star)
	{
		//获取基础建筑信息
		s_stagebuildInfo BaseInfo = GetStageBuildInfo(id);
		if (BaseInfo == null)
		{
			NGUIUtil.DebugLog("获取不到该类型的建筑数据：" + id);
			return null;
		}
		BuildInfo Info = new BuildInfo();
		Info.ID = id;
		Info.BuildType = BaseInfo.type;
		Info.Level = BaseInfo.level;
		Info.Quality = BaseInfo.quality;
		Info.StarLevel = BaseInfo.starlevel;

		Info.m_name = BaseInfo.name;
		Info.m_modeltype = BaseInfo.modeltype;
		Info.m_damage = BaseInfo.damage;
		Info.m_RoomType = (RoomType)BaseInfo.resource;
		Info.m_Desc = BaseInfo.description;
		Info.m_data0 = BaseInfo.data0;
		Info.m_Shape = GetShapeType(BaseInfo.shapeid);
		Info.m_bear = BaseInfo.bear;
		Info.m_wood = BaseInfo.dropwood;
		Info.Mana = BaseInfo.dead_mana;


		//计算升级，升星，升阶 数据。
		CalcStageBuildInfo(BaseInfo, ref Info);
		//返回结果
		return Info;
	}

	public static int GetStageBuildMana(int id)
	{
		s_stagebuildInfo buildInfo = GetStageBuildInfo(id);
		if(buildInfo != null)
		{
			return buildInfo.dead_mana;
		}
		return 0;
	}

	/// <summary>
	/// 获取建筑数据
	/// </summary>
	/// <param name="item">战役副本摆设</param>
	/// <returns>建筑数据,null 获取失败</returns>
	public static BuildInfo  GetBuildInfo(ShipBuildInfo item )
	{
		if (item == null ) return null;
		
		BuildInfo Info = GetBuildInfo(item.buildtype ,item.level ,item.quality,item.starlevel);
		if(Info != null)
		{
			Info.ID = item.objid;
			Info.m_cx = item.cx;
			Info.m_cy = item.cy;
			Info.m_ShipPutdata0 = item.shipput_data0;
			Info.m_ShipPutdata1 = item.shipput_data1;
			return Info;
		}
		else return null ;
	}

	public static List<BuildInfo> GetAllBuildInfo()
	{
		List<BuildInfo> l = new List<BuildInfo>();
		foreach(s_building_typeInfo info in m_lType)
		{
			BuildInfo buildInf = new BuildInfo();
			buildInf.BuildType =  info.id ;
			buildInf.Level =  1 ;
			buildInf.Quality = 10 ;
			buildInf.StarLevel =  info.starlevel ;
			UpdateBuildInfo(ref buildInf);
			l.Add(buildInf);
		}
		return l;
	}
	/// <summary>
	/// 获取建筑数据
	/// </summary>
	/// <param name="item">战役副本摆设</param>
	/// <returns>建筑数据,null 获取失败</returns>
	public static BuildInfo  GetBuildInfo( int buildID ,s_itemtypeInfo item )
	{
		if (item == null || item.gtype != 2) return null;
		BuildInfo Info = GetStartBuildInfo(item.gid);
		if(Info != null)
		{
			Info.ID = buildID;
			Info.m_cx = 0;
			Info.m_cy = 0;
			Info.m_ShipPutdata0 = 0;
			Info.m_ShipPutdata1 = 0;
		}
		return Info;
	}

	/// <summary>
	/// 获取建筑数据
	/// </summary>
	/// <param name="Type">建筑类型</param>
	/// <param name="Level">建筑等级</param>
	/// <param name="Quality">建筑品质</param>
	/// <param name="Star">建筑星级</param>
	/// <returns>建筑数据,null 获取失败</returns>
	public static void UpdateBuildInfo(ref BuildInfo Info)
	{
		if(Info == null) 
			return ;
		//获取基础建筑信息
		s_building_typeInfo BaseInfo = GetBuildType(Info.BuildType);
		if (BaseInfo == null)
		{
			NGUIUtil.DebugLog("获取不到该类型的建筑数据：" + Info.BuildType);
			return ;
		}
		FillBaseBuildInfo(BaseInfo ,ref Info);
		//获取建筑掉落资源
		s_buildresourceInfo sreinfo = GetBuildSource(Info.BuildType ,Info.Level);
		if(sreinfo != null)
		{
			FillSourceDrop(sreinfo ,ref Info);
		}
		//计算升级，升星，升阶 数据。
		CalcBuildInfo(BaseInfo ,ref Info) ;
	}
	/// <summary>
	/// 获取建筑数据
	/// </summary>
	/// <param name="Type">建筑类型</param>
	/// <param name="Level">建筑等级</param>
	/// <param name="Quality">建筑品质</param>
	/// <param name="Star">建筑星级</param>
	/// <returns>建筑数据,null 获取失败</returns>
	private static BuildInfo GetBuildInfo(int Type,int Level ,int Quality ,int Star )
	{
		//获取基础建筑信息
		s_building_typeInfo BaseInfo = GetBuildType(Type);
		if (BaseInfo == null)
		{
			NGUIUtil.DebugLog("获取不到该类型的建筑数据：" + Type);
			return null;
		}
		BuildInfo Info = new BuildInfo ();
		Info.BuildType = Type;
		Info.Level = Level;
		Info.Quality = Quality;
		Info.StarLevel = Star;
		FillBaseBuildInfo(BaseInfo ,ref Info);
		//获取建筑掉落资源
		s_buildresourceInfo sreinfo = GetBuildSource(Type ,Level);
		if(sreinfo != null)
		{
			FillSourceDrop(sreinfo ,ref Info);
		}
		//计算升级，升星，升阶 数据。
		CalcBuildInfo(BaseInfo ,ref Info) ;
		//返回结果
		return Info ;
	}
	/// <summary>
	/// 获取初级建筑数据
	/// </summary>
	/// <param name="Type">建筑类型</param>
	/// <param name="Level">建筑等级</param>
	/// <param name="Quality">建筑品质</param>
	/// <param name="Star">建筑星级</param>
	/// <returns>建筑数据,null 获取失败</returns>
	public static BuildInfo GetStartBuildInfo(int Type )
	{
		//获取基础建筑信息
		s_building_typeInfo BaseInfo = GetBuildType(Type);
		if (BaseInfo == null)
		{
			NGUIUtil.DebugLog("获取不到该类型的建筑数据：" + Type);
			return null;
		}

		return GetBuildInfo(Type, 1,10 ,BaseInfo.starlevel) ;
	}


	/// <summary>
	/// 填充基础建筑信息
	/// </summary>
	private static void FillBaseBuildInfo(s_building_typeInfo I ,ref BuildInfo Info)
	{
		if(I == null || Info == null) return;
		Info.m_name = I.name;
		Info.m_modeltype = I.modeltype;
		Info.m_damage = I.damage;
		Info.m_RoomType = (RoomType)I.resource;
		Info.m_RoomKind = I.kindtype ;
		Info.m_Desc = I.description ;
		Info.m_data0 = I.data0;
		Info.m_data1 = I.data1;
		Info.m_Shape = GetShapeType(I.shapeid);
		Info.m_bear = I.bear;
		Info.fragmentTypeID = I.fragment;
	}

	/// <summary>
	/// 填充基础建筑信息
	/// </summary>
	private static void FillStageBuildInfo(s_stagebuildInfo I, ref BuildInfo Info)
	{
		if (I == null || Info == null) return;
		Info.m_name = I.name;
		Info.m_modeltype = I.modeltype;
		Info.m_damage = I.damage;
		Info.m_RoomType = (RoomType)I.resource;
		//Info.m_RoomKind = I.kindtype;
		Info.m_Desc = I.description;
		Info.m_data0 = I.data0;
		//Info.m_data1 = I.data1;
		Info.m_Shape = GetShapeType(I.shapeid);
		Info.m_bear = I.bear;
	}

	/// <summary>
	/// 填充基础建筑掉落资源
	/// </summary>
	private static void FillSourceDrop(s_buildresourceInfo sreinfo,ref BuildInfo Info)
	{
		if(sreinfo == null || Info == null)
			return ;
		Info.m_wood = sreinfo.wood;
		Info.m_stone = sreinfo.stone;
		Info.m_steel = sreinfo.steel;
	}
	/// <summary>
	/// 填充基础建筑信息
	/// </summary>
	private static void CalcBuildInfo(s_building_typeInfo I ,ref BuildInfo Info)
	{
		if(I == null || Info == null)
			return ;
		//获取技能
		Info.m_Skill =  GetSkill(Info.BuildType,Info.Quality);
		s_buildstarInfo StarInfo = GetBuildStar(Info.BuildType,Info.StarLevel);
		s_buildqualityInfo QualityInfo = GetBuildQuality(Info.BuildType ,Info.Quality);

		int originalStar = buildingM.GetMinBuildStar (Info.BuildType);
		s_buildstarInfo oriInfo = buildingM.GetBuildStar (Info.BuildType,originalStar);

		//建筑物三围
		Info.m_Solidity = CalcSolidity(I,StarInfo,oriInfo,QualityInfo ,Info.Level);   
		Info.m_Intensity = CalcIntensity(I,StarInfo,oriInfo,QualityInfo ,Info.Level);   
		Info.m_Tenacity = CalcTenacity(I,StarInfo,oriInfo,QualityInfo ,Info.Level); 
		Info.m_hp = CalcHp(I,QualityInfo ,Info.m_Solidity);
		Info.m_phyattack = CalcPhyAttack(I,QualityInfo,Info.m_Intensity);
		Info.m_magicattack = CalcMagAttack(I,QualityInfo,Info.m_Intensity);
		Info.m_phydefend = CalcPhyDefend(I,QualityInfo,Info.m_Tenacity);
		Info.m_magicdefend = CalcMagDefend(I,QualityInfo,Info.m_Tenacity);
		Info.m_DefensePower = CalcDefendPower(Info.Quality,Info.StarLevel ,Info.Level);
		Info.m_bear = CalcBear(I,StarInfo,QualityInfo);
	}
	/// <summary>
	/// 填充基础建筑信息
	/// </summary>
	private static void CalcStageBuildInfo(s_stagebuildInfo I, ref BuildInfo Info)
	{
		if (I == null || Info == null)
			return;
		//获取技能
		Info.m_Skill = GetSkill(Info.BuildType, Info.Quality);
		s_buildstarInfo StarInfo = GetBuildStar(Info.BuildType, Info.StarLevel);
		s_buildqualityInfo QualityInfo = GetBuildQuality(Info.BuildType, Info.Quality);

		int originalStar = buildingM.GetMinBuildStar(Info.BuildType);
		s_buildstarInfo oriInfo = buildingM.GetBuildStar(Info.BuildType, originalStar);

		//建筑物三围
		//Info.m_Solidity = CalcSolidity(I, StarInfo, oriInfo, QualityInfo, Info.Level);
		//Info.m_Intensity = CalcIntensity(I, StarInfo, oriInfo, QualityInfo, Info.Level);
		//Info.m_Tenacity = CalcTenacity(I, StarInfo, oriInfo, QualityInfo, Info.Level);
		Info.m_hp = I.hp;
		Info.m_phyattack = I.phy_attack;
		Info.m_magicattack = I.magic_attack;
		Info.m_phydefend = I.physicaldefend;
		Info.m_magicdefend = I.magicdefend;
		Info.m_DefensePower = CalcDefendPower(Info.Quality, Info.StarLevel, Info.Level);
		Info.m_bear = I.bear;
	}
	/// <summary>
	/// 获取建筑类型基础数据
	/// </summary>
	public static s_building_typeInfo GetBuildType(int id)
	{
		foreach (s_building_typeInfo v in m_lType)
		{
			if (v.id == id ) 
				return v;
		}
		return null;
	}

	/// <summary>
	/// 获取建筑类型基础数据
	/// </summary>
	public static s_stagebuildInfo GetStageBuildInfo(int id)
	{
		foreach (s_stagebuildInfo info in m_lStageBuild)
		{
			if (info.id == id)
				return info;
		}
		return null;
	}

	/// <summary>
	/// 获取建筑类型基础数据
	/// </summary>
	/// <returns>获取建筑类型基础数据</returns>
	/// <param name="type">建筑类型</param>
	/// <param name="level">建筑等级</param>
	public static ShapeType GetShapeType(int Shapeid)
	{
		ShapeType Shape = new ShapeType();
		foreach (sdata.s_shapetypeInfo v in m_lShapeType)
		{
			if (v.id == Shapeid  ) 
			{
				Shape.id = v.id;
				Shape.width = v.width;
				Shape.height = v.height;
				Shape.shape = v.shape;
				Shape.map = v.map;
				return Shape;
			}
		}
		Debug.Log("没有对应的形状");
		return Shape;
	}
	
	/// <summary>
	/// 获取掉落资源
	/// </summary>
	/// <param name="buildtype">建筑类型</param>
	/// <param name="buildlevel">等级</param>
	/// <returns>建筑掉落资源</returns>
	private static s_buildresourceInfo GetBuildSource(int buildtype,int buildlevel)
	{
		int id = buildtype * 1000 + buildlevel ;
        foreach (s_buildresourceInfo I in m_lResource) 
		{
			if(I.id == id)
			{
				return I;
			}
		}
		return null;
	}

	/// <summary>
	/// 获取星级增长
	/// </summary>
	/// <param name="buildtype">建筑类型</param>
	/// <param name="StarLevel">星级</param>
	/// <returns>获取星级升级信息</returns>
	public static s_buildstarInfo GetBuildStar(int buildtype,int StarLevel)
	{
		foreach (s_buildstarInfo I in m_lBuildStar) 
		{
			if(I.buildtype == buildtype && I.starlevel == StarLevel)
			{
				return I;
			}
		}
		return null;
	}
	/// <summary>
	/// 获取建筑最小的星级.
	/// </summary>
	/// <returns>The minimum build star.</returns>
	public static int GetMinBuildStar(int buildtype)
	{
		int star = 5;
		foreach (s_buildstarInfo I in m_lBuildStar) 
		{
			if(I.buildtype == buildtype)
			{
				if(star >= I.starlevel)
				{
					star = I.starlevel;
				}
			}
		}
		return star;
	}
	/// <summary>
	/// 获取建筑星级成长
	/// </summary>
	/// <param name="Info">建筑数据</param>
	/// <param name="SolidityGrow">硬度成长</param>
	/// <param name="IntensityGrow">强度成长</param>
	/// <param name="TenacityGrow">韧性成长</param>
	public static void GetStarInfoGrow(BuildInfo Info ,ref float SolidityGrow,ref float  IntensityGrow ,ref float TenacityGrow)
	{
		SolidityGrow = 0;
		IntensityGrow = 0;
		TenacityGrow = 0 ;
		if(Info == null) return ;
		s_buildstarInfo  I = GetBuildStar(Info.BuildType,Info.StarLevel);
		if(I != null)
		{
			SolidityGrow = I.soliditygrow * 0.01f;
			IntensityGrow = I.intensitygrow * 0.01f;
			TenacityGrow = I.tenacitygrow * 0.01f;
		}
	}

	/// <summary>
	/// 获取升阶增长
	/// </summary>
	/// <param name="buildtype">建筑类型</param>
	/// <param name="Quality">阶数</param>
	/// <returns>获取星级升级信息</returns>
	private static s_buildqualityInfo GetBuildQuality(int buildtype,int Quality)
	{
		foreach (s_buildqualityInfo I in m_lBuildQuality) 
		{
			if(I.buildtype == buildtype && I.quality == Quality)
			{
				return I;
			}
		}
		return null;
	}


	/// <summary>
	/// 获取升级信息
	/// </summary>
	/// <param name="buildtype">建筑类型</param>
	/// <param name="StarLevel">星级</param>
	/// <returns>获取星级升级信息</returns>
	private static s_buildupInfo GetBuildUp(int buildtype,int Level)
	{
		foreach (s_buildupInfo I in m_lBuildUp) 
		{
			if(I.buildtype == buildtype && I.level == Level)
			{
				return I;
			}
		}
		return null;
	}

	/// <summary>
	/// 填充建筑技能
	/// </summary>
	private static BuildSkillInfo GetSkill(int BuildType  ,int Quality )
	{
		//获取技能
		return SkillM.GetBuildSkill(BuildType ,Quality);
	}
	/// <summary>
	/// 计算承受力
	/// </summary>
	private static int CalcBear(s_building_typeInfo TypeInfo ,s_buildstarInfo StarInfo,s_buildqualityInfo QualityInfo)
	{
		if(TypeInfo == null )
			return 0;
		
		int value = TypeInfo.bear ;
		
		if(StarInfo != null)
		{
			value += StarInfo.bear ;
		}
		
		if(QualityInfo != null)
		{
			value += QualityInfo.bear;
		}
		return value ;
	}

	/// <summary>
	/// 计算硬度
	/// </summary>
	private static float CalcSolidity(s_building_typeInfo TypeInfo ,s_buildstarInfo StarInfo ,s_buildstarInfo original,s_buildqualityInfo QualityInfo,int level)
	{
		if(TypeInfo == null )
			return 0.0f;

		float value = TypeInfo.solidity * 0.01f  ;

		if(StarInfo != null)
		{
			value += StarInfo.soliditygrow * (level -1) * 0.01f ;
		}
		if(original != null)
		{
			value += (StarInfo.soliditygrow - original.soliditygrow) * 0.01f;
		}

		if(QualityInfo != null)
		{
			value += QualityInfo.addsolidity * 0.01f;
		}
		return value ;
	}


	/// <summary>
	/// 计算强度
	/// </summary>
	private static float CalcIntensity(s_building_typeInfo TypeInfo ,s_buildstarInfo StarInfo ,s_buildstarInfo original,s_buildqualityInfo QualityInfo,int level)
	{
		if(TypeInfo == null )
			return 0;
		float value = TypeInfo.intensity * 0.01f;

		if(StarInfo != null)
		{
			value += StarInfo.intensitygrow * (level -1) * 0.01f ;
		}
			
		if(original != null)
		{
			value += (StarInfo.intensitygrow - original.intensitygrow) * 0.01f;
		}


		if(QualityInfo != null)
		{
			value += QualityInfo.addintensity * 0.01f;
		}
		return value ;
	}

	/// <summary>
	/// 计算韧性
	/// </summary>
	private static float CalcTenacity(s_building_typeInfo TypeInfo ,s_buildstarInfo StarInfo ,s_buildstarInfo original,s_buildqualityInfo QualityInfo,int level)
	{
		if(TypeInfo == null  )
			return 0;
		float value = TypeInfo.tenacity  * 0.01f;

		if(StarInfo != null)
		{
			value += StarInfo.tenacitygrow * (level -1) * 0.01f ;
		}

		if(original != null)
		{
			value += (StarInfo.tenacitygrow - original.tenacitygrow) * 0.01f;
		}

		if(QualityInfo != null)
		{
			value += QualityInfo.addtenacity * 0.01f;
		}
		return value ;
	}


	/// <summary>
	/// 计算血量上限
	/// </summary>
	private static int CalcHp(s_building_typeInfo TypeInfo ,s_buildqualityInfo QualityInfo ,float Solidity)
	{
		if(TypeInfo == null )
			return 0;
		float k1 = ConfigM.GetBuildUpParam(1) ; 
		float add = (Solidity - TypeInfo.solidity *0.01f) * k1 ;
		int value =  TypeInfo.hp  + (int)add ;

		if(QualityInfo != null)
		{
			value += QualityInfo.addhp;
		}
		return value ;
	
	}

	/// <summary>
	/// 计算物理攻击
	/// </summary>
	private static int CalcPhyAttack(s_building_typeInfo TypeInfo ,s_buildqualityInfo QualityInfo ,float Intensity)
	{
		if(TypeInfo == null )
			return 0;
		float k2 = ConfigM.GetBuildUpParam(2) ;
		float add =  (Intensity -  TypeInfo.intensity *0.01f) * k2 ;
		int value =  TypeInfo.phy_attack  + (int)add ;

		if(QualityInfo != null)
		{
			value += QualityInfo.addphyattack;
		}
		return value ;
		
	}

	/// <summary>
	/// 计算魔法攻击
	/// </summary>
	private static int CalcMagAttack(s_building_typeInfo TypeInfo ,s_buildqualityInfo QualityInfo ,float Intensity)
	{
		if(TypeInfo == null )
			return 0;
		float k3 = ConfigM.GetBuildUpParam(3) ; 
		float add =  (Intensity - TypeInfo.intensity *0.01f) * k3 ;
		int value = TypeInfo.magic_attack  + (int)add ;

		if(QualityInfo != null)
		{
			value += QualityInfo.addmagicattack;
		}
		return value ;
		
	}


	/// <summary>
	/// 计算物理防御
	/// </summary>
	private static int CalcPhyDefend(s_building_typeInfo TypeInfo ,s_buildqualityInfo QualityInfo ,float Tenacity)
	{
		if(TypeInfo == null )
			return 0;
		float k4 = ConfigM.GetBuildUpParam(4) ; 
		float add =  (Tenacity - TypeInfo.tenacity *0.01f) * k4 ;
		int value = TypeInfo.physicaldefend  + (int)add ;

		if(QualityInfo != null)
		{
			value += QualityInfo.addphydefend;
		}
		return value ;
		
	}

	/// <summary>
	/// 计算魔法防御
	/// </summary>
	private static int CalcMagDefend(s_building_typeInfo TypeInfo ,s_buildqualityInfo QualityInfo ,float Tenacity)
	{
		if(TypeInfo == null )
			return 0;
		float k5 = ConfigM.GetBuildUpParam(5) ; 
		float add =  (Tenacity - TypeInfo.tenacity *0.01f) * k5 ;
		int value = TypeInfo.magicdefend  + (int)add ;

		if(QualityInfo != null)
		{
			value += QualityInfo.addmagicdefend;
		}
		return value ;
		
	}


	/// <summary>
	/// 计算魔法防御
	/// </summary>
	private static int CalcDefendPower(int Quality,int StarLevel,int  level)
	{
		int k1 = 0;
		int k2 = 0; 
		int k3 = 0;
		ConfigM.GetTrapDefensePower(ref k1,ref k2,ref k3);
		return (k1 + ConfigM.GetQualityIndex (Quality) * k2   + StarLevel * k3 ) * level; 
	}

	/// <summary>
	/// 陷阱能否升阶.
	/// </summary>
	/// <returns>The quality can U.</returns>
	/// <param name="info">Info.</param>
	public static CanQualityResult GetQualityCanUP(BuildInfo info)
	{
		int NextQuality = ConfigM.GetNextQuality (info.Quality);

		if(NextQuality == info.Quality) 
			return CanQualityResult.QualityMax;

		int LimitLev = 0;

		Dictionary<int ,int> lNeedMaterial = new Dictionary<int,int>();
		Dictionary<int ,int> lhaveFragment = new Dictionary<int,int> ();

		buildingM.GetUpQualityNeed (info.BuildType,info.Quality,ref lNeedMaterial,ref LimitLev);
		if(info.Level < LimitLev)
		{
			return CanQualityResult.LevelLimit;
		}
		
		if(lNeedMaterial.Count == 0 && info.Quality == NextQuality ) return CanQualityResult.QualityMax;

		foreach(int itemtype in lNeedMaterial.Keys)
		{
			if(lNeedMaterial[itemtype] == 0 && info.Quality == NextQuality)
			{
				return CanQualityResult.QualityMax;
			}
		}

		//获取已有的资源。
		List<int> litemType = new List<int>();
		foreach(int itemtype in lNeedMaterial.Keys)
		{
			litemType.Add(itemtype);
		}
		ItemDC.GetItemCount(litemType,ref lhaveFragment);

		foreach(int itemtype in lNeedMaterial.Keys)
		{
			if(lhaveFragment[itemtype] < lNeedMaterial[itemtype])
			{
				return CanQualityResult.MaterialLimit;
			}
		}
		return CanQualityResult.CanUp;
	}
	/// <summary>
	/// 陷阱是否能升级.
	/// </summary>
	/// <returns><c>true</c> if can up level the specified Info; otherwise, <c>false</c>.</returns>
	/// <param name="Info">Info.</param>
	public static CanLevelResult GetLevelCanUP(BuildInfo Info)
	{
		int needCoin = 0;
		int needWood = 0;
		buildingM.GetUpLevelNeed(Info.BuildType,Info.Level,ref needCoin ,ref needWood) ;
		//获取已有的资源。
		int HaveCoin = UserDC.GetCoin() ;
		int HaveWood = UserDC.GetWood() ;
		if(Info.Level >= UserDC.GetLevel())
		{
			return CanLevelResult.LevelLimit;
		}
		else if(needCoin == 0 && needWood == 0)
		{
			return CanLevelResult.LevelMax;
		}
		else if(HaveCoin < needCoin)
		{
			return CanLevelResult.LessCoin;
		}
		else if(HaveWood < needWood)
		{
			return CanLevelResult.LessMater;
		}
		else if(HaveCoin >= needCoin && HaveWood >= needWood)
		{
			return CanLevelResult.CanUp;
			
		}
		else 
		{
			return CanLevelResult.LevelLimit;
		}
	}

	/// <summary>
	/// 陷阱能否升星.
	/// </summary>
	/// <returns>The can up star.</returns>
	/// <param name="Info">Info.</param>
	public static CanStarResult GetCanUpStar(BuildInfo Info)
	{
		if(Info.StarLevel == 5)
			return CanStarResult.StarMax;

		int NeedCoin = 0;
		int HaveCoin = 0;
		int NeedFragmentNum = 0;
		GetUpStarNeed(Info.BuildType ,Info.StarLevel + 1 ,ref  NeedFragmentNum ,ref  NeedCoin) ;

		int HaveFragmentNum = ItemDC.GetItemCount(Info.fragmentTypeID);//当前灵魂石
		HaveCoin = UserDC.GetCoin() ;
		
		

		
		if(HaveCoin < NeedCoin)
			return CanStarResult.LessCoin;

		if(HaveFragmentNum < NeedFragmentNum)
			return CanStarResult.LessMater;

		return  CanStarResult.CanUp;
	}
	/// <summary>
	/// 获取升级需要的材料及等级
	/// </summary>
	/// <param name="BuildType">类型</param>
	/// <param name="Quality">品阶</param>
	/// <param name="NeedMaterial">需要材料，key item id,value 数量</param>
	public static void GetUpQualityNeed(int BuildType ,int Quality ,ref Dictionary<int ,int> NeedMaterial ,ref int NeedLevel)
	{
		NeedLevel = 1 ;
		if(NeedMaterial == null)
			NeedMaterial = new Dictionary<int, int>();
		NeedMaterial.Clear();

		s_buildqualityInfo Info  = GetBuildQuality(BuildType, Quality);
		if(Info != null)
		{
			NeedLevel = Info.levellimit ;
			if(Info.num1 != 0 && Info.material1id > 0) 
				NeedMaterial.Add(Info.material1id ,Info.num1);
			if(Info.num2 > 0 && Info.material2id > 0) 
				NeedMaterial.Add(Info.material2id ,Info.num2);
			if(Info.num3 > 0 && Info.material3id > 0) 
				NeedMaterial.Add(Info.material3id ,Info.num3);
			if(Info.num4 > 0 && Info.material4id > 0) 
				NeedMaterial.Add(Info.material4id ,Info.num4);
			if(Info.num5 > 0 && Info.material5id >  0) 
				NeedMaterial.Add(Info.material5id ,Info.num5);
			if(Info.num6 > 0 && Info.material6id > 0) 
				NeedMaterial.Add(Info.material6id ,Info.num6);
		}
	}
	/// <summary>
	/// 获取升级需要的等级.
	/// </summary>
	public static int GetUpQualityLevelNeed(BuildInfo info)
	{
		s_buildqualityInfo qualityInfo  = GetBuildQuality(info.BuildType, info.Quality);
		if(qualityInfo != null)
		{
			return qualityInfo.levellimit ;

		}
		return 0;
	}
	/// <summary>
	/// 获取升星所需资源
	/// </summary>
	/// <param name="BuildType">类型</param>
	/// <param name="Star">星级</param>
	/// <param name="NeedMaterial">需要材料，key item id,value 数量</param>
	/// <param name="Star">建筑星级</param>
	/// <returns>建筑数据,null 获取失败</returns>
	public static void GetUpStarNeed(int BuildType ,int Star ,ref int fragmentNum ,ref int Coin)
	{
		Coin = 0 ;
		
		s_buildstarInfo Info  = GetBuildStar(BuildType, Star);
		if(Info != null)
		{
			Coin = Info.coin ;
			fragmentNum = Info.num;
		}
	}
	/// <summary>
	/// 获取升级所需资源
	/// </summary>
	/// <param name="BuildType">类型</param>
	/// <param name="level">等级</param>
	/// <param name="NeedGold">需要金币</param>
	/// <param name="Needwood">需要木材</param>
	public static void GetUpLevelNeed(int BuildType ,int level ,ref int NeedGold ,ref int Needwood)
	{
		NeedGold = 0 ;
		Needwood = 0 ;
		s_buildupInfo Info  = GetBuildUp(BuildType, level);
		if(Info != null)
		{
			NeedGold = Info.coin;
			Needwood = Info.wood;
		}
	}
	/// <summary>
	/// 拆解陷阱获取资源
	/// </summary>
	/// <param name="BuildType">类型</param>
	/// <param name="level">等级</param>
	/// <param name="wood">获得木材</param>
	/// <param name="coin">获得金币</param>
	public static bool GetBuildAnanlyze(int BuildType ,int level ,ref int wood ,ref int coin)
	{
		wood = 0 ;
		coin = 0 ;
		foreach(s_buildanalyzeInfo Info in m_lBuildannalyze)
		{
			if(Info.buildtype == BuildType && Info.level == level)
			{
				wood = Info.wood;
				coin = Info.coin;
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// 检测是否满足召唤条件（钱够，碎片够）
	/// </summary>
	public static bool CheckCanSummon(int buildTypeId)
	{
		foreach(s_building_typeInfo BaseInfo in m_lType) 
		{
			if (BaseInfo == null) continue;
			if (BaseInfo.id == buildTypeId)
			{
				int NeedCoin = 0;
				int fragment = 0;
				int NeedNum = 0;
				GetUpStarNeed(buildTypeId, BaseInfo.starlevel, ref NeedNum, ref  NeedCoin);
				int Have = ItemDC.GetItemCount(BaseInfo.fragment);//当前灵魂石
				if (Have >= NeedNum)
				{
					return true;   
				}
				return false;
			}
		}
		return false;
	}

}

