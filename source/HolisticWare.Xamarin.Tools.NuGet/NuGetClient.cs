using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using NuGet.Protocol;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using NuGet.Frameworks;
using System.Linq;
using Core.Math.Discrete.GraphTheory;

namespace HolisticWare.Xamarin.Tools.NuGet
{
    /// <summary>
    /// 
    /// </summary>
    /// <see cref="https://docs.microsoft.com/en-us/nuget/reference/nuget-client-sdk"/>
    public partial class NuGetClient
    {
        CancellationToken cancellationToken;
        ILogger logger = null;

        SourceCacheContext source_cache = null;
        SourceRepository repository = null;

        public NuGetClient()
        {
            cancellationToken = CancellationToken.None;
            logger = NullLogger.Instance;

            source_cache = new SourceCacheContext();
            repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");

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
                                                    string keyword,
                                                    int number_of_results = 100
                                                )
        {
            PackageSearchResource resource = await repository.GetResourceAsync<PackageSearchResource>();
            SearchFilter filter = new SearchFilter
                                        (
                                            includePrerelease: true,
                                            new SearchFilterType
                                            {                                                       
                                            }
                                        );

            IEnumerable<IPackageSearchMetadata> results = await resource.SearchAsync
                                                                                (
                                                                                    keyword,
                                                                                    filter,
                                                                                    skip: 0,
                                                                                    take: number_of_results,
                                                                                    logger,
                                                                                    cancellationToken
                                                                                );

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
                                                                                source_cache,
                                                                                logger,
                                                                                cancellationToken
                                                                            );

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
                                                                                    source_cache,
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

        public async
            Task<System.Collections.Concurrent.ConcurrentBag<string>>
                                        GetDependencies
                                                (
                                                    string id,
                                                    string version
                                                )
        {
            DependencyInfoResource resource = await repository.GetResourceAsync<DependencyInfoResource>(CancellationToken.None);
            SourcePackageDependencyInfo package = await resource.ResolvePackage
                                                                        (
                                                                            new PackageIdentity(id, new NuGetVersion(version)),
                                                                            NuGetFramework.AnyFramework,
                                                                            source_cache,
                                                                            logger,
                                                                            cancellationToken
                                                                        );
            if (package == null)
            {
                throw new InvalidOperationException("Could not locate dependency!");
            }

            //System.Collections.Concurrent.ConcurrentBag<string> dependency_collection;
            //dependency_collection = new System.Collections.Concurrent.ConcurrentBag<string>();

            Graph<string, string> dependency_graph = new Graph<string, string>();

            foreach (var dependency in package.Dependencies)
            {
                Node<string> dependency_node = new Node<string>()
                {
                    
                };
                //dependency_collection.Add(dependency.Id + " " + dependency.VersionRange.MinVersion);
            }

            await Task.WhenAll
                        (
                            package.Dependencies.Select
                                                    (
                                                        async (d) =>
                                                        {
                                                            var rec = await GetDependencies
                                                                                (
                                                                                    d.Id,
                                                                                    d.VersionRange.MinVersion.ToNormalizedString()
                                                                                );

                                                            foreach (string s in rec)
                                                            {
                                                                dependency_collection.Add(s);
                                                            }
                                                        }
                                                    )
                        );

            return dependency_collection;
        }

    }
}
