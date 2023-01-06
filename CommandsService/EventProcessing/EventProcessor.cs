using System.Text.Json;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;

namespace CommandsService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }
        public void ProcessEvent(string message)
        {
            var eventType = DetermineEventType(message);
            switch (eventType)
            {
                case EventType.PlatformPublished:
                    AddPlatform(message);
                    break;
                default:
                    break;
            }
        }

        private void AddPlatform(string platformPublishedMessage)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<ICommandRepository>();
                var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);
                try
                {
                    var plat = _mapper.Map<Platform>(platformPublishedDto);
                    if (!repository.ExternPlatformExist(plat.ExternalId))
                    {
                        repository.CreatePlatform(plat);
                        repository.SaveChanges();
                        Console.WriteLine("--> Platform added");
                    }
                    else
                    {
                        Console.WriteLine("--> Platform already exists...");
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"Could not add platform to DB: {exception.Message}");
                }
            }
        }
        private EventType DetermineEventType(string notificationMessage)
        {
            Console.WriteLine($"--> Determening event: {notificationMessage}");
            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);
            var result = eventType.Event switch
            {
                "PlatformPublished" => EventType.PlatformPublished,
                _ => EventType.Undeterminded
            };
            var logResponse = result switch
            {
                EventType.PlatformPublished => "--> Platform published event detected",
                EventType.Undeterminded => "--> Could not determine event type"
            };
            Console.WriteLine(logResponse);
            return result;
        }
    }

    enum EventType
    {
        Undeterminded,
        PlatformPublished,
        
    }
}
