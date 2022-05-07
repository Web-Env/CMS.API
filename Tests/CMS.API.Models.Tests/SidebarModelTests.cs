using CMS.API.Models.Content;
using CMS.API.Models.Tests.Helpers;
using CMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CMS.API.Models.Tests
{
    [Trait("Category", "Unit")]
    public class SidebarModelTests
    {
        #region MapSidebarButtonsToSidebarButtonDownloadModels
        // MapSidebarButtonsToSidebarButtonDownloadModels_WithZeroSectionsAndZeroContents
        [Fact]
        public void MapSidebarButtonsToSidebarButtonDownloadModels_WithZeroSectionsAndZeroContentsAndIsAdminEqualsFalse_ShouldReturnZeroSidebarButtonDownloadModels()
        {
            var sections = new List<Section>();
            var contents = new List<Domain.Entities.Content>();

            var sidebarButtonDownloadModels = SidebarModel.MapSidebarButtonsToSidebarButtonDownloadModels(sections, contents, false);

            Assert.Empty(sidebarButtonDownloadModels);
        }

        [Fact]
        public void MapSidebarButtonsToSidebarButtonDownloadModels_WithZeroSectionsAndZeroContentsAndIsAdminEqualsTrue_ShouldReturnOneSidebarButtonDownloadModelWithCorrectProperties()
        {
            var sections = new List<Section>();
            var contents = new List<Domain.Entities.Content>();

            var sidebarButtonDownloadModels = SidebarModel.MapSidebarButtonsToSidebarButtonDownloadModels(sections, contents, true);
            var sidebarButtonDownloadModel = sidebarButtonDownloadModels.FirstOrDefault();

            Assert.Single(sidebarButtonDownloadModels);
            Assert.Equal("Admin", sidebarButtonDownloadModel.Title);
            Assert.Equal("admin", sidebarButtonDownloadModel.Path);
            Assert.Equal(4, sidebarButtonDownloadModel.SubButtons.Count);
        }

        // MapSidebarButtonsToSidebarButtonDownloadModels_WithOneSectionAndZeroContents
        [Fact]
        public void MapSidebarButtonsToSidebarButtonDownloadModels_WithOneSectionAndZeroContentsAndIsAdminEqualsFalse_ShouldReturnZeroSidebarButtonDownloadModels()
        {
            var sections = new List<Section> { SidebarModelTestHelpers.CreateDummySection() };
            var contents = new List<Domain.Entities.Content>();

            var sidebarButtonDownloadModels = SidebarModel.MapSidebarButtonsToSidebarButtonDownloadModels(sections, contents, false);

            Assert.Empty(sidebarButtonDownloadModels);
        }

        [Fact]
        public void MapSidebarButtonsToSidebarButtonDownloadModels_WithOneSectionAndZeroContentsAndIsAdminEqualsTrue_ShouldReturnOneSidebarButtonDownloadModelWithCorrectProperties()
        {
            var sections = new List<Section> { SidebarModelTestHelpers.CreateDummySection() };
            var contents = new List<Domain.Entities.Content>();

            var sidebarButtonDownloadModels = SidebarModel.MapSidebarButtonsToSidebarButtonDownloadModels(sections, contents, true);
            var sidebarButtonDownloadModel = sidebarButtonDownloadModels.FirstOrDefault();

            Assert.Single(sidebarButtonDownloadModels);
            Assert.Equal("Admin", sidebarButtonDownloadModel.Title);
            Assert.Equal("admin", sidebarButtonDownloadModel.Path);
            Assert.Equal(4, sidebarButtonDownloadModel.SubButtons.Count);
        }

        // MapSidebarButtonsToSidebarButtonDownloadModels_WithZeroSectionsAndOneContent
        [Fact]
        public void MapSidebarButtonsToSidebarButtonDownloadModels_WithZeroSectionsAndOneContentAndIsAdminEqualsFalse_ShouldReturnOneSidebarButtonDownloadModelWithCorrectProperties()
        {
            var sections = new List<Section>();
            var contents = new List<Domain.Entities.Content> { SidebarModelTestHelpers.CreateDummyContent() };

            var sidebarButtonDownloadModels = SidebarModel.MapSidebarButtonsToSidebarButtonDownloadModels(sections, contents, false);

            Assert.Single(sidebarButtonDownloadModels);
        }

        [Fact]
        public void MapSidebarButtonsToSidebarButtonDownloadModels_WithZeroSectionsAndOneContentAndIsAdminEqualsTrue_ShouldReturnTwoSidebarButtonDownloadModelWithCorrectProperties()
        {
            var sections = new List<Section>();
            var contents = new List<Domain.Entities.Content> { SidebarModelTestHelpers.CreateDummyContent() };

            var sidebarButtonDownloadModels = SidebarModel.MapSidebarButtonsToSidebarButtonDownloadModels(sections, contents, true);
            var firstSidebarButtonDownloadModel = sidebarButtonDownloadModels.FirstOrDefault();
            var secondSidebarButtonDownloadModel = sidebarButtonDownloadModels[1];

            Assert.Equal(2, sidebarButtonDownloadModels.Count);
            Assert.Equal(SidebarModelTestHelpers.DummyContentTitle, firstSidebarButtonDownloadModel.Title);
            Assert.Equal(SidebarModelTestHelpers.DummyContentPath, firstSidebarButtonDownloadModel.Path);
            Assert.Equal(4, secondSidebarButtonDownloadModel.SubButtons.Count);
        }

        // MapSidebarButtonsToSidebarButtonDownloadModels_WithOneSectionAndOneRelatedContent
        [Fact]
        public void MapSidebarButtonsToSidebarButtonDownloadModels_WithOneSectionAndOneRelatedContentAndIsAdminEqualsFalse_ShouldReturnTwoSidebarButtonDownloadModelWithCorrectProperties()
        {
            var sections = new List<Section> { SidebarModelTestHelpers.CreateDummySection() };
            var sectionId = SidebarModelTestHelpers.DummySectionId;
            var contents = new List<Domain.Entities.Content> { SidebarModelTestHelpers.CreateDummyContent(sectionId) };
            var expectedSidebarSubButtonPath = $"{SidebarModelTestHelpers.DummySectionPath}/{SidebarModelTestHelpers.DummyContentPath}";

            var sidebarButtonDownloadModels = SidebarModel.MapSidebarButtonsToSidebarButtonDownloadModels(sections, contents, false);
            var sidebarButtonDownloadModel = sidebarButtonDownloadModels.FirstOrDefault();
            var sidebarButtonDownloadModelSubButton = sidebarButtonDownloadModel.SubButtons.FirstOrDefault();

            Assert.Single(sidebarButtonDownloadModels);
            Assert.Equal(SidebarModelTestHelpers.DummySectionTitle, sidebarButtonDownloadModel.Title);
            Assert.Equal(SidebarModelTestHelpers.DummySectionPath, sidebarButtonDownloadModel.Path);
            Assert.Single(sidebarButtonDownloadModel.SubButtons);
            Assert.Equal(SidebarModelTestHelpers.DummyContentTitle, sidebarButtonDownloadModelSubButton.Title);
            Assert.Equal(expectedSidebarSubButtonPath, sidebarButtonDownloadModelSubButton.Path);
        }

        [Fact]
        public void MapSidebarButtonsToSidebarButtonDownloadModels_WithOneSectionAndOneRelatedContentAndIsAdminEqualsTrue_ShouldReturnOneSidebarButtonDownloadModelWithCorrectProperties()
        {
            var sections = new List<Section> { SidebarModelTestHelpers.CreateDummySection() };
            var sectionId = SidebarModelTestHelpers.DummySectionId;
            var contents = new List<Domain.Entities.Content> { SidebarModelTestHelpers.CreateDummyContent(sectionId) };
            var expectedSidebarSubButtonPath = $"{SidebarModelTestHelpers.DummySectionPath}/{SidebarModelTestHelpers.DummyContentPath}";

            var sidebarButtonDownloadModels = SidebarModel.MapSidebarButtonsToSidebarButtonDownloadModels(sections, contents, true);
            var firstSidebarButtonDownloadModel = sidebarButtonDownloadModels.FirstOrDefault();
            var firstSidebarButtonDownloadModelSubButton = firstSidebarButtonDownloadModel.SubButtons.FirstOrDefault();
            var secondSidebarButtonDownloadModel = sidebarButtonDownloadModels[1];

            Assert.Equal(2, sidebarButtonDownloadModels.Count);
            Assert.Equal(SidebarModelTestHelpers.DummySectionTitle, firstSidebarButtonDownloadModel.Title);
            Assert.Equal(SidebarModelTestHelpers.DummySectionPath, firstSidebarButtonDownloadModel.Path);
            Assert.Single(firstSidebarButtonDownloadModel.SubButtons);
            Assert.Equal(SidebarModelTestHelpers.DummyContentTitle, firstSidebarButtonDownloadModelSubButton.Title);
            Assert.Equal(expectedSidebarSubButtonPath, firstSidebarButtonDownloadModelSubButton.Path);
            Assert.Equal("Admin", secondSidebarButtonDownloadModel.Title);
            Assert.Equal("admin", secondSidebarButtonDownloadModel.Path);
            Assert.Equal(4, secondSidebarButtonDownloadModel.SubButtons.Count);
        }

        // MapSidebarButtonsToSidebarButtonDownloadModels_WithOneSectionAndOneRelatedContentAndOneUnrelatedContent
        [Fact]
        public void MapSidebarButtonsToSidebarButtonDownloadModels_WithOneSectionAndOneRelatedContentAndOneUnrelatedContentAndIsAdminEqualsFalse_ShouldReturnTwoSidebarButtonDownloadModelWithCorrectProperties()
        {
            var sections = new List<Section> { SidebarModelTestHelpers.CreateDummySection() };
            var sectionId = SidebarModelTestHelpers.DummySectionId;
            var contents = new List<Domain.Entities.Content> {
                SidebarModelTestHelpers.CreateDummyContent(sectionId),
                SidebarModelTestHelpers.CreateDummyContent()
            };
            var expectedSidebarSubButtonPath = $"{SidebarModelTestHelpers.DummySectionPath}/{SidebarModelTestHelpers.DummyContentPath}";

            var sidebarButtonDownloadModels = SidebarModel.MapSidebarButtonsToSidebarButtonDownloadModels(sections, contents, false);
            var contentSidebarButtonDownloadModel = sidebarButtonDownloadModels.FirstOrDefault(x => x.SubButtons.Count == 0);
            var sectionSidebarButtonDownloadModel = sidebarButtonDownloadModels.FirstOrDefault(x => x.SubButtons.Any());
            var sidebarButtonDownloadModelSubButton = sectionSidebarButtonDownloadModel.SubButtons.FirstOrDefault();

            Assert.Equal(2, sidebarButtonDownloadModels.Count);
            Assert.Equal(SidebarModelTestHelpers.DummyContentTitle, contentSidebarButtonDownloadModel.Title);
            Assert.Equal(SidebarModelTestHelpers.DummyContentPath, contentSidebarButtonDownloadModel.Path);
            Assert.Equal(SidebarModelTestHelpers.DummySectionTitle, sectionSidebarButtonDownloadModel.Title);
            Assert.Equal(SidebarModelTestHelpers.DummySectionPath, sectionSidebarButtonDownloadModel.Path);
            Assert.Single(sectionSidebarButtonDownloadModel.SubButtons);
            Assert.Equal(SidebarModelTestHelpers.DummyContentTitle, sidebarButtonDownloadModelSubButton.Title);
            Assert.Equal(expectedSidebarSubButtonPath, sidebarButtonDownloadModelSubButton.Path);
        }

        [Fact]
        public void MapSidebarButtonsToSidebarButtonDownloadModels_WithOneSectionAndOneRelatedContentAndOneUnrelatedContentAndIsAdminEqualsTrue_ShouldReturnOneSidebarButtonDownloadModelWithCorrectProperties()
        {
            var sections = new List<Section> { SidebarModelTestHelpers.CreateDummySection() };
            var sectionId = SidebarModelTestHelpers.DummySectionId;
            var contents = new List<Domain.Entities.Content> {
                SidebarModelTestHelpers.CreateDummyContent(sectionId),
                SidebarModelTestHelpers.CreateDummyContent()
            };
            var expectedSidebarSubButtonPath = $"{SidebarModelTestHelpers.DummySectionPath}/{SidebarModelTestHelpers.DummyContentPath}";

            var sidebarButtonDownloadModels = SidebarModel.MapSidebarButtonsToSidebarButtonDownloadModels(sections, contents, true);
            var contentSidebarButtonDownloadModel = sidebarButtonDownloadModels.FirstOrDefault(x => x.SubButtons.Count == 0);
            var sectionSidebarButtonDownloadModel = sidebarButtonDownloadModels.FirstOrDefault(x => x.SubButtons.Any());
            var sidebarButtonDownloadModelSubButton = sectionSidebarButtonDownloadModel.SubButtons.FirstOrDefault();
            var adminSectionSidebarButtonDownloadModel = sidebarButtonDownloadModels.FirstOrDefault(x => x.Title == "Admin");

            Assert.Equal(3, sidebarButtonDownloadModels.Count);
            Assert.Equal(SidebarModelTestHelpers.DummyContentTitle, contentSidebarButtonDownloadModel.Title);
            Assert.Equal(SidebarModelTestHelpers.DummyContentPath, contentSidebarButtonDownloadModel.Path);
            Assert.Equal(SidebarModelTestHelpers.DummySectionTitle, sectionSidebarButtonDownloadModel.Title);
            Assert.Equal(SidebarModelTestHelpers.DummySectionPath, sectionSidebarButtonDownloadModel.Path);
            Assert.Single(sectionSidebarButtonDownloadModel.SubButtons);
            Assert.Equal(SidebarModelTestHelpers.DummyContentTitle, sidebarButtonDownloadModelSubButton.Title);
            Assert.Equal(expectedSidebarSubButtonPath, sidebarButtonDownloadModelSubButton.Path);
            Assert.Equal("Admin", adminSectionSidebarButtonDownloadModel.Title);
            Assert.Equal("admin", adminSectionSidebarButtonDownloadModel.Path);
            Assert.Equal(4, adminSectionSidebarButtonDownloadModel.SubButtons.Count);
        }
        #endregion

        #region MapContentToSections
        [Fact]
        public void MapContentToSections_WithZeroSectionsAndZeroContents_ShouldReturnZeroSidebarButtonDownloadModels()
        {
            var sections = new List<Section>();
            var contents = new List<Domain.Entities.Content>();
            var sectionContentIds = new List<Guid>();

            var sidebarButtonDownloadModels = SidebarModel.MapContentToSections(sections, contents, sectionContentIds);

            Assert.Empty(sidebarButtonDownloadModels);
        }

        [Fact]
        public void MapContentToSections_WithOneSectionAndZeroContents_ShouldReturnZeroSidebarButtonDownloadModels()
        {
            var sections = new List<Section> { SidebarModelTestHelpers.CreateDummySection() };
            var contents = new List<Domain.Entities.Content>();
            var sectionContentIds = new List<Guid>();

            var sidebarButtonDownloadModels = SidebarModel.MapContentToSections(sections, contents, sectionContentIds);

            Assert.Empty(sidebarButtonDownloadModels);
        }

        [Fact]
        public void MapContentToSections_WithZeroSectionsAndOneContent_ShouldReturnZeroSidebarButtonDownloadModelsAndZeroSectionContentIds()
        {
            var sections = new List<Section>();
            var contents = new List<Domain.Entities.Content> { SidebarModelTestHelpers.CreateDummyContent() };
            var sectionContentIds = new List<Guid>();

            var sidebarButtonDownloadModels = SidebarModel.MapContentToSections(sections, contents, sectionContentIds);

            Assert.Empty(sidebarButtonDownloadModels);
            Assert.Empty(sectionContentIds);
        }

        [Fact]
        public void MapContentToSections_WithZeroSectionsAndTwoContents_ShouldReturnZeroSidebarButtonDownloadModelsAndZeroSectionContentIds()
        {
            var sections = new List<Section>();
            var contents = new List<Domain.Entities.Content> {
                SidebarModelTestHelpers.CreateDummyContent(),
                SidebarModelTestHelpers.CreateDummyContent()
            };
            var sectionContentIds = new List<Guid>();

            var sidebarButtonDownloadModels = SidebarModel.MapContentToSections(sections, contents, sectionContentIds);

            Assert.Empty(sidebarButtonDownloadModels);
            Assert.Empty(sectionContentIds);
        }

        [Fact]
        public void MapContentToSections_WithOneSectionAndOneRelatedContent_ShouldReturnOneSidebarButtonDownloadModelWithCorrectPropertiesAndOneSectionContentId()
        {
            var sections = new List<Section> { SidebarModelTestHelpers.CreateDummySection() };
            var sectionId = SidebarModelTestHelpers.DummySectionId;
            var contents = new List<Domain.Entities.Content> { SidebarModelTestHelpers.CreateDummyContent(sectionId) };
            var expectedSidebarSubButtonPath = $"{SidebarModelTestHelpers.DummySectionPath}/{SidebarModelTestHelpers.DummyContentPath}";
            var sectionContentIds = new List<Guid>();

            var sidebarButtonDownloadModels = SidebarModel.MapContentToSections(sections, contents, sectionContentIds);
            var sidebarButtonDownloadModel = sidebarButtonDownloadModels.FirstOrDefault();
            var sidebarButtonDownloadModelSubButton = sidebarButtonDownloadModel.SubButtons.FirstOrDefault();

            Assert.Single(sidebarButtonDownloadModels);
            Assert.Equal(SidebarModelTestHelpers.DummySectionTitle, sidebarButtonDownloadModel.Title);
            Assert.Equal(SidebarModelTestHelpers.DummySectionPath, sidebarButtonDownloadModel.Path);
            Assert.Single(sidebarButtonDownloadModel.SubButtons);
            Assert.Equal(SidebarModelTestHelpers.DummyContentTitle, sidebarButtonDownloadModelSubButton.Title);
            Assert.Equal(expectedSidebarSubButtonPath, sidebarButtonDownloadModelSubButton.Path);
            Assert.Single(sectionContentIds);
        }

        [Fact]
        public void MapContentToSections_WithOneSectionAndTwoRelatedContents_ShouldReturnOneSidebarButtonDownloadModelWithCorrectPropertiesAndTwoSectionContentId()
        {
            var sections = new List<Section> { SidebarModelTestHelpers.CreateDummySection() };
            var sectionId = SidebarModelTestHelpers.DummySectionId;
            var contents = new List<Domain.Entities.Content> {
                SidebarModelTestHelpers.CreateDummyContent(sectionId),
                SidebarModelTestHelpers.CreateDummyContent(sectionId)
            };
            var expectedSidebarSubButtonPath = $"{SidebarModelTestHelpers.DummySectionPath}/{SidebarModelTestHelpers.DummyContentPath}";
            var sectionContentIds = new List<Guid>();

            var sidebarButtonDownloadModels = SidebarModel.MapContentToSections(sections, contents, sectionContentIds);
            var sidebarButtonDownloadModel = sidebarButtonDownloadModels.FirstOrDefault();
            var sidebarButtonDownloadModelSubButton = sidebarButtonDownloadModel.SubButtons.FirstOrDefault();

            Assert.Single(sidebarButtonDownloadModels);
            Assert.Equal(SidebarModelTestHelpers.DummySectionTitle, sidebarButtonDownloadModel.Title);
            Assert.Equal(SidebarModelTestHelpers.DummySectionPath, sidebarButtonDownloadModel.Path);
            Assert.Equal(2, sidebarButtonDownloadModel.SubButtons.Count);
            Assert.Equal(SidebarModelTestHelpers.DummyContentTitle, sidebarButtonDownloadModelSubButton.Title);
            Assert.Equal(expectedSidebarSubButtonPath, sidebarButtonDownloadModelSubButton.Path);
            Assert.Equal(2, sectionContentIds.Count);
        }

        [Fact]
        public void MapContentToSections_WithOneSectionAndTwoRelatedContentsAndOneUnrelatedContent_ShouldReturnOneSidebarButtonDownloadModelWithCorrectPropertiesAndTwoSectionContentId()
        {
            var sections = new List<Section> { SidebarModelTestHelpers.CreateDummySection() };
            var sectionId = SidebarModelTestHelpers.DummySectionId;
            var contents = new List<Domain.Entities.Content> {
                SidebarModelTestHelpers.CreateDummyContent(sectionId),
                SidebarModelTestHelpers.CreateDummyContent(sectionId),
                SidebarModelTestHelpers.CreateDummyContent()
            };
            var expectedSidebarSubButtonPath = $"{SidebarModelTestHelpers.DummySectionPath}/{SidebarModelTestHelpers.DummyContentPath}";
            var sectionContentIds = new List<Guid>();

            var sidebarButtonDownloadModels = SidebarModel.MapContentToSections(sections, contents, sectionContentIds);
            var sidebarButtonDownloadModel = sidebarButtonDownloadModels.FirstOrDefault();
            var sidebarButtonDownloadModelSubButton = sidebarButtonDownloadModel.SubButtons.FirstOrDefault();

            Assert.Single(sidebarButtonDownloadModels);
            Assert.Equal(SidebarModelTestHelpers.DummySectionTitle, sidebarButtonDownloadModel.Title);
            Assert.Equal(SidebarModelTestHelpers.DummySectionPath, sidebarButtonDownloadModel.Path);
            Assert.Equal(2, sidebarButtonDownloadModel.SubButtons.Count);
            Assert.Equal(SidebarModelTestHelpers.DummyContentTitle, sidebarButtonDownloadModelSubButton.Title);
            Assert.Equal(expectedSidebarSubButtonPath, sidebarButtonDownloadModelSubButton.Path);
            Assert.Equal(2, sectionContentIds.Count);
        }

        [Fact]
        public void MapContentToSections_WithOneSectionAndTwoRelatedContentsAndTwoUnrelatedContents_ShouldReturnOneSidebarButtonDownloadModelWithCorrectPropertiesAndTwoSectionContentId()
        {
            var sections = new List<Section> { SidebarModelTestHelpers.CreateDummySection() };
            var sectionId = SidebarModelTestHelpers.DummySectionId;
            var contents = new List<Domain.Entities.Content> {
                SidebarModelTestHelpers.CreateDummyContent(sectionId),
                SidebarModelTestHelpers.CreateDummyContent(sectionId),
                SidebarModelTestHelpers.CreateDummyContent(),
                SidebarModelTestHelpers.CreateDummyContent()
            };
            var expectedSidebarSubButtonPath = $"{SidebarModelTestHelpers.DummySectionPath}/{SidebarModelTestHelpers.DummyContentPath}";
            var sectionContentIds = new List<Guid>();

            var sidebarButtonDownloadModels = SidebarModel.MapContentToSections(sections, contents, sectionContentIds);
            var sidebarButtonDownloadModel = sidebarButtonDownloadModels.FirstOrDefault();
            var sidebarButtonDownloadModelSubButton = sidebarButtonDownloadModel.SubButtons.FirstOrDefault();

            Assert.Single(sidebarButtonDownloadModels);
            Assert.Equal(SidebarModelTestHelpers.DummySectionTitle, sidebarButtonDownloadModel.Title);
            Assert.Equal(SidebarModelTestHelpers.DummySectionPath, sidebarButtonDownloadModel.Path);
            Assert.Equal(2, sidebarButtonDownloadModel.SubButtons.Count);
            Assert.Equal(SidebarModelTestHelpers.DummyContentTitle, sidebarButtonDownloadModelSubButton.Title);
            Assert.Equal(expectedSidebarSubButtonPath, sidebarButtonDownloadModelSubButton.Path);
            Assert.Equal(2, sectionContentIds.Count);
        }
        #endregion

        # region MapSectionToSidebarButtonDownloadModel
        [Fact]
        public void MapSectionToSidebarButtonDownloadModel_ShouldReturnSidebarButtonDownloadModelWithCorrectProperties()
        {
            var section = SidebarModelTestHelpers.CreateDummySection();

            var sidebarButtonDownloadModel = SidebarModel.MapSectionToSidebarButtonDownloadModel(section);

            Assert.Equal(SidebarModelTestHelpers.DummySectionTitle, sidebarButtonDownloadModel.Title);
            Assert.Equal(SidebarModelTestHelpers.DummySectionPath, sidebarButtonDownloadModel.Path);
        }
        #endregion

        #region MapContentToSidebarButtonModel
        [Fact]
        public void MapContentToSidebarButtonModel_ShouldReturnSidebarButtonDownloadModelWithCorrectProperties()
        {
            var content = SidebarModelTestHelpers.CreateDummyContent();

            var sidebarButtonDownloadModel = SidebarModel.MapContentToSidebarButtonDownloadModel(content);

            Assert.Equal(SidebarModelTestHelpers.DummyContentTitle, sidebarButtonDownloadModel.Title);
            Assert.Equal(SidebarModelTestHelpers.DummyContentPath, sidebarButtonDownloadModel.Path);
        }

        [Fact]
        public void MapContentToSidebarButtonModel_WithSectionPathParameter_ShouldReturnSidebarButtonDownloadModelWithCorrectProperties()
        {
            var expectedSidebarButtonPath = $"{SidebarModelTestHelpers.DummySectionPath}/{SidebarModelTestHelpers.DummyContentPath}";

            var sidebarButtonDownloadModel = SidebarModel.MapContentToSidebarButtonDownloadModel(
                SidebarModelTestHelpers.CreateDummyContent(),
                SidebarModelTestHelpers.DummySectionPath);

            Assert.Equal(SidebarModelTestHelpers.DummyContentTitle, sidebarButtonDownloadModel.Title);
            Assert.Equal(expectedSidebarButtonPath, sidebarButtonDownloadModel.Path);
        }
        #endregion

        #region GetAdminSidebarButtons
        [Fact]
        public void GetAdminSidebarButtons_ShouldReturnSidebarButtonDownloadModelWithFourSubButtons()
        {
            var adminSidebarButtonDownloadModel = SidebarModel.GetAdminSidebarButtons();

            Assert.Equal("Admin", adminSidebarButtonDownloadModel.Title);
            Assert.Equal("admin", adminSidebarButtonDownloadModel.Path);
            Assert.Equal(4, adminSidebarButtonDownloadModel.SubButtons.Count);
        }
        #endregion
    }
}
