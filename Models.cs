public class TravelNoteAsset
{
    public string Background { get; set; }

    public List<string> Emotions { get; set; }

}



public record Stats(string Name, int Number)
{
    public float Percent { get; set; }
}
