namespace Crazyflie2DotNet.Crazyflie.TransferProtocol
{
	public interface ICommanderPacketPayload
		: IOutputPacketPayload
	{
		float Roll { get; }
		float Pitch { get; }
		float Yaw { get; }
		ushort Thrust { get; }
	}
}