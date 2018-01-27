using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;
using Moq;
using NUnit.Framework;
using NzbDrone.Core.MediaFiles.EpisodeImport.Augmenting.Augmenters;
using NzbDrone.Core.Parser;
using NzbDrone.Core.Parser.Model;
using NzbDrone.Core.Test.Framework;
using NzbDrone.Core.Tv;
using NzbDrone.Test.Common;

namespace NzbDrone.Core.Test.MediaFiles.EpisodeImport.Augmenting.Augmenters
{
    [TestFixture]
    public class AugmentEpisodesFixture : CoreTest<AugmentEpisodes>
    {
        private Series _series;

        [SetUp]
        public void Setup()
        {
            _series = Builder<Series>.CreateNew().Build();

            var augmenters = new List<Mock<IAugmentLocalEpisode>>
                             {
                                 new Mock<IAugmentLocalEpisode>()
                             };

            Mocker.SetConstant(augmenters.Select(c => c.Object));
        }

        [Test]
        public void should_not_use_folder_for_full_season()
        {
            var fileEpisodeInfo = Parser.Parser.ParseTitle("Series.Title.S01E01");
            var folderEpisodeInfo = Parser.Parser.ParseTitle("Series.Title.S01");
            var localEpisode = new LocalEpisode
                               {
                                   FileEpisodeInfo = fileEpisodeInfo,
                                   FolderEpisodeInfo = folderEpisodeInfo,
                                   Path = @"C:\Test\Unsorted TV\Series.Title.S01\Series.Title.S01E01.mkv".AsOsAgnostic(),
                                   Series = _series
                               };

            Subject.Augment(localEpisode, false);

            Mocker.GetMock<IParsingService>()
                  .Verify(v => v.GetEpisodes(fileEpisodeInfo, _series, localEpisode.SceneSource, null), Times.Once());
        }

        [Test]
        public void should_not_use_folder_when_it_contains_more_than_one_valid_video_file()
        {
            var fileEpisodeInfo = Parser.Parser.ParseTitle("Series.Title.S01E01");
            var folderEpisodeInfo = Parser.Parser.ParseTitle("Series.Title.S01");
            var localEpisode = new LocalEpisode
            {
                FileEpisodeInfo = fileEpisodeInfo,
                FolderEpisodeInfo = folderEpisodeInfo,
                Path = @"C:\Test\Unsorted TV\Series.Title.S01\Series.Title.S01E01.mkv".AsOsAgnostic(),
                Series = _series
            };

            Subject.Augment(localEpisode, true);

            Mocker.GetMock<IParsingService>()
                  .Verify(v => v.GetEpisodes(fileEpisodeInfo, _series, localEpisode.SceneSource, null), Times.Once());
        }

        [Test]
        public void should_not_use_folder_name_if_file_name_is_scene_name()
        {
            var fileEpisodeInfo = Parser.Parser.ParseTitle("Series.Title.S01E01");
            var folderEpisodeInfo = Parser.Parser.ParseTitle("Series.Title.S01E01");
            var localEpisode = new LocalEpisode
            {
                FileEpisodeInfo = fileEpisodeInfo,
                FolderEpisodeInfo = folderEpisodeInfo,
                Path = @"C:\Test\Unsorted TV\Series.Title.S01E01\Series.Title.S01E01.720p.HDTV-Sonarr.mkv".AsOsAgnostic(),
                Series = _series
            };

            Subject.Augment(localEpisode, false);

            Mocker.GetMock<IParsingService>()
                  .Verify(v => v.GetEpisodes(fileEpisodeInfo, _series, localEpisode.SceneSource, null), Times.Once());
        }

        [Test]
        public void should_use_folder_when_only_one_video_file()
        {
            var fileEpisodeInfo = Parser.Parser.ParseTitle("Series.Title.S01E01");
            var folderEpisodeInfo = Parser.Parser.ParseTitle("Series.Title.S01E01");
            var localEpisode = new LocalEpisode
            {
                FileEpisodeInfo = fileEpisodeInfo,
                FolderEpisodeInfo = folderEpisodeInfo,
                Path = @"C:\Test\Unsorted TV\Series.Title.S01E01\Series.Title.S01E01.mkv".AsOsAgnostic(),
                Series = _series
            };

            Subject.Augment(localEpisode, false);

            Mocker.GetMock<IParsingService>()
                  .Verify(v => v.GetEpisodes(folderEpisodeInfo, _series, localEpisode.SceneSource, null), Times.Once());
        }
    }
}
