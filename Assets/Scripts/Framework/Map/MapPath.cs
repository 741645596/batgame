using UnityEngine;
using System.Collections;
using System.Collections.Generic;




public class MapPath  {


	public static  List<MapGrid> GetPath(MapGrid startN, MapGrid endN){
		return Search(startN, endN);
		
	}
	public static float GetPathScoreG(MapGrid startN, MapGrid endN)
	{
		//同层则互为SpecialsStation，同时计算权值
		if (startN.GridPos.Layer == endN.GridPos.Layer) 
		{
			startN.pNearestSpecialStation = endN;
			startN.fNearestSpecialscoreG =Mathf.Abs(startN.GridPos.Unit-endN.GridPos.Unit)*MapGrid.fcounstScoreG;
			return startN.fNearestSpecialscoreG;
		}
		MapGrid currentNode=AstarSearch(startN, endN);
		float fTempScoreG =0;
		while(currentNode!=null){
			MapGrid preGrid = currentNode.preNode;
			if(preGrid==null)
				break;
			for(int nNSDir=(int)NearestStationDir.NSD_LEFTDOWN; nNSDir<(int)NearestStationDir.NSD_END;nNSDir++)
			{
				if(currentNode==preGrid.m_NpNearestStation[nNSDir])
				{
					fTempScoreG+=preGrid.m_NfNearestStationscoreG[nNSDir];
					break;
				}
			}
			if(currentNode==preGrid.pNearestSpecialStation)
			{
				fTempScoreG+=preGrid.fNearestSpecialscoreG;
			}
			currentNode = preGrid;
		}
		ResetGraph();
		return fTempScoreG;
	}
	private static  List<MapGrid> Search(MapGrid startN, MapGrid endN)
	{
		//同层则互为SpecialsStation，同时计算权值
		if (startN.GridPos.Layer == endN.GridPos.Layer && MapGrid.CheckLink(startN,endN)) 
		{
			startN.pNearestSpecialStation = endN;
			startN.fNearestSpecialscoreG =Mathf.Abs(startN.GridPos.Unit-endN.GridPos.Unit)*0.5f;
		}
		MapGrid currentNode=AstarSearch(startN, endN);
		//trace back the path through closeList
		List<MapGrid> listMapgrid=new List<MapGrid>();
		if (currentNode != null) 
		{
			while(currentNode!=null){
				listMapgrid.Add(currentNode);
				MapGrid preGrid = currentNode.preNode;
				if(null!=preGrid&&currentNode.GridPos.Layer == preGrid.GridPos.Layer) 
				{
					if(currentNode.GridPos.Unit<preGrid.GridPos.Unit)
					{
						currentNode=currentNode.Right;
						while(currentNode!=null&&currentNode!=preGrid){
							listMapgrid.Add(currentNode);
							currentNode=currentNode.Right;
						}
					}
					else{
						currentNode=currentNode.Left;
						while(currentNode!=null&&currentNode!=preGrid){
							listMapgrid.Add(currentNode);
							currentNode=currentNode.Left;
						}
					}
				}
				else{
					currentNode = preGrid;
				}
			}
			listMapgrid.Reverse();
		}
		ResetGraph();
		return listMapgrid;
	}
	
	public static MapGrid[] s_openList;//=new MapGrid[n];
	public static int s_nOpenListLength=0;
	private static MapGrid AstarSearch(MapGrid startN, MapGrid endN)
	{
		//set start as currentNode
		MapGrid currentNode=startN;
		//List<MapGrid> closeList=new List<MapGrid>();
		//openlist, all the possible node that yet to be on the path, the number can only be as much as the number of node in the garph
		int n = MapGrid.GetMapGridCount();
		if (s_nOpenListLength != n) 
		{
			s_openList = new MapGrid[n];
			s_nOpenListLength = n;

		}
		MapGrid[] openList=s_openList;
		
		//an array use to record the element number in the open list which is empty after the node is removed to be use as currentNode,
		//so we can use builtin array with fixed length for openlist, also we can loop for the minimal amount of node in every search
		List<int> openListRemoved=new List<int>();
		//current count of elements that are occupied in openlist, openlist[n>openListCounter] are null
		int openListCounter=0;
		
		//set start as currentNode
		//MapGrid currentNode=startN;
		
		//use to compare node in the openlist has the lowest score, alwyas set to Infinity when not in used
		float currentLowestF=Mathf.Infinity;
		
		//loop start
		while(true){
			
			//if we have reach the destination
			if(currentNode==endN) 
			{
				break;
			}

			for(int nNSDir=(int)NearestStationDir.NSD_LEFTDOWN; nNSDir<(int)NearestStationDir.NSD_END;nNSDir++)
			{
				if(currentNode==endN.m_NpNearestStation[nNSDir])
				{
					if(currentNode.GridPos.Layer != endN.GridPos.Layer && (endN.Type == GridType.GRID_HOLE || endN.Type == GridType.GRID_HOLESTAIR))
						continue;
					currentNode.pNearestSpecialStation = endN;
					currentNode.fNearestSpecialscoreG = endN.m_NfNearestStationscoreG[nNSDir];
					break;
				}
			}

			//move currentNode to closeList;
			currentNode.listState= MapGrid._ListState.Close;
			
			//分支start
			//Vector3 pos = endN.pos;
			//List<MapGrid> l = new List<MapGrid>();
			//List<float> lcost = new List<float>();
			for(int nNSDir=(int)NearestStationDir.NSD_LEFTDOWN; nNSDir<(int)NearestStationDir.NSD_END;nNSDir++)
			{
				if(currentNode.m_NpNearestStation[nNSDir]!=null)
				{

					bool bratHole=false;//是否无底洞
					MapGrid nodeDown = currentNode.m_NpNearestStation[nNSDir];
					while(nodeDown!=null&&nodeDown.Type==GridType.GRID_HOLE)
					{
						bratHole=true;
						nodeDown = nodeDown.Down;
						if(nodeDown!=null&&nodeDown.Type!=GridType.GRID_HOLE)
						{
							bratHole=false;
							break;
						}
					}

					if(!bratHole)
					{
						ProcessNeighbour(currentNode,currentNode.m_NpNearestStation[nNSDir],currentNode.m_NfNearestStationscoreG[nNSDir],endN.pos);
						MapGrid neighbour = currentNode.m_NpNearestStation[nNSDir];
						if(neighbour.listState==MapGrid._ListState.Unassigned && neighbour.walkable) {
							//set the node state to open
							neighbour.listState=MapGrid._ListState.Open;
							//if there's an open space in openlist, fill the space
							if(openListRemoved.Count>0){
								openList[openListRemoved[0]]=neighbour;
								//remove the number from openListRemoved since this element has now been occupied
								openListRemoved.RemoveAt(0);
							}
							//else just stack on it and increase the occupication counter
							else{
								openList[openListCounter]=neighbour;
								openListCounter+=1;
							}
						}
					}
				}
			}
			if (currentNode.pNearestSpecialStation !=null)
			{
				ProcessNeighbour(currentNode,currentNode.pNearestSpecialStation,currentNode.fNearestSpecialscoreG,endN.pos);
				MapGrid neighbour = currentNode.pNearestSpecialStation;
				if(neighbour.listState==MapGrid._ListState.Unassigned && neighbour.walkable) {
					//set the node state to open
					neighbour.listState=MapGrid._ListState.Open;
					//if there's an open space in openlist, fill the space
					if(openListRemoved.Count>0){
						openList[openListRemoved[0]]=neighbour;
						//remove the number from openListRemoved since this element has now been occupied
						openListRemoved.RemoveAt(0);
					}
					//else just stack on it and increase the occupication counter
					else{
						openList[openListCounter]=neighbour;
						openListCounter+=1;
					}
				}
			}
			/*
			//loop through the neighbour of current loop, calculate  score and stuff
			List<MapGrid> neighbourNode = ProcessNeighbour(currentNode,endN);
			//put all neighbour in openlist
			foreach(MapGrid neighbour in neighbourNode){
				if(neighbour.listState==MapGrid._ListState.Unassigned && neighbour.walkable) {
					//set the node state to open
					neighbour.listState=MapGrid._ListState.Open;
					//if there's an open space in openlist, fill the space
					if(openListRemoved.Count>0){
						openList[openListRemoved[0]]=neighbour;
						//remove the number from openListRemoved since this element has now been occupied
						openListRemoved.RemoveAt(0);
					}
					//else just stack on it and increase the occupication counter
					else{
						openList[openListCounter]=neighbour;
						openListCounter+=1;
					}
				}
			}
			*/
			//分支end
			//clear the current node, before getting a new one, so we know if there isnt any suitable next node
			currentNode=null;
			
			//get the next point from openlist, set it as current point
			//just loop through the openlist until we reach the maximum occupication
			//while that, get the node with the lowest score
			currentLowestF=Mathf.Infinity;
			int id=0;	//use element num of the node with lowest score in the openlist during the comparison process
			//int i=0;		//universal int value used for various looping operation
			for(int i=0; i<openListCounter; i++){
				if(openList[i]!=null){
					if(openList[i].scoreF<currentLowestF){
						currentLowestF=openList[i].scoreF;
						currentNode=openList[i];
						id=i;
					}
				}
			}
			
			//if there's no node left in openlist, path doesnt exist
			if(currentNode==null) {
				//pathFound=false;
				//return new List<MapGrid>();
				break;
			}
			
			//remove the new currentNode from openlist
			openList[id]=null;
			//put the id into openListRemoved so we know there's an empty element that can be filled in the next loop
			openListRemoved.Add(id);

		}
		return currentNode;
	}


	public static  void ResetGraph(){
		MapM.ResetGraph ();
	}
	//分支start
	private static void ProcessNeighbour(MapGrid node,MapGrid nodeDir,float fscoreG,Vector3 vEndPos)
	{
		//if the neightbour state is clean (never evaluated so far in the search)
		if(nodeDir.listState==MapGrid._ListState.Unassigned){
			//check the score of G and H and update F, also assign the parent to currentNode
			nodeDir.scoreG=node.scoreG+fscoreG;

			int nLayer =nodeDir.GridPos.Layer-node.GridPos.Layer;
			//nLayer=nLayer>0?nLayer:-nLayer;
			float fLayerScoreH= nLayer*MapGrid.fcounstScoreG;

			int nUnit=nodeDir.GridPos.Unit-node.GridPos.Unit;
			//nUnit=nUnit>0?nUnit:-nUnit;
			float fnUnitScoreH = nUnit*MapGrid.fcounstScoreG;

			nodeDir.scoreH = Mathf.Sqrt(fLayerScoreH*fLayerScoreH+fnUnitScoreH*fnUnitScoreH);
			nodeDir.scoreF = nodeDir.scoreG +nodeDir.scoreH;
			nodeDir.preNode=node;
		}
		//if the neighbour state is open (it has been evaluated and added to the open list)
		else if(nodeDir.listState==MapGrid._ListState.Open){
			//calculate if the path if using this neighbour node through current node would be shorter compare to previous assigned parent node
			float tempScoreG=node.scoreG+fscoreG;
			if(nodeDir.scoreG>tempScoreG){
				//if so, update the corresponding score and and reassigned parent
				nodeDir.preNode=node;
				nodeDir.scoreG=tempScoreG;
				nodeDir.scoreF = nodeDir.scoreG + nodeDir.scoreH;
			}
		}
	}
	/*
	private static List<MapGrid> ProcessNeighbour(MapGrid node,MapGrid endN)
	{
		Vector3 pos = endN.pos;
		List<MapGrid> l = new List<MapGrid>();
		List<float> lcost = new List<float>();
		for(int nNSDir=(int)NearestStationDir.NSD_LEFTTOP; nNSDir<(int)NearestStationDir.NSD_END;nNSDir++)
		{
			if(node.m_NpNearestStation[nNSDir]!=null)
			{
				l.Add(node.m_NpNearestStation[nNSDir]);
				lcost.Add(node.m_NfNearestStationscoreG[nNSDir]);
			}
		}
		if (node.pNearestSpecialStation !=null)
		{
			l.Add(node.pNearestSpecialStation);
			lcost.Add(node.fNearestSpecialscoreG);
		}

		for(int i=0; i<l.Count; i++){
			//Debug.Log(neighbourNode[i].listState);
			//if the neightbour state is clean (never evaluated so far in the search)
			if(l[i].listState==MapGrid._ListState.Unassigned){
				//check the score of G and H and update F, also assign the parent to currentNode
				l[i].scoreG=node.scoreG+lcost[i];
				l[i].scoreH=Vector3.Distance(l[i].pos, pos);
				l[i].scoreF = l[i].scoreG + l[i].scoreH;
				l[i].preNode=node;
			}
			//if the neighbour state is open (it has been evaluated and added to the open list)
			else if(l[i].listState==MapGrid._ListState.Open){
				//calculate if the path if using this neighbour node through current node would be shorter compare to previous assigned parent node
				float tempScoreG=node.scoreG+lcost[i];
				if(l[i].scoreG>tempScoreG){
					//if so, update the corresponding score and and reassigned parent
					l[i].preNode=node;
					l[i].scoreG=tempScoreG;
					l[i].scoreF = l[i].scoreG + l[i].scoreH;
				}
			}
		}
		return l;
	}
*/
	//分支 end
	public  static int FindShortestTarget(List<MapGrid> t,MapGrid start)
	{
		if (t == null || t.Count == 0 || start == null )
			return -1;
		int nTargetCount = t.Count;
		if (nTargetCount == 1)
			return  0;
		
		int iTarget = -1;
		float fScoreGMax = 1000000;
		//最短路径查找方法。
		for (int i = 0; i < nTargetCount; i++)
		{
			if(t[i] == null) continue;
			float fScore = MapPath.GetPathScoreG(start,t[i]);
			if (fScore < 0||fScore>=fScoreGMax)
					continue;
			fScoreGMax =fScore;
			iTarget = i;
		}
		return iTarget;
	}











}
