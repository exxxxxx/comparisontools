// Guids.cs
// MUST match guids.h
using System;

namespace VisualStudioComparisonToolsVSPackage
{
    static class GuidList
    {
        public const string guidVisualStudioComparisonToolsVSPackagePkgString = "9cc9b6af-31d9-417b-8900-fd95dbcff788";
        public const string guidVisualStudioComparisonToolsVSPackageCmdSetString = "66493f66-f2ea-4063-8957-e8445088c748";

        public static readonly Guid guidVisualStudioComparisonToolsVSPackageCmdSet = new Guid(guidVisualStudioComparisonToolsVSPackageCmdSetString);
    };
}