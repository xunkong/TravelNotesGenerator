namespace TravelNotesGenerator.TravelNotes;



[JsonSerializable(typeof(UserGameRoleWrapper))]
[JsonSerializable(typeof(UserGameRoleInfo))]
[JsonSerializable(typeof(TravelRecordDetail))]
//[JsonSerializable(typeof(TravelRecordAwardType))]
[JsonSerializable(typeof(TravelRecordAwardType[]))]
[JsonSerializable(typeof(TravelRecordAwardItem))]
[JsonSerializable(typeof(HoyolabBaseWrapper<UserGameRoleWrapper>))]
[JsonSerializable(typeof(HoyolabBaseWrapper<TravelRecordDetail>))]
//[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Metadata)]
internal partial class HoyolabJsonContext : JsonSerializerContext { }


