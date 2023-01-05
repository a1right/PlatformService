using CommandsService.Models;

namespace CommandsService.Data
{
    public interface ICommandRepository
    {
        public bool SaveChanges();

        //Platforms
        public IEnumerable<Platform> GetAllPlatforms();
        public void CreatePlatform(Platform platform);
        public bool IsPlatformExist(int platformId);

        //Commands
        public IEnumerable<Command> GetCommandsForPlatform(int platformId);
        public Command GetCommand(int platformId, int commandId);
        public void CreateCommand(int platformId, Command command);
    }
}
