namespace Abundance_Nk.Model.Model
{
    public class OLevelResultDetail
    {
        public long Id { get; set; }
        public OLevelResult Header { get; set; }
        public OLevelSubject Subject { get; set; }
        public OLevelGrade Grade { get; set; }
    }
}