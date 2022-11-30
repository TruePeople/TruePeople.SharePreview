using Microsoft.Extensions.Logging;
using System;
using TruePeople.SharePreview.Models;
using Umbraco.Cms.Infrastructure.Migrations;

namespace TruePeople.SharePreview.Migrations
{
    internal class SetupSettingsTable : MigrationBase
    {
        public SetupSettingsTable(IMigrationContext context) : base(context)
        {
        }

        protected override void Migrate()
        {
            Logger.LogDebug("Running migration {MigrationStep}", nameof(SetupSettingsTable));

            if (TableExists(nameof(ShareablePreviewSettings)) == false)
            {
                Create
                    .Table<ShareablePreviewSettings>()
                    .Do();

                Insert
                    .IntoTable(nameof(ShareablePreviewSettings))
                    .Row(new { privateKey = Guid.NewGuid().ToString(), notValidUrl = "/" })
                    .Do();
            }
            else
            {
                Logger.LogDebug("The database table {DbTable} already exists, skipping", nameof(ShareablePreviewSettings));
            }
        }
    }
}