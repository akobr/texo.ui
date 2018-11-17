using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using BeaverSoft.Texo.Commands.NugetManager.Model;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace BeaverSoft.Texo.Commands.NugetManager.Services
{
    public class SourceManagementService : ISourceManagementService
    {
        private const string OFFICIAL_PACKAGE_REPOSITORY_URL = "https://packages.nuget.org/api/v2";

        private ImmutableHashSet<string> sources;

        public SourceManagementService()
        {
            ResetSources();
        }

        public IEnumerable<string> GetAllSources()
        {
            return sources;
        }

        public void Add(string source)
        {
            sources.Add(source);
        }

        public void AddRange(IEnumerable<string> newSources)
        {
            foreach (string source in newSources)
            {
                Add(source);
            }
        }

        public IPackageInfo FetchPackage(string packageId)
        {
            IPackageInfo result = null;

            foreach (string source in sources)
            {
                IPackageInfo package = GetPackageFromRepository(packageId, source);
                result = result == null
                    ? package
                    : new PackageInfo(result.Id, result.Versions.Union(package.Versions));
            }

            return result;
        }

        public IImmutableDictionary<string, IPackageInfo> SearchPackages(string searchTerm)
        {
            var result = ImmutableSortedDictionary<string, IPackageInfo>.Empty.ToBuilder();

            foreach (string source in sources)
            {
                foreach (IPackageInfo package in GetPackagesFromRepository(searchTerm, source))
                {
                    if (result.TryGetValue(package.Id, out IPackageInfo existingPackage))
                    {
                        result[package.Id] =
                            new PackageInfo(package.Id, existingPackage.Versions.Union(package.Versions));
                    }
                    else
                    {
                        result[package.Id] = package;
                    }
                }
            }

            return result.ToImmutable();
        }

        public void Clear()
        {
            ResetSources();
        }

        private IEnumerable<IPackageInfo> GetPackagesFromRepository(string searchTerm, string repositoryUrl)
        {
            //IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository(repositoryUrl);
            //var versions = new Dictionary<string, ImmutableSortedSet<string>.Builder>(new InsensitiveStringComparer());
            //var previousPackageVersions = ImmutableSortedSet.CreateBuilder<string>(new InsensitiveOpositeStringComparer());
            //string previousPackageId = string.Empty;

            //foreach (NuGet.IPackage nugetPackage in repo.Search(searchTerm, true))
            //{
            //    if (!string.Equals(previousPackageId, nugetPackage.Id, StringComparison.OrdinalIgnoreCase))
            //    {
            //        previousPackageId = nugetPackage.Id;

            //        if (!versions.TryGetValue(previousPackageId, out previousPackageVersions))
            //        {
            //            previousPackageVersions = ImmutableSortedSet.CreateBuilder<string>(new InsensitiveOpositeStringComparer());
            //            versions[previousPackageId] = previousPackageVersions;
            //        }
            //    }

            //    previousPackageVersions.Add(nugetPackage.Version.ToNormalizedString());
            //}

            //foreach (var packagePair in versions)
            //{
            //    yield return new PackageInfo(packagePair.Key, packagePair.Value.ToImmutable());
            //}

            var packages = new Dictionary<string, ImmutableSortedSet<string>.Builder>(new InsensitiveStringComparer());
            var previousPackageVersions =
                ImmutableSortedSet.CreateBuilder<string>(new InsensitiveOpositeStringComparer());
            string previousPackageId = string.Empty;

            PackageSearchResource resource = BuildSearchResource(repositoryUrl);
            var searchResult = resource.SearchAsync(
                searchTerm, new SearchFilter(true), 0, 10,
                new Logger(), CancellationToken.None).Result;

            foreach (IPackageSearchMetadata metadata in searchResult)
            {
                if (!string.Equals(previousPackageId, metadata.Identity.Id, StringComparison.OrdinalIgnoreCase))
                {
                    previousPackageId = metadata.Identity.Id;

                    if (!packages.TryGetValue(previousPackageId, out previousPackageVersions))
                    {
                        previousPackageVersions =
                            ImmutableSortedSet.CreateBuilder<string>(new InsensitiveOpositeStringComparer());
                        packages[previousPackageId] = previousPackageVersions;
                    }
                }

                if (metadata.Identity.HasVersion)
                {
                    previousPackageVersions.Add(metadata.Identity.Version.ToNormalizedString());
                }
            }

            foreach (var packagePair in packages)
            {
                yield return new PackageInfo(packagePair.Key, packagePair.Value.ToImmutable());
            }
        }

        private static IPackageInfo GetPackageFromRepository(string packageId, string repositoryUrl)
        {
            //IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository(repositoryUrl);
            //var versions = ImmutableSortedSet.CreateBuilder<string>(new InsensitiveOpositeStringComparer());
            //NuGet.IPackage lastNugetPackage = null;

            //foreach (NuGet.IPackage nugetPackage in repo.FindPackagesById(packageId))
            //{
            //    lastNugetPackage = nugetPackage;
            //    versions.Add(nugetPackage.Version.ToNormalizedString());
            //}

            //if (lastNugetPackage == null)
            //{
            //    return null;
            //}

            //return new PackageInfo(lastNugetPackage.Id, versions.ToImmutable());

            PackageMetadataResource resourse = BuildPackageResource(repositoryUrl);
            string resultPackageId = packageId;
            var versions = ImmutableSortedSet.CreateBuilder<string>(new InsensitiveOpositeStringComparer());

            var metadataResult = resourse.GetMetadataAsync(
                packageId,
                true, true,
                null, new Logger(),
                CancellationToken.None).Result;

            foreach (IPackageSearchMetadata metadata in metadataResult)
            {
                resultPackageId = metadata.Identity.Id;

                if (metadata.Identity.HasVersion)
                {
                    versions.Add(metadata.Identity.Version.ToNormalizedString());
                }
            }

            return new PackageInfo(resultPackageId, versions.ToImmutable());
        }

        private static PackageSearchResource BuildSearchResource(string repositoryUrl)
        {
            SourceRepository repository = BuildSourceRepository(repositoryUrl);
            return repository.GetResource<PackageSearchResource>();
        }

        private static PackageMetadataResource BuildPackageResource(string repositoryUrl)
        {
            SourceRepository repository = BuildSourceRepository(repositoryUrl);
            return repository.GetResource<PackageMetadataResource>();
        }

        private static SourceRepository BuildSourceRepository(string repositoryUrl)
        {
            List<Lazy<INuGetResourceProvider>> providers = new List<Lazy<INuGetResourceProvider>>();
            providers.AddRange(Repository.Provider.GetCoreV3()); // Add v3 API support
            //providers.AddRange(Repository.Provider.GetCoreV2());  // Add v2 API support
            PackageSource packageSource = new PackageSource(repositoryUrl);
            return new SourceRepository(packageSource, providers);
        }

        private void ResetSources()
        {
            sources = ImmutableHashSet<string>.Empty;
            sources = sources.Add(OFFICIAL_PACKAGE_REPOSITORY_URL);
        }

        public class Logger : ILogger
        {
            public void LogDebug(string data)
            {
            }

            public void LogVerbose(string data)
            {
            }

            public void LogInformation(string data)
            {
            }

            public void LogMinimal(string data)
            {
            }

            public void LogWarning(string data)
            {
            }

            public void LogError(string data)
            {
            }

            public void LogInformationSummary(string data)
            {
            }

            public void Log(LogLevel level, string data)
            {
            }

            public Task LogAsync(LogLevel level, string data)
            {
                return Task.FromResult(0);
            }

            public void Log(ILogMessage message)
            {
            }

            public Task LogAsync(ILogMessage message)
            {
                return Task.FromResult(0);
            }
        }
    }
}