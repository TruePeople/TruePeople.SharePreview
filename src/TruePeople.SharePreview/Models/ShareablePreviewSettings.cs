using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace TruePeople.SharePreview.Models
{
    [TableName("ShareablePreviewSettings")]
    [PrimaryKey("id", AutoIncrement = true)]
    [ExplicitColumns]
    public class ShareablePreviewSettings
    {
        [Column("id")]
        [PrimaryKeyColumn(AutoIncrement = true, IdentitySeed = 1)]
        public int Id { get; set; }

        [Column("privateKey")]
        public string PrivateKey { get; set; }

        [Column("notValidUrl")]
        public string NotValidUrl { get; set; }
    }
}