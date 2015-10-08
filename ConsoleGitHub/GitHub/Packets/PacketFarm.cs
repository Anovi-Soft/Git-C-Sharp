namespace GitHub.Packets
{
    class PacketFarm
    {
        public static ICommandPacket GetFromBytes(byte[] bytes)
        {
            return CommandPacket.FromBytes(bytes);
        }
        
    }
}
