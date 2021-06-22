using System.Collections.Generic;

namespace Contents.Migration.Model
{
    public class ContentIntegrationData : IntegrationResponseBase
    {
        public string Status { get; set; }
        public int TotalResults { get; set; }
        public IEnumerable<Article> Articles { get; set; }
    }
}
