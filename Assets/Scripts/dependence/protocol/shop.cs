//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: shop.proto
namespace shop
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ShopInfo")]
  public partial class ShopInfo : global::ProtoBuf.IExtensible
  {
    public ShopInfo() {}
    
    private int _next_auto_refresh = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"next_auto_refresh", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int next_auto_refresh
    {
      get { return _next_auto_refresh; }
      set { _next_auto_refresh = value; }
    }
    private int _manual_refresh_times = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"manual_refresh_times", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int manual_refresh_times
    {
      get { return _manual_refresh_times; }
      set { _manual_refresh_times = value; }
    }
    private readonly global::System.Collections.Generic.List<shop.ShopInfo.WareInfo> _ware_info = new global::System.Collections.Generic.List<shop.ShopInfo.WareInfo>();
    [global::ProtoBuf.ProtoMember(3, Name=@"ware_info", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<shop.ShopInfo.WareInfo> ware_info
    {
      get { return _ware_info; }
    }
  
    private int _shop_type = default(int);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"shop_type", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int shop_type
    {
      get { return _shop_type; }
      set { _shop_type = value; }
    }
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"WareInfo")]
  public partial class WareInfo : global::ProtoBuf.IExtensible
  {
    public WareInfo() {}
    
    private int _ware_id = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"ware_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int ware_id
    {
      get { return _ware_id; }
      set { _ware_id = value; }
    }
    private int _ware_status = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"ware_status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int ware_status
    {
      get { return _ware_status; }
      set { _ware_status = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ShopInfoRequest")]
  public partial class ShopInfoRequest : global::ProtoBuf.IExtensible
  {
    public ShopInfoRequest() {}
    
    private readonly global::System.Collections.Generic.List<int> _shop_types = new global::System.Collections.Generic.List<int>();
    [global::ProtoBuf.ProtoMember(1, Name=@"shop_types", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public global::System.Collections.Generic.List<int> shop_types
    {
      get { return _shop_types; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ShopInfoResponse")]
  public partial class ShopInfoResponse : global::ProtoBuf.IExtensible
  {
    public ShopInfoResponse() {}
    
    private readonly global::System.Collections.Generic.List<shop.ShopInfo> _shop_infos = new global::System.Collections.Generic.List<shop.ShopInfo>();
    [global::ProtoBuf.ProtoMember(1, Name=@"shop_infos", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<shop.ShopInfo> shop_infos
    {
      get { return _shop_infos; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ShopBuyRequest")]
  public partial class ShopBuyRequest : global::ProtoBuf.IExtensible
  {
    public ShopBuyRequest() {}
    
    private int _shop_type;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"shop_type", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int shop_type
    {
      get { return _shop_type; }
      set { _shop_type = value; }
    }
    private int _ware_id;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"ware_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int ware_id
    {
      get { return _ware_id; }
      set { _ware_id = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ShopBuyResponse")]
  public partial class ShopBuyResponse : global::ProtoBuf.IExtensible
  {
    public ShopBuyResponse() {}
    
    private shop.ShopInfo _shop_info = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"shop_info", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public shop.ShopInfo shop_info
    {
      get { return _shop_info; }
      set { _shop_info = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ShopRefreshRequest")]
  public partial class ShopRefreshRequest : global::ProtoBuf.IExtensible
  {
    public ShopRefreshRequest() {}
    
    private int _shop_type;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"shop_type", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int shop_type
    {
      get { return _shop_type; }
      set { _shop_type = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ShopRefreshResponse")]
  public partial class ShopRefreshResponse : global::ProtoBuf.IExtensible
  {
    public ShopRefreshResponse() {}
    
    private shop.ShopInfo _shop_info = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"shop_info", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public shop.ShopInfo shop_info
    {
      get { return _shop_info; }
      set { _shop_info = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}