namespace CruiseDAL.DownMigrators
{
    public interface IDownMigrator
    {
        string CreateCommand(string toDbName, string fromDbName, string cruiseID, string createdBy);
    }
}