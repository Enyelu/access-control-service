namespace access_control.api.Jobs.Interfaces
{
    public interface ILockJobs
    {
        Task<string> CheckAndCloseOpenDoors();
    }
}