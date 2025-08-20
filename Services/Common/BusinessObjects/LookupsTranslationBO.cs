namespace Common.BusinessObjects
{
    public class LookupsTranslationBO : BaseBO
    {
        public int LookupID { get; set; }
        public int LanguageID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int Value { get; set; }
    }
}
