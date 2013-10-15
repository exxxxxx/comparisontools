// Guids.cs
// MUST match guids.h
using System;

namespace VSCompTools
{
    static class GuidList
    {
        public const string guidVSCompToolsPkgString = "aff98032-7c33-47c4-a6f6-7879970a1d68";
        public const string guidVSCompToolsCmdSetString = "6b182c7d-08b3-46e2-b1d0-1fb3c2f9ffc1";

        public static readonly Guid guidVSCompToolsCmdSet = new Guid(guidVSCompToolsCmdSetString);
    };
}