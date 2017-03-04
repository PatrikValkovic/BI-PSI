namespace second
{
    enum Command : byte
    {
        DOWNLOAD = 0x1,
        UPLOAD = 0x2
    }

    enum Flag : byte
    {
        RST = 0x1,
        FIN = 0x2,
        SYN = 0x4
    }
}