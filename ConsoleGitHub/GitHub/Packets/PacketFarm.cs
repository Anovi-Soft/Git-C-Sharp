namespace GitHub.Packets
{
    class PacketFarm
    {
        public static ICommandPacket GetFromBytes(byte[] bytes)
        {
            return CPacket.FromBytes(bytes);
        }
        
    }
}
