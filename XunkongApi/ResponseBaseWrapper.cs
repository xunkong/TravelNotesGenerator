using Xunkong.Core.Metadata;
using Xunkong.Core.Wish;

namespace Xunkong.Core.XunkongApi
{
    public class ResponseBaseWrapper
    {
        public int Code { get; set; }

        public string? Message { get; set; }

        public MetadataDto? Data { get; set; }

    }




    [JsonSerializable(typeof(ResponseBaseWrapper))]
    [JsonSerializable(typeof(MetadataDto))]
    [JsonSerializable(typeof(WishEventInfo[]))]
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    public partial class XunkongJsonContext : JsonSerializerContext { }


    [JsonSerializable(typeof(TravelNoteAsset))]
    public partial class AssetJsonContext : JsonSerializerContext { }


}
