using UnityEngine;

	/** Holds a coordinate in integers */
[System.Serializable]
	public struct Int3 {
	    public int GridStart;
	    public int RoomStart;
	    public int GridLength;
		
		//These should be set to the same value (only PrecisionFactor should be 1 divided by Precision)
		
		/** Precision for the integer coordinates.
		 * One world unit is divided into [value] pieces. A value of 1000 would mean millimeter precision, a value of 1 would mean meter precision (assuming 1 world unit = 1 meter).
		 * This value affects the maximum coordinates for nodes as well as how large the cost values are for moving between two nodes.
		 * A higher value means that you also have to set all penalty values to a higher value to compensate since the normal cost of moving will be higher.
		 */
		public const int Precision = 1000;
		
		/** #Precision as a float */
		public const float FloatPrecision = 1000F;
		
		/** 1 divided by #Precision */
		public const float PrecisionFactor = 0.001F;
		
		/* Factor to multiply cost with */
		//public const float CostFactor = 0.01F;
		
		private static Int3 _zero = new Int3(0,0,0);
		public static Int3 zero { get { return _zero; } }
		
		
		
		public Int3 (int _x, int _y, int _z) {
		    GridStart = _x;
		    RoomStart = _y;
		    GridLength = _z;
		}

		
		public static bool operator == (Int3 lhs, Int3 rhs) {
		        return  lhs.GridStart == rhs.GridStart &&
			            lhs.GridLength == rhs.GridLength &&
				     lhs.RoomStart == rhs.RoomStart;
		}
		
		public static bool operator != (Int3 lhs, Int3 rhs) {
		return 	lhs.GridStart != rhs.GridStart ||
			    lhs.GridLength != rhs.GridLength ||
				lhs.RoomStart != rhs.RoomStart;
		}
		
		public static explicit operator Int3 (Vector3 ob) {
			return new Int3 (
				(int)System.Math.Round (ob.x*FloatPrecision),
				(int)System.Math.Round (ob.y*FloatPrecision),
				(int)System.Math.Round (ob.z*FloatPrecision)
				);
			//return new Int3 (Mathf.RoundToInt (ob.x*FloatPrecision),Mathf.RoundToInt (ob.y*FloatPrecision),Mathf.RoundToInt (ob.z*FloatPrecision));
		}

		
		public static Int3 operator - (Int3 lhs, Int3 rhs) {
		lhs.GridStart -= rhs.GridStart;
		lhs.GridLength -= rhs.GridLength;
		lhs.RoomStart -= rhs.RoomStart;
			return lhs;
		}
		
		public static Int3 operator - (Int3 lhs) {
		lhs.GridStart = -lhs.GridStart;
		lhs.GridLength = -lhs.GridLength;
		lhs.RoomStart = -lhs.RoomStart;
			return lhs;
		}
		
		public static Int3 operator + (Int3 lhs, Int3 rhs) {
		lhs.GridStart += rhs.GridStart;
		lhs.GridLength += rhs.GridLength;
		lhs.RoomStart += rhs.RoomStart;
			return lhs;
		}
		
		public static Int3 operator * (Int3 lhs, int rhs) {
		lhs.GridStart *= rhs;
		lhs.GridLength *= rhs;
		lhs.RoomStart *= rhs;
			
			return lhs;
		}
		
		public static Int3 operator * (Int3 lhs, float rhs) {
		lhs.GridStart = (int)System.Math.Round (lhs.GridStart * rhs);
		lhs.GridLength = (int)System.Math.Round (lhs.GridLength * rhs);
		lhs.RoomStart = (int)System.Math.Round (lhs.RoomStart * rhs);
			
			return lhs;
		}
		
		public static Int3 operator * (Int3 lhs, double rhs) {
		lhs.GridStart = (int)System.Math.Round (lhs.GridStart * rhs);
		lhs.GridLength = (int)System.Math.Round (lhs.GridLength * rhs);
		lhs.RoomStart = (int)System.Math.Round (lhs.RoomStart * rhs);
			
			return lhs;
		}
		
		public static Int3 operator * (Int3 lhs, Vector3 rhs) {
		lhs.GridStart = (int)System.Math.Round (lhs.GridStart * rhs.x);
		lhs.GridLength =	(int)System.Math.Round (lhs.GridLength * rhs.y);
		lhs.RoomStart = (int)System.Math.Round (lhs.RoomStart * rhs.z);
			
			return lhs;
		}
		
		public static Int3 operator / (Int3 lhs, float rhs) {
		lhs.GridStart = (int)System.Math.Round (lhs.GridStart / rhs);
		lhs.GridLength = (int)System.Math.Round (lhs.GridLength / rhs);
		lhs.RoomStart = (int)System.Math.Round (lhs.RoomStart / rhs);
			return lhs;
		}



		
		public static implicit operator string (Int3 ob) {
			return ob.ToString ();
		}
		
		/** Returns a nicely formatted string representing the vector */
		public override string ToString () {
		return "( "+GridStart+", "+GridLength+", "+RoomStart+")";
		}


	}
	
	/** Two Dimensional Integer Coordinate Pair */
[System.Serializable]
	public struct Int2 {
	    public int Unit;
	    public int Layer;
		
	public Int2 (int Unit, int Layer) {
		    this.Unit = Unit;
		    this.Layer = Layer;
		}


	private static Int2 _zero = new Int2(0,0);
	public static Int2 zero { get { return _zero; } }
		
		public int sqrMagnitude {
			get {
			     return Unit*Unit + Layer * Layer;
			}
		}
		
		public long sqrMagnitudeLong {
			get {
			    return (long)Unit *(long)Unit + (long)Layer *(long)Layer;
			}
		}
		
		public static Int2 operator + (Int2 a, Int2 b) {
		return new Int2 (a.Unit + b.Unit, a.Layer + b.Layer);
		}
		
		public static Int2 operator - (Int2 a, Int2 b) {
		return new Int2 (a.Unit-b.Unit, a.Layer - b.Layer);
		}
		
		public static bool operator == (Int2 a, Int2 b) {
		return a.Unit == b.Unit && a.Layer == b.Layer;
		}
		
		public static bool operator != (Int2 a, Int2 b) {
		return a.Unit != b.Unit || a.Layer != b.Layer;
		}
		
		public static int Dot (Int2 a, Int2 b) {
		return a.Unit *b.Unit + a.Layer * b.Layer;
		}
		
		public static long DotLong (Int2 a, Int2 b) {
		return (long)a.Unit *(long)b.Unit + (long)a.Layer *(long)b.Layer;
		}
		
		public override bool Equals (System.Object o) {
			if (o == null) return false;
			Int2 rhs = (Int2)o;
			
		return Unit == rhs.Unit && Layer == rhs.Layer;
		}
		
		public override int GetHashCode () {
		return Unit * 49157 + Layer * 98317;
		}
		
		/** Matrices for rotation.
		 * Each group of 4 elements is a 2x2 matrix.
		 * The XZ position is multiplied by this.
		 * So
		 * \code
		 * //A rotation by 90 degrees clockwise, second matrix in the array
		 * (5,2) * ((0, 1), (-1, 0)) = (2,-5)
		 * \endcode
		 */
		private static readonly int[] Rotations = {
			 1, 0, //Identity matrix
			 0, 1,
			
			 0, 1,
			-1, 0,
			
			-1, 0,
			 0,-1,
			
			 0,-1,
			 1, 0
		};
		
		/** Returns a new Int2 rotated 90*r degrees around the origin. */
		public static Int2 Rotate ( Int2 v, int r ) {
			r = r % 4;
		return new Int2 ( v.Unit *Rotations[r*4+0] + v.Layer * Rotations[r*4+1], v.Unit * Rotations[r*4+2] + v.Layer * Rotations[r*4+3] );
		}
		
		public static Int2 Min (Int2 a, Int2 b) {
		return new Int2 (System.Math.Min (a.Unit,b.Unit), System.Math.Min (a.Layer,b.Layer));
		}
		
		public static Int2 Max (Int2 a, Int2 b) {
		return new Int2 (System.Math.Max (a.Unit,b.Unit), System.Math.Max (a.Layer,b.Layer));
		}
		
		
		public static Int3 ToInt3XZ (Int2 o) {
		return new Int3 (o.Unit,0,o.Layer);
		}
		
		public override string ToString ()
		{
		return "("+Unit+", " +Layer+")";
		}
	}

