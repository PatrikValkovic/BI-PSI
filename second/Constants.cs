using System;

namespace second
{
    enum Command : byte
    {
        DOWNLOAD = 0x1,
        UPLOAD = 0x2
    }

    [Flags]
    enum Flag : byte
    {
        RST = 0x1,
        FIN = 0x2,
        SYN = 0x4
    }

    [Flags]
    public enum Sizes : int
    {
        HEADER_SIZE = 9,
        PACKET_MIN = 9,
        PACKET_MAX = 264,
        TIMEOUT = 100,
        MAX_DATA = 255,
        WINDOW_SIZE = 2040,
        WINDOW_PACKETS = WINDOW_SIZE / MAX_DATA
    }
}