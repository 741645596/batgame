//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: chat.proto
namespace chat
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ChatRequest")]
  public partial class ChatRequest : global::ProtoBuf.IExtensible
  {
    public ChatRequest() {}
    
    private string _chat;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"chat", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string chat
    {
      get { return _chat; }
      set { _chat = value; }
    }
    private int _channel;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"channel", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int channel
    {
      get { return _channel; }
      set { _channel = value; }
    }
    private int _touid = default(int);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"touid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int touid
    {
      get { return _touid; }
      set { _touid = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ChatResponse")]
  public partial class ChatResponse : global::ProtoBuf.IExtensible
  {
    public ChatResponse() {}
    
    private int _result;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"result", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int result
    {
      get { return _result; }
      set { _result = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ChatNotify")]
  public partial class ChatNotify : global::ProtoBuf.IExtensible
  {
    public ChatNotify() {}
    
    private int _channel;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"channel", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int channel
    {
      get { return _channel; }
      set { _channel = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ChatPullRequest")]
  public partial class ChatPullRequest : global::ProtoBuf.IExtensible
  {
    public ChatPullRequest() {}
    
    private int _channel;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"channel", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int channel
    {
      get { return _channel; }
      set { _channel = value; }
    }
    private int _id;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int id
    {
      get { return _id; }
      set { _id = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ChatMessage")]
  public partial class ChatMessage : global::ProtoBuf.IExtensible
  {
    public ChatMessage() {}
    
    private string _from_name;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"from_name", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string from_name
    {
      get { return _from_name; }
      set { _from_name = value; }
    }
    private int _from_uid;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"from_uid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int from_uid
    {
      get { return _from_uid; }
      set { _from_uid = value; }
    }
    private int _channel;
    [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name=@"channel", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int channel
    {
      get { return _channel; }
      set { _channel = value; }
    }
    private string _chat;
    [global::ProtoBuf.ProtoMember(4, IsRequired = true, Name=@"chat", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string chat
    {
      get { return _chat; }
      set { _chat = value; }
    }
    private int _time;
    [global::ProtoBuf.ProtoMember(5, IsRequired = true, Name=@"time", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int time
    {
      get { return _time; }
      set { _time = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ChatPullResponse")]
  public partial class ChatPullResponse : global::ProtoBuf.IExtensible
  {
    public ChatPullResponse() {}
    
    private readonly global::System.Collections.Generic.List<chat.ChatMessage> _msg = new global::System.Collections.Generic.List<chat.ChatMessage>();
    [global::ProtoBuf.ProtoMember(1, Name=@"msg", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<chat.ChatMessage> msg
    {
      get { return _msg; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ChatPmRequest")]
  public partial class ChatPmRequest : global::ProtoBuf.IExtensible
  {
    public ChatPmRequest() {}
    
    private string _pm_cmd;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"pm_cmd", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string pm_cmd
    {
      get { return _pm_cmd; }
      set { _pm_cmd = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ChatPmResponse")]
  public partial class ChatPmResponse : global::ProtoBuf.IExtensible
  {
    public ChatPmResponse() {}
    
    private bool _result;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"result", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public bool result
    {
      get { return _result; }
      set { _result = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}