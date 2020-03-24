using ProtoBuf;
using battle;
using chat;
using gate;
using soldier;
using user;
using System.IO;
using System;

class protobufM 
{
	public static byte[] SerializerProtobuf(int msgno, string name, byte[] networkmsg)
	{
		GateMessage gm = new GateMessage ();

        gm.head = new Header();

		gm.head.uid = UserDC.GetUserID();

		gm.head.client_version = ConstantData.g_version;

		gm.head.internal_version = ConstantData.g_Ndversion;

        GateMessage.MessageContent msg = new GateMessage.MessageContent();
        msg.cmd = msgno;

        msg.proto_name = name;


        msg.network_message = networkmsg;

        gm.content.Add(msg);

		MemoryStream gateStream = new MemoryStream ();

		ProtoBuf.Serializer.Serialize<GateMessage> (gateStream,gm);

		return gateStream.ToArray();
	}

	public static byte[] Serializerobject<T>(T obj)
	{
		//dump
		FileLog.ProtocolDump(obj);
		//end dump
		MemoryStream networkStream = new MemoryStream();
		
		ProtoBuf.Serializer.Serialize<T> (networkStream,obj);

		return networkStream.ToArray ();
	}
	
	public static object Deserialize(string name,byte[] data)
	{
		Type t = Type.GetType (name);

		MemoryStream ms = new MemoryStream(data);

        ms.SetLength(data.Length );
	
		return ProtoBuf.Meta.RuntimeTypeModel.Default.Deserialize(ms, null , t);
	}
}