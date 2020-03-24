using UnityEngine;
using System.Collections;
public enum roombgtype
{
	none,//上下左都有房间
	l,//左无房间，右上有房间
	r,//右无房间，左上有房间
	t,//上无房间，左右有房间
	lr,//左右无房间，上有房间
	lrt,//左右上无房间
}
public class BuildingRoomBg : MonoBehaviour {
	public Texture2D Texnone;
	public Texture2D Texl;
	public Texture2D Texr;
	public Texture2D Text;
	public Texture2D Texlr;
	public Texture2D Texlrt;
	public Int2 m_pos;
	public roombgtype m_type = roombgtype.none;
	// Use this for initialization
	void Start () {
		if (m_type != roombgtype.none)
		{
			Int2 Pos = m_pos;
			MapGrid m = MapGrid.GetMG(Pos);
			if(m != null)
			{
				int BuildRoomSceneID =  -1;
				if(m.GetBuildRoom (ref BuildRoomSceneID) == true)
				{
					Life buildRoom = CM.GetLifeM(BuildRoomSceneID ,LifeMType.BUILD);
					if(buildRoom != null) 
					{
						//m_rooms.Add(buildRoom as BuildingRoom);
						(buildRoom as Building).AddBg(this);
						//buildRoom.ad
					}
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void DestroyRoom()
	{
		if (m_type == roombgtype.l)
			GetComponent<Renderer>().material.mainTexture = Texl;
		else if (m_type == roombgtype.r)
			GetComponent<Renderer>().material.mainTexture = Texr;
		else if (m_type == roombgtype.t)
			GetComponent<Renderer>().material.mainTexture = Text;
		else if (m_type == roombgtype.lr)
			GetComponent<Renderer>().material.mainTexture = Texlr;
		else if (m_type == roombgtype.lrt)
			GetComponent<Renderer>().material.mainTexture = Texlrt;
	}
}
