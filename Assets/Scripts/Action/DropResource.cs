using UnityEngine;
using System.Collections;

public class DropResource  {

    public static void Drop(ResourceType t, int count, Vector3 pos)
    {
        if (t == ResourceType.Wood)
        {
            int num1 = ConfigM.GetBuildSourceDropIRow((int)ResourceType.Wood, count).num1;
            int num2 = ConfigM.GetBuildSourceDropIRow((int)ResourceType.Wood, count).num2;
            int num3 = ConfigM.GetBuildSourceDropIRow((int)ResourceType.Wood, count).num3;
            int units = num1 + num2 * 3 + num3 * 9;
            int unitcount = (int)count / units;
            DropResourceEffect("Wood1", t, num1, unitcount * num1 + count - units * unitcount,pos);
			DropResourceEffect("Wood2", t, num2, unitcount * num2 * 3,pos);
			DropResourceEffect("Wood3", t, num3, unitcount * num3 * 9,pos);

        }
        else if (t == ResourceType.Stone)
        {
        }
        else if (t == ResourceType.Steel)
        {
        }
    }



	public static void Drop(sdata.s_itemtypeInfo item, Vector3 pos)
	{
		if(item != null)
			DropResourceBoxEffect("box",item,pos);
	}
	

		
	static void DropResourceEffect(string name, ResourceType t, int num, int count, Vector3 pos)
	{
		
		pos = BattleEnvironmentM.Local2WorldPos(pos);
		pos.x -= num/2f;
        for (int i = 0; i < num; i++)
        {
            int n = (int)count / num;
            if (i == 0)
                n += count - num * ((int)count / num);
			pos.x += 1;//Random.Range(0,2) == 0 ? - Random.Range(0,0.5f*num):Random.Range(0,0.5f*num);
			pos.z = 0;
			GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", name,EffectCamera.GetEffectPos(pos) ,BattleEnvironmentM.GetLifeMBornNode(true));
			//EffectCamera.AddFollowList(gae.transform,pos);
			if(gae != null)
			{
				Random.seed ++;
				GameObjectActionResourceDrop gaw = new GameObjectActionResourceDrop(2f, pos,pos + new Vector3(Random.Range(-1.5f,1.5f),0,0));
				gae.AddAction(gaw);
				GameObjectActionResourceFlyToUI gar = new GameObjectActionResourceFlyToUI();
				gar.SetData(EffectCamera.camera,WndManager.GetNGUICamera(),n,t);
				gae.AddAction(gar);

				Animator ani = gae.gameObject.GetComponent<Animator>();
				if (null != ani)
				{
					int nValue = (int)Random.value % 2;
					if (nValue == 0)
						nValue = 2;
					ani.SetInteger("iState", nValue);
				}
			}
		}
	}



	static void DropResourceBoxEffect(string name, sdata.s_itemtypeInfo item, Vector3 pos)
	{
		pos = BattleEnvironmentM.Local2WorldPos(pos);
		pos.x += 0.5f;
		pos.z = 0;
		GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", name,EffectCamera.GetEffectPos(pos) ,BattleEnvironmentM.GetLifeMBornNode(true));
		EffectCamera.AddFollowList(gae.transform,pos);
		if(gae != null)
		{
			//先等待
			GameObjectActionBoxWait gaw = new GameObjectActionBoxWait(2f);
			gae.AddAction(gaw);
			//open box
			GameObjectActionOpenBox gaopen = new GameObjectActionOpenBox(2.0f ,item);
			gae.AddAction(gaopen);

			Animator ani = gae.gameObject.GetComponent<Animator>();
			if (null != ani)
			{
				int nValue = (int)Random.value % 2;
				if (nValue == 0)
					nValue = 2;
				ani.SetInteger("iState", nValue);
			}
		}
	}
}
