using AutoMapper;
using CommandsService.Models;
using Grpc.Net.Client;
using PlatformService;

namespace CommandsService.SyncDataServices.Grpc
{
    public class PlatformDataClient : IPlatformDataClient
    {
        private readonly IConfiguration _confituration;
        private readonly IMapper _mapper;

        public PlatformDataClient(IConfiguration confituration, IMapper mapper)
        {
            _confituration = confituration;
            _mapper = mapper;
        }
        public IEnumerable<Platform> ReturnAllPlatforms()
        {
            Console.WriteLine($"--> Calling gRPC Service: {_confituration["GrpcPlatform"]}");
            var channel = GrpcChannel.ForAddress(_confituration["GrpcPlatform"]);
            var client = new GrpcPlatform.GrpcPlatformClient(channel);
            var request = new GetAllRequest();

            try
            {
                var reply = client.GetAllPlatforms(request);
                return _mapper.Map<IEnumerable<Platform>>(reply.Platform);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"--> Could not call gRPC Server: {exception.Message}");
                return new List<Platform>();
            }
        }
    }
}
