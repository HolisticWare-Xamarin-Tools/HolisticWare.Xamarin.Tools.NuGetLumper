using System;
using Xunit;

using NuGet.Common;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

using HolisticWare.Xamarin.Tools.NuGet;

namespace UnitTests.XUnit
{
    public class UnitTest_NugetClient
    {
        [Theory]
        [InlineData("AndroidX")]
        [InlineData("HolisticWare")]
        [InlineData("TensofFlow")]
        public void Test_Search(string search_term)
        {
            NuGetClient nc = new NuGetClient();

            var results = nc.SearchPackagesByKeywordAsync(search_term).Result;

            string output = "";

            foreach (IPackageSearchMetadata r in results)
            {
                output += $"Found package";
                output += Environment.NewLine;
                output += $"    Id              = {r.Identity.Id}";
                output += Environment.NewLine;
                output += $"    Version         = {r.Identity.Version}";
                output += Environment.NewLine;
                output += $"    Description     = {r.Description}";
                output += Environment.NewLine;
                output += $"    Summary         = {r.Summary}";
                output += Environment.NewLine;
                output += $"    Tags            = {r.Tags}";
                output += Environment.NewLine;
                output += $"    DownloadCount   = {r.DownloadCount}";
                output += Environment.NewLine;
                output += $"    Authors         = {r.Authors}";
                output += Environment.NewLine;
                output += $"    Owners          = {r.Owners}";
                output += Environment.NewLine;
                output += $"    DependencySets  = {r.DependencySets.ToJson()}";
                output += Environment.NewLine;
                output += $"    IconUrl         = {r.IconUrl}";
                output += Environment.NewLine;
            }

            System.IO.File.WriteAllText($"{search_term}.txt", output);

            return;
        }

        [Fact]
        public void Test_Versions()
        {
            NuGetClient nc = new NuGetClient();

            var versions = nc.GetPackageVersionsAsync("Xamarin.AndroidX.Activity").Result;

            foreach (NuGetVersion version in versions)
            {
                Console.WriteLine($"Found version {version}");
            }

            return;
        }
    }
}
