//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: captain.proto
namespace captain
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"CaptainInfoRequest")]
  public partial class CaptainInfoRequest : global::ProtoBuf.IExtensible
  {
    public CaptainInfoRequest() {}
    
    private readonly global::System.Collections.Generic.List<int> _id = new global::System.Collections.Generic.List<int>();
    [global::ProtoBuf.ProtoMember(1, Name=@"id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public global::System.Collections.Generic.List<int> id
    {
      get { return _id; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"CaptainInfo")]
  public partial class CaptainInfo : global::ProtoBuf.IExtensible
  {
    public CaptainInfo() {}
    
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
    private int _id = (int)-1;
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int id
    {
      get { return _id; }
      set { _id = value; }
    }
    private int _captainid = (int)-1;
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"captainid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int captainid
    {
      get { return _captainid; }
      set { _captainid = value; }
    }
    private int _godskilltype1 = (int)-1;
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"godskilltype1", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int godskilltype1
    {
      get { return _godskilltype1; }
      set { _godskilltype1 = value; }
    }
    private int _level1 = (int)-1;
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"level1", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int level1
    {
      get { return _level1; }
      set { _level1 = value; }
    }
    private int _godskilltype2 = (int)-1;
    [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name=@"godskilltype2", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int godskilltype2
    {
      get { return _godskilltype2; }
      set { _godskilltype2 = value; }
    }
    private int _level2 = (int)-1;
    [global::ProtoBuf.ProtoMember(8, IsRequired = false, Name=@"level2", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int level2
    {
      get { return _level2; }
      set { _level2 = value; }
    }
    private int _godskilltype3 = (int)-1;
    [global::ProtoBuf.ProtoMember(9, IsRequired = false, Name=@"godskilltype3", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int godskilltype3
    {
      get { return _godskilltype3; }
      set { _godskilltype3 = value; }
    }
    private int _level3 = (int)-1;
    [global::ProtoBuf.ProtoMember(10, IsRequired = false, Name=@"level3", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int level3
    {
      get { return _level3; }
      set { _level3 = value; }
    }
    private int _godskilltype4 = (int)-1;
    [global::ProtoBuf.ProtoMember(11, IsRequired = false, Name=@"godskilltype4", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int godskilltype4
    {
      get { return _godskilltype4; }
      set { _godskilltype4 = value; }
    }
    private int _level4 = (int)-1;
    [global::ProtoBuf.ProtoMember(12, IsRequired = false, Name=@"level4", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int level4
    {
      get { return _level4; }
      set { _level4 = value; }
    }
    private int _star = (int)-1;
    [global::ProtoBuf.ProtoMember(13, IsRequired = false, Name=@"star", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)-1)]
    public int star
    {
      get { return _star; }
      set { _star = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"CaptainInfoResponse")]
  public partial class CaptainInfoResponse : global::ProtoBuf.IExtensible
  {
    public CaptainInfoResponse() {}
    
    private readonly global::System.Collections.Generic.List<captain.CaptainInfo> _captain_infos = new global::System.Collections.Generic.List<captain.CaptainInfo>();
    [global::ProtoBuf.ProtoMember(1, Name=@"captain_infos", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<captain.CaptainInfo> captain_infos
    {
      get { return _captain_infos; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"CaptainLevelupRequest")]
  public partial class CaptainLevelupRequest : global::ProtoBuf.IExtensible
  {
    public CaptainLevelupRequest() {}
    
    private int _dcaptainid;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"dcaptainid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int dcaptainid
    {
      get { return _dcaptainid; }
      set { _dcaptainid = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"CaptainLevelupResponse")]
  public partial class CaptainLevelupResponse : global::ProtoBuf.IExtensible
  {
    public CaptainLevelupResponse() {}
    
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"CaptainStarupRequest")]
  public partial class CaptainStarupRequest : global::ProtoBuf.IExtensible
  {
    public CaptainStarupRequest() {}
    
    private int _dcaptainid;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"dcaptainid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int dcaptainid
    {
      get { return _dcaptainid; }
      set { _dcaptainid = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"CaptainStarupResponse")]
  public partial class CaptainStarupResponse : global::ProtoBuf.IExtensible
  {
    public CaptainStarupResponse() {}
    
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"CaptainActivateRequest")]
  public partial class CaptainActivateRequest : global::ProtoBuf.IExtensible
  {
    public CaptainActivateRequest() {}
    
    private int _scaptainid;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"scaptainid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int scaptainid
    {
      get { return _scaptainid; }
      set { _scaptainid = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"CaptainActivateResponse")]
  public partial class CaptainActivateResponse : global::ProtoBuf.IExtensible
  {
    public CaptainActivateResponse() {}
    
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}