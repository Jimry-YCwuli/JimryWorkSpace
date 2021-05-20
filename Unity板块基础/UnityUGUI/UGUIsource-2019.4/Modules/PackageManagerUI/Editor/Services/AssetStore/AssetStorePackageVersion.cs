// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace UnityEditor.PackageManager.UI.AssetStore
{
    [Serializable]
    internal class AssetStorePackageVersion : IPackageVersion
    {
        [SerializeField]
        private string m_PackageUniqueId;
        [SerializeField]
        private string m_DisplayName;
        [SerializeField]
        private string m_Author;
        [SerializeField]
        private string m_Description;
        [SerializeField]
        private string m_Category;
        [SerializeField]
        private List<Error> m_Errors;
        [SerializeField]
        private SemVersion m_Version;
        [SerializeField]
        private long m_PublishedDateTicks;
        [SerializeField]
        private string m_PublisherId;
        [SerializeField]
        private bool m_IsAvailableOnDisk;
        [SerializeField]
        private string m_LocalPath;
        [SerializeField]
        private string m_VersionString;
        [SerializeField]
        private string m_VersionId;
        [SerializeField]
        private List<SemVersion> m_SupportedUnityVersions;
        [SerializeField]
        private SemVersion m_SupportedUnityVersion;
        [SerializeField]
        private List<PackageSizeInfo> m_SizeInfos;
        [SerializeField]
        private PackageTag m_Tag;

        public string name => string.Empty;

        public string displayName => m_DisplayName;

        public string author => m_Author;

        public string authorLink => $"{AssetStoreUtils.instance.assetStoreUrl}/publishers/{publisherId}";

        public string description => m_Description;

        public string category => m_Category;

        private Dictionary<string, string> m_CategoryLinks;
        public IDictionary<string, string> categoryLinks
        {
            get
            {
                if (m_CategoryLinks == null)
                {
                    m_CategoryLinks = new Dictionary<string, string>();
                    var categories = m_Category.Split('/');
                    var parentCategory = "/";
                    foreach (var category in categories)
                    {
                        var lower = category.ToLower(CultureInfo.InvariantCulture);
                        var url = $"{AssetStoreUtils.instance.assetStoreUrl}{parentCategory}{lower}";
                        parentCategory += lower + "/";
                        m_CategoryLinks[category] = url;
                    }
                }
                return m_CategoryLinks;
            }
        }

        public string packageUniqueId => m_PackageUniqueId;

        public string uniqueId => m_VersionId;

        public IEnumerable<Error> errors => m_Errors;

        public IEnumerable<Sample> samples => Enumerable.Empty<Sample>();

        public EntitlementsInfo entitlements => null;

        public SemVersion version
        {
            get { return m_Version; }
            set { m_Version = value; }
        }

        public DateTime? publishedDate => m_PublishedDateTicks == 0 ? (DateTime?)null : new DateTime(m_PublishedDateTicks, DateTimeKind.Utc);

        public string publisherId => m_PublisherId;

        public DependencyInfo[] dependencies => null;

        public DependencyInfo[] resolvedDependencies => null;

        public PackageInfo packageInfo => null;

        public bool isInstalled => false;

        public bool isFullyFetched => true;

        public bool isAvailableOnDisk => m_IsAvailableOnDisk;

        public bool isVersionLocked => true;

        public bool canBeRemoved => false;

        public bool canBeEmbedded => false;

        public bool isDirectDependency => true;

        public string localPath
        {
            get { return m_LocalPath; }
            set
            {
                m_LocalPath = value;
                m_IsAvailableOnDisk = !string.IsNullOrEmpty(m_LocalPath) && File.Exists(m_LocalPath);
            }
        }

        public string versionString
        {
            get { return m_VersionString; }
            set { m_VersionString = value; }
        }

        public string versionId
        {
            get { return m_VersionId; }
            set { m_VersionId = value; }
        }

        public SemVersion supportedVersion => m_SupportedUnityVersion;

        public IEnumerable<SemVersion> supportedVersions => m_SupportedUnityVersions;

        public IEnumerable<PackageSizeInfo> sizes => m_SizeInfos;

        public bool HasTag(PackageTag tag)
        {
            return (m_Tag & tag) == tag;
        }

        public AssetStorePackageVersion(FetchedInfo fetchedInfo, LocalInfo localInfo = null)
        {
            if (fetchedInfo == null)
                throw new ArgumentNullException(nameof(fetchedInfo));

            m_Errors = new List<Error>();
            m_Tag = PackageTag.Downloadable | PackageTag.Importable;
            m_PackageUniqueId = fetchedInfo.id.ToString();

            m_Description = fetchedInfo.description;
            m_Author = fetchedInfo.author;
            m_PublisherId = fetchedInfo.publisherId;

            m_Category = fetchedInfo.category;

            m_VersionString = localInfo?.versionString ?? fetchedInfo.versionString ?? string.Empty;
            m_VersionId = localInfo?.versionId ?? fetchedInfo.versionId ?? string.Empty;
            m_Version = SemVersion.TryParse(m_VersionString.Trim(), out m_Version) ? m_Version : new SemVersion(0);

            var publishDateString = localInfo?.publishedDate ?? fetchedInfo.publishedDate ?? string.Empty;
            m_PublishedDateTicks = !string.IsNullOrEmpty(publishDateString) ? DateTime.Parse(publishDateString).Ticks : 0;
            m_DisplayName = !string.IsNullOrEmpty(fetchedInfo.displayName) ? fetchedInfo.displayName : $"Package {m_PackageUniqueId}@{m_VersionId}";

            m_SupportedUnityVersions = new List<SemVersion>();
            if (fetchedInfo.supportedVersions?.Any() ?? false)
            {
                foreach (var supportedVersion in fetchedInfo.supportedVersions)
                {
                    SemVersion version;
                    if (SemVersion.TryParse(supportedVersion as string, out version))
                        m_SupportedUnityVersions.Add(version);
                }
                m_SupportedUnityVersions.Sort((left, right) => left.CompareByPrecedence(right));
            }

            if (localInfo != null)
            {
                var simpleVersion = Regex.Replace(localInfo.supportedVersion, @"(?<major>\d+)\.(?<minor>\d+).(?<patch>\d+)[abfp].+", "${major}.${minor}.${patch}");
                SemVersion.TryParse(simpleVersion.Trim(), out m_SupportedUnityVersion);
            }
            else
            {
                m_SupportedUnityVersion = m_SupportedUnityVersions.LastOrDefault();
            }

            m_SizeInfos = new List<PackageSizeInfo>(fetchedInfo.sizeInfos);
            m_SizeInfos.Sort((left, right) => left.supportedUnityVersion.CompareByPrecedence(right.supportedUnityVersion));

            var state = fetchedInfo.state ?? string.Empty;
            if (state.Equals("published", StringComparison.InvariantCultureIgnoreCase))
                m_Tag |= PackageTag.Published;
            else if (state.Equals("deprecated", StringComparison.InvariantCultureIgnoreCase))
                m_Tag |= PackageTag.Deprecated;

            m_LocalPath = localInfo?.packagePath ?? string.Empty;
            m_IsAvailableOnDisk = !string.IsNullOrEmpty(m_LocalPath) && File.Exists(m_LocalPath);
        }

        public void SetUpmPackageFetchError(Error error)
        {
            m_Errors.Add(error);
            m_Tag &= ~(PackageTag.Downloadable | PackageTag.Importable);
        }
    }
}
