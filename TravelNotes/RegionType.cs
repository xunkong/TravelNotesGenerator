using System.ComponentModel;

namespace TravelNotesGenerator.TravelNotes
{
    public enum RegionType
    {
        [Description("")]
        None = 0,

        [Description("cn_gf01")]
        cn_gf01 = 1,

        [Description("cn_qd01")]
        cn_qd01 = 5,

        [Description("os_usa")]
        os_usa = 6,

        [Description("os_euro")]
        os_euro = 7,

        [Description("os_asia")]
        os_asia = 8,

        [Description("os_cht")]
        os_cht = 9,

    }



    internal class RegionTypeJsonConverter : JsonConverter<RegionType>
    {
        public override RegionType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetString() switch
            {
                "cn_gf01" => RegionType.cn_gf01,
                "cn_qd01" => RegionType.cn_qd01,
                "os_usa" => RegionType.os_usa,
                "os_euro" => RegionType.os_euro,
                "os_asia" => RegionType.os_asia,
                "os_cht" => RegionType.os_cht,
                _ => RegionType.None,
            };
        }

        public override void Write(Utf8JsonWriter writer, RegionType value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToDescriptionOrString());
        }
    }

}
