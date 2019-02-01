namespace ticketing_api.Data
{
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
    }
}
