namespace Game.Shared.Enums
{
	public enum EPacketType : short
	{
		HANDSHAKE,
		REQUEST,
		RESPONSE,
		BROADCAST,
	}

	public enum ERequestType : int
	{
		SendChatRequest,
	}

	public enum EResponseType : int
	{
		
	}

	public enum EBroadcastType : int
	{
		ChatBroadcast
	}

	public static class PacketTypeExtension
	{
		public static short ToShort(this EPacketType packetType)
		{
			return (short)packetType;
		}
		
		public static int ToInt(this EBroadcastType packetType)
		{
			return (int)packetType;
		}
		
		public static int ToInt(this ERequestType packetType)
		{
			return (int)packetType;
		}
		
		public static int ToInt(this EResponseType packetType)
		{
			return (int)packetType;
		}
	}
}