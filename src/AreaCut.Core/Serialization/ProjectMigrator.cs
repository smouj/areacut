using AreaCut.Core.ProjectModel;
using System;

namespace AreaCut.Core.Serialization;

/// <summary>
/// Migrates .areacut project files from older format versions to the current version.
/// Each migration step is explicit and reversible-aware.
/// </summary>
public static class ProjectMigrator
{
    /// <summary>Migrate a project document to the current format version.</summary>
    public static ProjectDocument Migrate(ProjectDocument document)
    {
        if (document.Version > ProjectSerializer.CurrentFormatVersion)
            throw new InvalidOperationException(
                $"Project version {document.Version} is newer than supported version {ProjectSerializer.CurrentFormatVersion}. " +
                "Please update AreaCut.");

        // Future migrations go here:
        // if (document.Version < 2) document = MigrateV1ToV2(document);
        // if (document.Version < 3) document = MigrateV2ToV3(document);

        document.Version = ProjectSerializer.CurrentFormatVersion;
        return document;
    }
}
