// Guids.cs
// MUST match guids.h
using System;

namespace MikkoHalttunen.VSCompTools2
{
    static class GuidList
    {
        public const string guidVSCompTools2PkgString = "4d4f9f9a-6eba-4dd8-b5ef-7d48b94a312c";
        public const string guidVSCompTools2CmdSetString = "a1543206-66d1-43b2-8b96-7340b7f8d238";

        public static readonly Guid guidVSCompTools2CmdSet = new Guid(guidVSCompTools2CmdSetString);
    };
}