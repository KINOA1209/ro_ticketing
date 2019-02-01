namespace ticketing_api.Models
{
    public class LogUpdate
    {
        public string FieldName { get; set; }

        public object Old { get; set; }

        public object New { get; set; }
    }
}
