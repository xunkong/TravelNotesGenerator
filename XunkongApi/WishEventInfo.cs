namespace Xunkong.Core.Wish
{
    public class WishEventInfo
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public string Version { get; set; }

        [JsonIgnore]
        public DateTimeOffset StartTime => TimeStringToTimeOffset(_StartTimeString);

        [JsonIgnore]
        public DateTimeOffset EndTime => TimeStringToTimeOffset(_EndTimeString);


        [JsonPropertyName("StartTime")]
        public string _StartTimeString { get; set; }


        [JsonPropertyName("EndTime")]
        public string _EndTimeString { get; set; }

        public List<string> Rank5UpItems { get; set; }

        public List<string> Rank4UpItems { get; set; }


        [JsonIgnore]
        public static string RegionType { get; set; }


        private static DateTimeOffset TimeStringToTimeOffset(string str)
        {
            if (str.Contains("+"))
            {
                return DateTimeOffset.Parse(str);
            }
            else
            {
                var offset = RegionType switch
                {
                    "os_usa" => "-05:00",
                    "os_euro" => "+01:00",
                    _ => "+08:00",
                };
                return DateTimeOffset.Parse($"{str} {offset}");
            }
        }


    }
}
