using CMS.Domain.Entities;
using System;

namespace CMS.API.Models.Tests.Helpers
{
    public static class SidebarModelTestHelpers
    {
        public static Guid DummySectionId { get; set; } = Guid.Parse("662483CD-10CA-4CB9-AAE4-AE7990F8ECCF");
        public static string DummySectionTitle { get; set; } = "Dummy Section";
        public static string DummySectionPath { get; set; } = "dummy-section";
        public static string DummyContentTitle { get; set; } = "Dummy Content";
        public static string DummyContentPath { get; set; } = "dummy-content";

        public static Section CreateDummySection()
        {
            return new Section
            {
                Id = DummySectionId,
                Title = DummySectionTitle,
                Path = DummySectionPath,
                CreatedBy = Guid.NewGuid(),
                CreatedOn = DateTime.Now,
                LastUpdatedBy = Guid.NewGuid(),
                LastUpdatedOn = DateTime.Now
            };
        }

        public static Domain.Entities.Content CreateDummyContent(Guid? sectionId = null)
        {
            return new Domain.Entities.Content
            {
                Id = Guid.NewGuid(),
                Title = DummyContentTitle,
                Path = DummyContentPath,
                SectionId = sectionId ?? Guid.NewGuid(),
                CreatedBy = Guid.NewGuid(),
                CreatedOn = DateTime.Now,
                LastUpdatedBy = Guid.NewGuid(),
                LastUpdatedOn = DateTime.Now
            };
        }
    }
}
