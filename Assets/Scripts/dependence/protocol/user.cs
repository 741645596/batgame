//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: user.proto
namespace user
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"UserInfoRequest")]
  public partial class UserInfoRequest : global::ProtoBuf.IExtensible
  {
    public UserInfoRequest() {}
    
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"UserInfoResponse")]
  public partial class UserInfoResponse : global::ProtoBuf.IExtensible
  {
    public UserInfoResponse() {}
    
    private int _action = (int)0;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"action", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)0)]
    public int action
    {
      get { return _action; }
      set { _action = value; }
    }
    private string _name = @"null";
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"name", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(@"null")]
    public string name
    {
      get { return _name; }
      set { _name = value; }
    }
    private int _level = (int)-1;
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"level", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int level
    {
      get { return _level; }
      set { _level = value; }
    }
    private int _exp = (int)-1;
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"exp", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int exp
    {
      get { return _exp; }
      set { _exp = value; }
    }
    private int _coin = (int)-1;
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"coin", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int coin
    {
      get { return _coin; }
      set { _coin = value; }
    }
    private int _wood = (int)-1;
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"wood", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int wood
    {
      get { return _wood; }
      set { _wood = value; }
    }
    private int _stone = (int)-1;
    [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name=@"stone", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int stone
    {
      get { return _stone; }
      set { _stone = value; }
    }
    private int _steel = (int)-1;
    [global::ProtoBuf.ProtoMember(8, IsRequired = false, Name=@"steel", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int steel
    {
      get { return _steel; }
      set { _steel = value; }
    }
    private int _diamond = (int)-1;
    [global::ProtoBuf.ProtoMember(9, IsRequired = false, Name=@"diamond", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int diamond
    {
      get { return _diamond; }
      set { _diamond = value; }
    }
    private int _hdiamond = (int)-1;
    [global::ProtoBuf.ProtoMember(10, IsRequired = false, Name=@"hdiamond", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int hdiamond
    {
      get { return _hdiamond; }
      set { _hdiamond = value; }
    }
    private int _athletics_money = (int)-1;
    [global::ProtoBuf.ProtoMember(11, IsRequired = false, Name=@"athletics_money", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int athletics_money
    {
      get { return _athletics_money; }
      set { _athletics_money = value; }
    }
    private int _login_time = (int)-1;
    [global::ProtoBuf.ProtoMember(12, IsRequired = false, Name=@"login_time", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int login_time
    {
      get { return _login_time; }
      set { _login_time = value; }
    }
    private int _sociatyid = (int)-1;
    [global::ProtoBuf.ProtoMember(13, IsRequired = false, Name=@"sociatyid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int sociatyid
    {
      get { return _sociatyid; }
      set { _sociatyid = value; }
    }
    private int _physical = (int)-1;
    [global::ProtoBuf.ProtoMember(14, IsRequired = false, Name=@"physical", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int physical
    {
      get { return _physical; }
      set { _physical = value; }
    }
    private int _logout_time = (int)-1;
    [global::ProtoBuf.ProtoMember(15, IsRequired = false, Name=@"logout_time", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int logout_time
    {
      get { return _logout_time; }
      set { _logout_time = value; }
    }
    private string _gate = @"null";
    [global::ProtoBuf.ProtoMember(16, IsRequired = false, Name=@"gate", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(@"null")]
    public string gate
    {
      get { return _gate; }
      set { _gate = value; }
    }
    private int _chattime = (int)-1;
    [global::ProtoBuf.ProtoMember(17, IsRequired = false, Name=@"chattime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int chattime
    {
      get { return _chattime; }
      set { _chattime = value; }
    }
    private int _headid = (int)-1;
    [global::ProtoBuf.ProtoMember(18, IsRequired = false, Name=@"headid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int headid
    {
      get { return _headid; }
      set { _headid = value; }
    }
    private int _headframeid = (int)-1;
    [global::ProtoBuf.ProtoMember(19, IsRequired = false, Name=@"headframeid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int headframeid
    {
      get { return _headframeid; }
      set { _headframeid = value; }
    }
    private int _uid = (int)-1;
    [global::ProtoBuf.ProtoMember(20, IsRequired = false, Name=@"uid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int uid
    {
      get { return _uid; }
      set { _uid = value; }
    }
    private int _decklevel = (int)-1;
    [global::ProtoBuf.ProtoMember(21, IsRequired = false, Name=@"decklevel", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int decklevel
    {
      get { return _decklevel; }
      set { _decklevel = value; }
    }
    private int _crystal = (int)-1;
    [global::ProtoBuf.ProtoMember(22, IsRequired = false, Name=@"crystal", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int crystal
    {
      get { return _crystal; }
      set { _crystal = value; }
    }
    private int _pirate_ref_time = (int)-1;
    [global::ProtoBuf.ProtoMember(23, IsRequired = false, Name=@"pirate_ref_time", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int pirate_ref_time
    {
      get { return _pirate_ref_time; }
      set { _pirate_ref_time = value; }
    }
    private int _skillpoint = (int)-1;
    [global::ProtoBuf.ProtoMember(24, IsRequired = false, Name=@"skillpoint", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int skillpoint
    {
      get { return _skillpoint; }
      set { _skillpoint = value; }
    }
    private int _skillpoint_time = (int)-1;
    [global::ProtoBuf.ProtoMember(25, IsRequired = false, Name=@"skillpoint_time", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int skillpoint_time
    {
      get { return _skillpoint_time; }
      set { _skillpoint_time = value; }
    }
    private int _physical_time = (int)-1;
    [global::ProtoBuf.ProtoMember(26, IsRequired = false, Name=@"physical_time", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int physical_time
    {
      get { return _physical_time; }
      set { _physical_time = value; }
    }
    private long _deblocking_flag = (long)-1;
    [global::ProtoBuf.ProtoMember(27, IsRequired = false, Name=@"deblocking_flag", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((long)-1)]
    public long deblocking_flag
    {
      get { return _deblocking_flag; }
      set { _deblocking_flag = value; }
    }
    private int _erniecoin_flag = (int)-1;
    [global::ProtoBuf.ProtoMember(28, IsRequired = false, Name=@"erniecoin_flag", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int erniecoin_flag
    {
      get { return _erniecoin_flag; }
      set { _erniecoin_flag = value; }
    }
    private int _erniediamond_flag = (int)-1;
    [global::ProtoBuf.ProtoMember(29, IsRequired = false, Name=@"erniediamond_flag", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int erniediamond_flag
    {
      get { return _erniediamond_flag; }
      set { _erniediamond_flag = value; }
    }
    private int _gnomeshop_time = (int)-1;
    [global::ProtoBuf.ProtoMember(30, IsRequired = false, Name=@"gnomeshop_time", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int gnomeshop_time
    {
      get { return _gnomeshop_time; }
      set { _gnomeshop_time = value; }
    }
    private int _blackshop_time = (int)-1;
    [global::ProtoBuf.ProtoMember(31, IsRequired = false, Name=@"blackshop_time", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int blackshop_time
    {
      get { return _blackshop_time; }
      set { _blackshop_time = value; }
    }
    private int _signin_times = (int)-1;
    [global::ProtoBuf.ProtoMember(32, IsRequired = false, Name=@"signin_times", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int signin_times
    {
      get { return _signin_times; }
      set { _signin_times = value; }
    }
    private int _lastsignin_time = (int)-1;
    [global::ProtoBuf.ProtoMember(33, IsRequired = false, Name=@"lastsignin_time", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int lastsignin_time
    {
      get { return _lastsignin_time; }
      set { _lastsignin_time = value; }
    }
    private int _headchartid = (int)-1;
    [global::ProtoBuf.ProtoMember(34, IsRequired = false, Name=@"headchartid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int headchartid
    {
      get { return _headchartid; }
      set { _headchartid = value; }
    }
    private int _vip_level = (int)-1;
    [global::ProtoBuf.ProtoMember(35, IsRequired = false, Name=@"vip_level", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int vip_level
    {
      get { return _vip_level; }
      set { _vip_level = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"UserTimesRequest")]
  public partial class UserTimesRequest : global::ProtoBuf.IExtensible
  {
    public UserTimesRequest() {}
    
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"UserTimesResponse")]
  public partial class UserTimesResponse : global::ProtoBuf.IExtensible
  {
    public UserTimesResponse() {}
    
    private int _action = (int)0;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"action", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)0)]
    public int action
    {
      get { return _action; }
      set { _action = value; }
    }
    private int _uid = (int)-1;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"uid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int uid
    {
      get { return _uid; }
      set { _uid = value; }
    }
    private int _skilltimes = (int)-1;
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"skilltimes", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int skilltimes
    {
      get { return _skilltimes; }
      set { _skilltimes = value; }
    }
    private int _treasuretimes = (int)-1;
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"treasuretimes", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int treasuretimes
    {
      get { return _treasuretimes; }
      set { _treasuretimes = value; }
    }
    private int _shop_normal_times = (int)-1;
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"shop_normal_times", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int shop_normal_times
    {
      get { return _shop_normal_times; }
      set { _shop_normal_times = value; }
    }
    private int _athletics_times = (int)-1;
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"athletics_times", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int athletics_times
    {
      get { return _athletics_times; }
      set { _athletics_times = value; }
    }
    private int _athletics_buy_times = (int)-1;
    [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name=@"athletics_buy_times", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int athletics_buy_times
    {
      get { return _athletics_buy_times; }
      set { _athletics_buy_times = value; }
    }
    private int _ernie_coin_times = (int)-1;
    [global::ProtoBuf.ProtoMember(8, IsRequired = false, Name=@"ernie_coin_times", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int ernie_coin_times
    {
      get { return _ernie_coin_times; }
      set { _ernie_coin_times = value; }
    }
    private int _ernie_diamond_times = (int)-1;
    [global::ProtoBuf.ProtoMember(9, IsRequired = false, Name=@"ernie_diamond_times", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int ernie_diamond_times
    {
      get { return _ernie_diamond_times; }
      set { _ernie_diamond_times = value; }
    }
    private int _buy_coin_times = (int)-1;
    [global::ProtoBuf.ProtoMember(10, IsRequired = false, Name=@"buy_coin_times", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int buy_coin_times
    {
      get { return _buy_coin_times; }
      set { _buy_coin_times = value; }
    }
    private int _buy_wood_times = (int)-1;
    [global::ProtoBuf.ProtoMember(11, IsRequired = false, Name=@"buy_wood_times", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int buy_wood_times
    {
      get { return _buy_wood_times; }
      set { _buy_wood_times = value; }
    }
    private int _buy_crystal_times = (int)-1;
    [global::ProtoBuf.ProtoMember(12, IsRequired = false, Name=@"buy_crystal_times", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int buy_crystal_times
    {
      get { return _buy_crystal_times; }
      set { _buy_crystal_times = value; }
    }
    private int _shop_athletics_times = (int)-1;
    [global::ProtoBuf.ProtoMember(13, IsRequired = false, Name=@"shop_athletics_times", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int shop_athletics_times
    {
      get { return _shop_athletics_times; }
      set { _shop_athletics_times = value; }
    }
    private int _shop_gnome_times = (int)-1;
    [global::ProtoBuf.ProtoMember(14, IsRequired = false, Name=@"shop_gnome_times", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int shop_gnome_times
    {
      get { return _shop_gnome_times; }
      set { _shop_gnome_times = value; }
    }
    private int _shop_black_times = (int)-1;
    [global::ProtoBuf.ProtoMember(15, IsRequired = false, Name=@"shop_black_times", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int shop_black_times
    {
      get { return _shop_black_times; }
      set { _shop_black_times = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"RoleNameRequest")]
  public partial class RoleNameRequest : global::ProtoBuf.IExtensible
  {
    public RoleNameRequest() {}
    
    private string _name;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"name", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string name
    {
      get { return _name; }
      set { _name = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"RoleNameResponse")]
  public partial class RoleNameResponse : global::ProtoBuf.IExtensible
  {
    public RoleNameResponse() {}
    
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"HeadModifyRequest")]
  public partial class HeadModifyRequest : global::ProtoBuf.IExtensible
  {
    public HeadModifyRequest() {}
    
    private int _headid = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"headid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int headid
    {
      get { return _headid; }
      set { _headid = value; }
    }
    private int _headframeid = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"headframeid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int headframeid
    {
      get { return _headframeid; }
      set { _headframeid = value; }
    }
    private int _headchartid = default(int);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"headchartid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int headchartid
    {
      get { return _headchartid; }
      set { _headchartid = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"HeadModifyResponse")]
  public partial class HeadModifyResponse : global::ProtoBuf.IExtensible
  {
    public HeadModifyResponse() {}
    
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"CheckDeviceRequest")]
  public partial class CheckDeviceRequest : global::ProtoBuf.IExtensible
  {
    public CheckDeviceRequest() {}
    
    private string _id;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"id", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string id
    {
      get { return _id; }
      set { _id = value; }
    }
    private int _device_type;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"device_type", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int device_type
    {
      get { return _device_type; }
      set { _device_type = value; }
    }
    private string _account_name = "";
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"account_name", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string account_name
    {
      get { return _account_name; }
      set { _account_name = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"CheckDeviceResponse")]
  public partial class CheckDeviceResponse : global::ProtoBuf.IExtensible
  {
    public CheckDeviceResponse() {}
    
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"KickResponse")]
  public partial class KickResponse : global::ProtoBuf.IExtensible
  {
    public KickResponse() {}
    
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"BuyPhysicalRequest")]
  public partial class BuyPhysicalRequest : global::ProtoBuf.IExtensible
  {
    public BuyPhysicalRequest() {}
    
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"BuyPhysicalResponse")]
  public partial class BuyPhysicalResponse : global::ProtoBuf.IExtensible
  {
    public BuyPhysicalResponse() {}
    
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"BuySkillPointRequest")]
  public partial class BuySkillPointRequest : global::ProtoBuf.IExtensible
  {
    public BuySkillPointRequest() {}
    
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"BuySkillPointResponse")]
  public partial class BuySkillPointResponse : global::ProtoBuf.IExtensible
  {
    public BuySkillPointResponse() {}
    
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"EMoneyBuyResourceRequest")]
  public partial class EMoneyBuyResourceRequest : global::ProtoBuf.IExtensible
  {
    public EMoneyBuyResourceRequest() {}
    
    private int _res_type;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"res_type", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int res_type
    {
      get { return _res_type; }
      set { _res_type = value; }
    }
    private bool _istenhit = (bool)false;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"istenhit", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue((bool)false)]
    public bool istenhit
    {
      get { return _istenhit; }
      set { _istenhit = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"EMoneyBuyResourceResponse")]
  public partial class EMoneyBuyResourceResponse : global::ProtoBuf.IExtensible
  {
    public EMoneyBuyResourceResponse() {}
    
    private int _res_type;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"res_type", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int res_type
    {
      get { return _res_type; }
      set { _res_type = value; }
    }
    private readonly global::System.Collections.Generic.List<int> _crit_values = new global::System.Collections.Generic.List<int>();
    [global::ProtoBuf.ProtoMember(2, Name=@"crit_values", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public global::System.Collections.Generic.List<int> crit_values
    {
      get { return _crit_values; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"UserSigninRequest")]
  public partial class UserSigninRequest : global::ProtoBuf.IExtensible
  {
    public UserSigninRequest() {}
    
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"UserSigninResponse")]
  public partial class UserSigninResponse : global::ProtoBuf.IExtensible
  {
    public UserSigninResponse() {}
    
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"StatusInfoRequest")]
  public partial class StatusInfoRequest : global::ProtoBuf.IExtensible
  {
    public StatusInfoRequest() {}
    
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"StatusInfoResponse")]
  public partial class StatusInfoResponse : global::ProtoBuf.IExtensible
  {
    public StatusInfoResponse() {}
    
    private readonly global::System.Collections.Generic.List<user.StatusInfoResponse.StatusInfo> _infos = new global::System.Collections.Generic.List<user.StatusInfoResponse.StatusInfo>();
    [global::ProtoBuf.ProtoMember(1, Name=@"infos", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<user.StatusInfoResponse.StatusInfo> infos
    {
      get { return _infos; }
    }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"StatusInfo")]
  public partial class StatusInfo : global::ProtoBuf.IExtensible
  {
    public StatusInfo() {}
    
    private int _action = (int)0;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"action", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)0)]
    public int action
    {
      get { return _action; }
      set { _action = value; }
    }
    private int _id = (int)-1;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int id
    {
      get { return _id; }
      set { _id = value; }
    }
    private int _uid = (int)-1;
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"uid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int uid
    {
      get { return _uid; }
      set { _uid = value; }
    }
    private int _status = (int)-1;
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int status
    {
      get { return _status; }
      set { _status = value; }
    }
    private long _power = (long)-1;
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"power", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((long)-1)]
    public long power
    {
      get { return _power; }
      set { _power = value; }
    }
    private long _param = (long)-1;
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"param", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((long)-1)]
    public long param
    {
      get { return _param; }
      set { _param = value; }
    }
    private long _end_time = (long)-1;
    [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name=@"end_time", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((long)-1)]
    public long end_time
    {
      get { return _end_time; }
      set { _end_time = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}