using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace HolisticWare.Xamarin.Tools.NuGet
{
    /// <summary>
    /// 
    /// </summary>
    /// <see cref="https://docs.microsoft.com/en-us/nuget/reference/nuget-client-sdk"/>
    public partial class Client
    {
        ILogger logger = null;
        CancellationToken cancellationToken;

        SourceCacheContext cache = null;
        SourceRepository repository = null;

        public Client()
        {
            ILogger logger = NullLogger.Instance;
            CancellationToken cancellationToken = CancellationToken.None;

            SourceCacheContext cache = new SourceCacheContext();
            SourceRepository repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");

            return;
        }

        /// <summary>
        /// Search for NuGet packages by keyword
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns>IEnumerable<NuGet.Protocol.Core.Types.IPackageSearchMetadata></returns>
        public async
            Task<IEnumerable<IPackageSearchMetadata>>
                                        SearchPackagesByKeywordAsync
                                                (
                                                    string keyword
                                                )
        {
            SourceRepository repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
            PackageSearchResource resource = await repository.GetResourceAsync<PackageSearchResource>();
            SearchFilter searchFilter = new SearchFilter
                                                (
                                                    includePrerelease: true,
                                                    new SearchFilterType
                                                    {                                                       
                                                    }
                                                );

            IEnumerable<IPackageSearchMetadata> results = await resource.SearchAsync
                                                                                (
                                                                                    keyword,
                                                                                    searchFilter,
                                                                                    skip: 0,
                                                                                    take: 100,
                                                                                    logger,
                                                                                    cancellationToken
                                                                                );

            foreach (IPackageSearchMetadata result in results)
            {
                Console.WriteLine($"Found package {result.Identity.Id} {result.Identity.Version}");
            }

            return results;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nuget_id"></param>
        /// <returns><IEnumerable<NuGet.Versioning.NuGetVersion>></returns>
        /// <see cref="https://docs.microsoft.com/en-us/nuget/reference/nuget-client-sdk#list-package-versions"/>
        public async
            Task<IEnumerable<NuGetVersion>>
                                        GetPackageVersionsAsync
                                                (
                                                    string nuget_id
                                                )
        {
            FindPackageByIdResource resource = await repository.GetResourceAsync<FindPackageByIdResource>();

            IEnumerable<NuGetVersion> versions = await resource.GetAllVersionsAsync
                                                                            (
                                                                                nuget_id,
                                                                                cache,
                                                                                logger,
                                                                                cancellationToken
                                                                            );

            foreach (NuGetVersion version in versions)
            {
                Console.WriteLine($"Found version {version}");
            }

            return versions;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="nuget_id"></param>
        /// <returns></returns>
        /// <see cref="https://docs.microsoft.com/en-us/nuget/reference/nuget-client-sdk#get-package-metadata"/>
        public async
            Task<IEnumerable<IPackageSearchMetadata>>
                                        GetPackageMetadataAsync
                                                (
                                                    string nuget_id
                                                )
        {
            PackageMetadataResource resource = await repository.GetResourceAsync<PackageMetadataResource>();

            IEnumerable<IPackageSearchMetadata> packages = await resource.GetMetadataAsync
                                                                                (
                                                                                    nuget_id,
                                                                                    includePrerelease: true,
                                                                                    includeUnlisted: false,
                                                                                    cache,
                                                                                    logger,
                                                                                    cancellationToken
                                                                                );

            foreach (IPackageSearchMetadata package in packages)
            {
                Console.WriteLine($"Version: {package.Identity.Version}");
                Console.WriteLine($"Listed: {package.IsListed}");
                Console.WriteLine($"Tags: {package.Tags}");
                Console.WriteLine($"Description: {package.Description}");
            }
            return packages;
        }

    }
}
