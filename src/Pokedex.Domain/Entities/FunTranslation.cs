namespace Pokedex.Domain.Entities
{
    public class FunTranslation
    {
        public Success success { get; set; }
        public Content contents { get; set; }
    }

    public class Success
    {
        public int total { get; set; }
    }

    public class Content
    {
        public string translated { get; set; }
        public string text { get; set; }
        public string translation { get; set; }
    }
}