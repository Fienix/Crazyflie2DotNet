namespace Crazyflie2DotNet.Crazyflie.TransferProtocol
{
	public interface ICommanderPacket
		: IOutputPacket<ICommanderPacketHeader, ICommanderPacketPayload>
	{
	}
}