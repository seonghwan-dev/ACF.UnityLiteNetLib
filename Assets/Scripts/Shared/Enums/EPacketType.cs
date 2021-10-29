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
		MoveToRequest,
	}

	public enum EResponseType : int
	{
		MoveToResponse,
	}

	public enum EBroadcastType : int
	{
		ChatBroadcast,
		
		BEGIN_MOVE,
		MOVING,
		END_MOVE,
		
		SpawnCharacter,
		DestroyCharacter,
	}

	public enum EDieReason : int
	{
		Undefined,
		Disconnected,
	}
}