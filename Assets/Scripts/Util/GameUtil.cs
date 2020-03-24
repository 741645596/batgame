// game util function
//
//
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

	public partial class GameUtil
	{
		
		static int[] _value_arr = new int[] { 20, 40, 100, 200, 400, 1000, 2000, 4000, 10000, 20000, 40000, 100000, 200000, 400000,};
		static int[] _multi_arr = new int[] { 1, 2, 5, 10, 20, 50, 100, 200, 500, 1000, 2000, 5000, 10000, 20000,};

		public static int FormulaFloor(float f, int flag)
		{
			int val = (int)f;
			int sign = (val <= 0) ? -1 : 1;
	
			val = Math.Abs (val);
	
			if (val > (flag == 1 ? _multi_arr [13] * 100 : _value_arr [13]))
				val = val / 50000 * 50000;
			else {
				int i = 0;
				for (i = 0; i < 14; i++) {
					if (val <= (flag == 1 ? _multi_arr [i] * 100 : _value_arr [i]))
						break;
				}
				val = val / _multi_arr [i] * _multi_arr [i];
			}
	
			return sign * val;
		}
		
		/// <summary>
		/// 获得数值符号
		/// </summary>
		public static int Sig(int i)
		{
			if (i > 0)
				return 1;
			if (i < 0)
				return -1;
			return 0;
		}
		
		public static string ConvertToCSFormat(string c_format_text)
		{
			List<char> cs_format_list = new List<char> ();
			int format_count = 0;

			string temp_text = c_format_text.Replace ("<br/>", "\\n");

			// Convert %s %O %d to {0} {1} {2} format
			int len = temp_text.Length;
			for (int i = 0; i < len; i++) {
				char ch = temp_text [i];
				if (ch != '%') {
					if ((ch == '{') || (ch == '}'))
						cs_format_list.Add (ch);

					cs_format_list.Add (ch);
					continue;
				}

				if (i + 1 >= len) {
					cs_format_list.Add (ch);
					continue;
				}

				char next_ch = temp_text [i + 1];
				if ((next_ch == 'd') ||
			    (next_ch == 's') ||
			    (next_ch == 'O')) {
					cs_format_list.AddRange (string.Format ("{{{0}}}", format_count).ToCharArray ());
					format_count++;
					i++;
					continue;
				}

				if (next_ch == '%') {
					cs_format_list.Add (next_ch);
					i++;
					continue;
				}
			}

			return new string (cs_format_list.ToArray ());
		}
		
		public static int atoi(string text)
	    {
	        int val = 0;
	        try
	        {
	            val = System.Convert.ToInt32(text);
	        }
	        catch
	        {
	            val = 0;
	        }
	        return val;
	    }
		
		public static string[] Explode(string path, string seperator)
	    {
	        return path.Split(seperator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
	    }
	
	    public static string Implode(string[] parts, string seperator)
	    {
	        if (parts.Length < 1)
	            return string.Empty;
	
	        if (parts.Length < 2)
	            return parts[0];
	
	        StringBuilder builder = new StringBuilder(string.Empty);
	        builder.Append(parts[0]);
	        for (int i = 1; i < parts.Length; i++)
	            builder.AppendFormat("{0}{1}", seperator, parts[i]);
	
	        return builder.ToString();
	    }
	
	    public static string Implode(string[] parts, string seperator, int start_index, int end_index)
	    {
	        if (start_index >= parts.Length)
	            return string.Empty;
	
	        if (start_index < 0)
	            start_index = 0;
	
	        if (end_index >= parts.Length)
	            end_index = parts.Length - 1;
	
	        List<string> part_list = new List<string>();
	        for (int i = start_index; i <= end_index; i++)
	            part_list.Add(parts[i]);
	
	        return Implode(part_list.ToArray(), seperator);
	    }
	
		/// <summary>
		/// 数值除以10再加上百分号
		/// </summary>
		public static string ChangeToPersent(int val)
		{
			float f = val * 0.1f;
			f = float.Parse(f.ToString("F1"));
			return f.ToString() + "%";
		}
		
		/// <summary>
		/// 构造字典序结构
		/// </summary>
		public static Dictionary<string, object> NewMapping(params object[] args)
		{
			// 参数一定是偶数个
			Debug.Assert(args.Length % 2 == 0);
			
			// 打包为mapping
			Dictionary<string, object> m = new Dictionary<string, object>();
			for (int i = 0; i < args.Length; i += 2)
			{
				string k = args[i] as string;
				object v = args[i + 1];
				
				m[k] = v;
			}
			
			// 返回结果
			return m;
		}
		
		/// <summary>
		/// 转换字符串为标志位
		/// </summary>
		public static int ConvertFieldToFlags(string value_text, Dictionary<char, int> field_table)
	    {
	        int result = 0;
	        foreach (char c in value_text)
	        {
	            if (field_table.ContainsKey(c))
	                result |= field_table[c];
	        }
	        return result;
	    }
		
		/// <summary>
		/// 根据包裹页及位置索引构造真实的包裹位置
		/// </summary>
		/// <returns>
		/// 真实的包裹位置
		/// </returns>
		/// <param name='page'>
		/// 包裹页
		/// </param>
		/// <param name='index_in_page'>
		/// 位置索引
		/// </param>
		/// 
		public static string MakePos(int page, int index_in_page)
	    {
	        return string.Format("{0}-{1}-{2}", page, 0, index_in_page);
	    }
		
		/// <summary>
		/// 从真实的包裹位置中读取包裹页和页中的索引
		/// </summary>
		/// <returns>
		/// 是否读取成功
		/// </returns>
		/// <param name='pos'>
		/// 真实的包裹位置
		/// </param>
		/// <param name='page'>
		/// 包裹页
		/// </param>
		/// <param name='index_in_page'>
		/// 页中索引
		/// </param>
	    public static bool ReadPos(string pos, ref int page, ref int index_in_page)
	    {
	        string[] str_list = pos.Split("-".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
	        if (str_list.Length < 3)
	            return false;
	
	        page = System.Convert.ToInt32(str_list[0]);
	        index_in_page = System.Convert.ToInt32(str_list[2]);
	        return true;
	    }
		
		public static float Fxn(params object[] args)
		{
			float result = 0;
	        float x = (float) args[0];
	        for (int i = 1; i < args.Length; i++)
	        {
	            float pi = (float) args[i];
	            result += pi * (float) Math.Pow(x, i);
	        }
	        return result;
		}
		
		/// <summary>
		/// 判断两个时间是不是同一天
		/// </summary>
		public static bool IsSameDay(int sec1, int sec2)
		{
            DateTime start = new DateTime(1970, 1, 1);
            DateTime dt1 = start.AddSeconds(sec1);
            DateTime dt2 = start.AddSeconds(sec2);

            return dt1.ToLongDateString() == dt2.ToLongDateString();
		}
		

	
		

		
		/// <summary>
		/// 根据服务器的时间计算本地时间
		/// </summary>
		public static DateTime GetServerTime(int serverTime)
		{
			DateTime start = DateTime.Parse("1970-1-1");
			start = start.AddHours(8f);
			
			DateTime now = start.AddSeconds(serverTime);
			return now;
		}
		

		
		/// <summary>
		/// 毫秒值
		/// </summary>
		private static DateTime _start = DateTime.Now;
		public static int Tick
		{
			get
			{
		        TimeSpan span = DateTime.Now - _start;
		        return (int) span.TotalMilliseconds;
			}
	    }

		
		
//      需要的函数在加进来
//	    public static string GetRidByDomainAddress(string domain_address)
//	    {
//	        // domain_address : rid@xxxx
//	        // domain_address : c@rid.xxx
//	        int start_pos, pos;
//	        if (domain_address.StartsWith("c@"))
//	        {
//	            start_pos = "c@".Length;
//	            pos = domain_address.IndexOf("#");
//	            if (pos < 0)
//	                pos = domain_address.IndexOf(".");
//	        }
//	        else
//	        {
//	            start_pos = 0;
//	            pos = domain_address.IndexOf('@');
//	        }
//	
//	        if (pos < 0)
//	            throw new Exception(string.Format("bad domain_address format, require '@': {0}", domain_address));
//	
//	        return domain_address.Substring(start_pos, pos - start_pos);
//	    }
//	
//	    public static int GetBaseInstanceNo(int rno)
//	    {
//	        // 34023-->34000
//	        return rno / 1000 * 1000;
//	    }
//	
//	    public static int GetBaseInstanceIndex(int rno)
//	    {
//	        // 34023-->23
//	        return rno % 1000;
//	    }
//	
//	    public static string GetInstanceTerrain(int rno)
//	    {
//	        int base_no = GetBaseInstanceNo(rno);
//	        return string.Format("map_{0}.terrain", base_no);
//	    }
//	
//	    public static float Fxn(params object[] args)
//	    {
//	        float result = 0;
//	        float x = (float) args[0];
//	        for (int i = 1; i < args.Length; i++)
//	        {
//	            float pi = (float) args[i];
//	            result += pi * (float) Math.Pow(x, i);
//	        }
//	        return result;
//	    }
//	
//	
//	    public static int GetCurrentTime()
//	    {
//	        TimeSpan span = DateTime.Now - new DateTime(1970, 1, 1);
//	        return (int) span.TotalSeconds;
//	    }
//	
//	    public static string GetInstanceResourceByRno(int instance_rno)
//	    {
//	        int base_rno  = GameUtil.GetBaseInstanceNo(instance_rno);
//	        string[] field_list = new string[] { "worldmap", };
//	        SQLiteRow row = DBLoader.UIDatabase.query_one_row("rno2instance", field_list,
//	                                                          "exit_rno/1000*1000=?", base_rno);
//	        if (row == null || row.empty)
//	            return string.Empty;
//	
//	        Dictionary<string, string> data = row.convert_raw();
//	        return data["worldmap"];
//	    }
//	
//	    public static string GetInstanceNameByWorldMap(string worldmap_name, int game_mode)
//	    {
//	        string[] field_list = new string[] { "instance", };
//	        SQLiteRow row = DBLoader.UIDatabase.query_one_row("rno2instance", field_list,
//	                                                            "worldmap=? AND game_mode=?", worldmap_name, game_mode);
//	        if (row == null || row.empty)
//	            return string.Empty;
//	
//	        Dictionary<string, string> data = row.convert_raw();
//	        return data["instance"];
//	    }
//	
//	    public static int GetInstanceExitRnoByWorldMap(string worldmap_name, int game_mode)
//	    {
//	        string[] field_list = new string[] { "exit_rno", };
//	        SQLiteRow row = DBLoader.UIDatabase.query_one_row("rno2instance", field_list,
//	                                                            "worldmap=? AND game_mode=?", worldmap_name, game_mode);
//	        if (row == null || row.empty)
//	            return -1;
//	
//	        Dictionary<string, string> data = row.convert_raw();
//	        return GameUtil.atoi(data["exit_rno"]);
//	    }
//	
//	    public static string GetWorldMapByExitRno(int exit_rno)
//	    {
//	        string[] field_list = new string[] { "worldmap", };
//	        SQLiteRow row = DBLoader.UIDatabase.query_one_row("rno2instance", field_list,
//	                                                            "exit_rno=?", exit_rno);
//	        if (row == null || row.empty)
//	            return string.Empty;
//	
//	        Dictionary<string, string> data = row.convert_raw();
//	        return data["worldmap"];
//	    }
//	
//	    public static int GetInstanceRnoByName(string instance_name)
//	    {
//	        string[] field_list = new string[] { "rno", "instance", };
//	        SQLiteRow row = DBLoader.UIDatabase.query_one_row("rno2instance", field_list,
//	                                                            "instance=?", instance_name);
//	        if (row == null || row.empty)
//	            return -1;
//	
//	        Dictionary<string, string> data = row.convert_raw();
//	        return Convert.ToInt32(data["rno"]);
//	    }
//		
//		public static string GetInstanceLevelStrByWorldMap(string worldmap_name, int game_mode)
//		{
//	        string[] field_list = new string[] { "level", };
//	        SQLiteRow row = DBLoader.UIDatabase.query_one_row("rno2instance", field_list,
//	                                                            "worldmap=? AND game_mode=?", worldmap_name, game_mode);
//	        if (row == null || row.empty)
//	            return string.Empty;
//	
//	        Dictionary<string, string> data = row.convert_raw();
//	        return data["level"];
//		}
//	
//	    public static string GetRoomPrefabByRno(int rno)
//	    {
//	        string[] field_list = new string[] { "rno", "room_prefab", };
//	        SQLiteRow row = DBLoader.UIDatabase.query_one_row("room_resource", field_list,
//	                                                            "rno=?", rno);
//	        if (row == null || row.empty)
//	            return string.Empty;
//	
//	        Dictionary<string, string> data = row.convert_raw();
//	        return data["room_prefab"];
//	    }
//	
//	    public static int GetMainCityRno()
//	    {
//	        return 980;
//	    }
//	
//	    public static bool IsMainCityRno(int rno)
//	    {
//	        return rno == GetMainCityRno();
//	    }
//	
//	    public static bool IsMainCityRno(string mapName)
//	    {
//	        return mapName.ToLower() == "hell";
//	    }
//	
//	
//	    public static bool ContainBit(int bits_value, int bit_value)
//	    {
//	        return (bits_value & bit_value) == bit_value;
//	    }
//		
	}
