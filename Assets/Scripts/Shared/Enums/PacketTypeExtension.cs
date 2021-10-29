namespace Game.Shared.Enums
{
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