using NzbDrone.Common.Extensions;
using NzbDrone.Core.Parser.Model;

namespace NzbDrone.Core.MediaFiles.EpisodeImport.Augmenting.Augmenters
{
    public class AugmentReleaseGroup : IAugmentLocalEpisode
    {
        public LocalEpisode Augment(LocalEpisode localEpisode, bool otherFiles)
        {
            var releaseGroup = localEpisode.DownloadClientEpisodeInfo?.ReleaseGroup;

            if (releaseGroup.IsNullOrWhiteSpace())
            {
                releaseGroup = localEpisode.FolderEpisodeInfo?.ReleaseGroup;
            }

            if (releaseGroup.IsNullOrWhiteSpace())
            {
                releaseGroup = localEpisode.FileEpisodeInfo?.ReleaseGroup;
            }

            localEpisode.ReleaseGroup = releaseGroup;

            return localEpisode;
        }
    }
}
