using System;
using System.Linq;

namespace Crazyflie2DotNet.Crazyflie.TransferProtocol
{
	public class CommanderPacket
		: OutputPacket<ICommanderPacketHeader, ICommanderPacketPayload>, ICommanderPacket
	{
		public CommanderPacket(byte[] packetBytes)
			: base(packetBytes)
		{
		}

		public CommanderPacket(ICommanderPacketHeader header, ICommanderPacketPayload payload)
			: base(header, payload)
		{
		}

		public CommanderPacket(float roll, float pitch, float yaw, ushort thrust, CommunicationChannel channel = OutputPacketHeader.DefaultChannel)
			: this(new CommanderPacketHeader(channel), new CommanderPacketPayload(roll, pitch, yaw, thrust))
		{
		}

		protected override ICommanderPacketHeader ParseHeader(byte[] packetBytes)
		{
			if (packetBytes != null && packetBytes.Length != 0)
			{
				var packetHeader = new CommanderPacketHeader(packetBytes[0]);
				return packetHeader;
			}

			return null;
		}

        protected override ICommanderPacketPayload ParsePayload(byte[] packetBytes)
		{
			if (packetBytes != null && packetBytes.Length != 0)
			{
				var packetPayload = new CommanderPacketPayload(packetBytes.Skip(1).ToArray());
				return packetPayload;
			}

			return null;
		}

        public override string ToString()
        {
            return string.Format("Roll: {0}, Pitch: {1}, Yaw: {2}, Thrust: {3}. Bytes: {4}", Payload.Roll, Payload.Pitch, Payload.Yaw, Payload.Thrust, BitConverter.ToString(GetBytes()));
        }
    }
}