using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Logic
{
	/// <summary>
	/// path 相关处理 
	/// </summary>
	public class PathUtil
	{
		static int subdivisions = 2;
		static bool uniformLength = true;
		static float maxSegmentLength = 0.3F;
		static float strength = 0.5F;
		static int iterations = 2;


		public static void SetPathParam(int sub,bool uniform,float maxSegment,float strengt,int iteration)
		{
			subdivisions = sub;
			uniformLength = uniform;
			maxSegmentLength = maxSegment;
			strength = strengt;
			iterations = iteration;
		}


		public static void Reset()
		{
			subdivisions = 2;
			uniformLength = true;
			maxSegmentLength = 0.3F;
			strength = 0.1F;
			iterations = 2;
		}
		//
		public static List<Vector3> SmoothSimple (List<Vector3> path) {
			if (path.Count < 2) {
				return path;
			}
			
			if (uniformLength) 
			{
				int numSegments = 0;
				maxSegmentLength = maxSegmentLength < 0.005F ? 0.005F : maxSegmentLength;
				for (int i=0;i<path.Count-1;i++) 
				{
					float length = Vector3.Distance (path[i],path[i+1]);
					numSegments += Mathf.FloorToInt (length / maxSegmentLength);
				}
				
				List<Vector3> subdivided = ListPool<Vector3>.Claim (numSegments+1);
				
				int c = 0;
				
				float carry = 0;
				
				for (int i=0;i<path.Count-1;i++) {
					
					float length = Vector3.Distance (path[i],path[i+1]);
					
					int numSegmentsForSegment = Mathf.FloorToInt ((length + carry) / maxSegmentLength);
					
					float carryOffset = carry/length;
					//float t = 1F / numSegmentsForSegment;
					
					Vector3 dir = path[i+1] - path[i];
					
					for (int q=0;q<numSegmentsForSegment;q++) {
						subdivided.Add (dir*(System.Math.Max (0, (float)q/numSegmentsForSegment - carryOffset)) + path[i]);
						c++;
					}
					
					carry = (length + carry) % maxSegmentLength;
				}
				
				subdivided.Add (path[path.Count-1]);
				
				if (strength != 0) {
					for (int it = 0; it < iterations; it++) {
						Vector3 prev = subdivided[0];
						
						for (int i=1;i<subdivided.Count-1;i++) {
							
							Vector3 tmp = subdivided[i];
							
							subdivided[i] = Vector3.Lerp (tmp, (prev+subdivided[i+1])/2F,strength);
							
							prev = tmp;
						}
					}
				}
				
				return subdivided;
			} 
			else 
			{
				List<Vector3> subdivided = ListPool<Vector3>.Claim ();
				//Polygon.Subdivide (path,subdivisions);
				if (subdivisions < 0) subdivisions = 0;
				
				int steps = 1 << subdivisions;
				
				for (int i=0;i<path.Count-1;i++)
					for (int j=0;j<steps;j++)
						subdivided.Add (Vector3.Lerp (path[i],path[i+1],(float)j / steps));
				
				for (int it = 0; it < iterations; it++) {
					Vector3 prev = subdivided[0];
					
					for (int i=1;i<subdivided.Count-1;i++) {
						
						Vector3 tmp = subdivided[i];
						
						subdivided[i] = Vector3.Lerp (tmp, (prev+subdivided[i+1])/2F,strength);
						
						prev = tmp;
					}
				}
				return subdivided;
			}
		}//end smoothSimple
	}
}