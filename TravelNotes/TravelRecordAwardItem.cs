namespace TravelNotesGenerator.TravelNotes
{

    /// <summary>
    /// 旅行记录原石或摩拉获取记录
    /// </summary>
    public class TravelRecordAwardItem
    {
        [JsonIgnore]
        public int Id { get; set; }

        public int Uid { get; set; }

        [JsonIgnore]
        public int Year { get; set; }

        [JsonIgnore]
        public int Month { get; set; }

        [JsonIgnore]
        public TravelRecordAwardType Type { get; set; }


        [JsonPropertyName("action_id")]
        public int ActionId { get; set; }


        [JsonPropertyName("action")]
        public string? ActionName { get; set; }


        [JsonPropertyName("time"), JsonConverter(typeof(DataTimeJsonConverter))]
        public DateTime Time { get; set; }


        [JsonPropertyName("num")]
        public int Number { get; set; }



    }


    internal class DataTimeJsonConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.Parse(reader.GetString()!);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}
