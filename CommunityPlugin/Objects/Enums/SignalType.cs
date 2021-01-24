namespace CommunityPlugin.Objects.Enums
{
    public enum SignalType
    {
        Equal = 1,
        NotEqual = 2,
        GreaterThanOrEqual = 11, // 0x0000000B
        LessThanOrEqual = 12, // 0x0000000C
        GreaterThan = 13, // 0x0000000D
        LessThan = 14, // 0x0000000E
        Contains = 21, // 0x00000015
        StartsWith = 22, // 0x00000016
        EndsWith = 23, // 0x00000017
        Unknown = 99, // 0x00000063
    }
}
