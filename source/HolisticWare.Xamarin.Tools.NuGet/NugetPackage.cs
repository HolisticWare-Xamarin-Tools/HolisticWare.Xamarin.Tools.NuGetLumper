using System;
using System.Collections.Generic;

namespace HolisticWare.Xamarin.Tools.NuGet
{
    public partial class NugetPackage
    {
        public NugetPackage()
        {
        }

        public string Id
        {
            get;
            set;
        }

        public string VersionLiteral
        {
            get;
            set;
        }

        public Version Version
        {
            get;
            set;
        }

        public List<NugetPackage> Dependencies
        {
            get;
            set;
        }

    }
}
