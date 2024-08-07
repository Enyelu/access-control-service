using access_control.api.Jobs.Interfaces;
using access_control.infrastructure;
using Newtonsoft.Json;

namespace access_control.api.Jobs.Implementations
{
    public class LockJobs : ILockJobs
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<LockJobs> _logger;
        public LockJobs(ApplicationContext context, ILogger<LockJobs> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<string> CheckAndCloseOpenDoors()
        {
            _logger.LogInformation($"Starting automatic locks closure at {DateTime.UtcNow} UTC time");
            
            var now = DateTime.UtcNow;
            var locksToClose = _context.Locks
                .Where(x => x.OpenedAt.HasValue)
                .AsEnumerable()
                .Where(x => (now - x.OpenedAt.Value).TotalMinutes > 5)
                .ToList();

            _logger.LogInformation($"Found {locksToClose.Count} openned lock(s) at {DateTime.UtcNow} UTC time");

            if (!locksToClose.Any())
                return "No doors/locks to close";

            var faileLockAttempt = new List<string>();
            foreach (var lockToClose in locksToClose)
            {
                bool apiCallToMicrocontroller = true; //Simulating value to be true
                //Make API call to open lock (typically a micro controller such as nanoframework that opens the lock)

                //If lock failed to close then
                if (!apiCallToMicrocontroller)
                {
                    faileLockAttempt.Add(lockToClose.Id);
                    continue;
                }

                //If lock closed successfully then
                lockToClose.IsOpen = false;
                lockToClose.OpenedAt = null;
            }

            if(faileLockAttempt.Any())
                _logger.LogInformation($"Automatic door closure failed for these locks {JsonConvert.SerializeObject(faileLockAttempt)} at {DateTime.UtcNow} UTC time");

            _context.UpdateRange(locksToClose);
            await _context.SaveChangesAsync();

            return "Done";
        }
    }
}
