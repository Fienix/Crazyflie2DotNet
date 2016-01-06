namespace Crazyflie2DotNet.Crazyflie.TransferProtocol
{
	public interface IOutputPacketHeader
		: IPacketHeader
	{
		CommunicationPort Port { get; }
		CommunicationChannel Channel { get; }
	}
}