namespace TravelNotesGenerator.TravelNotes
{
    public class HoyolabException : Exception
    {

        public int Retcode { get; init; }


        public HoyolabException() { }

        public HoyolabException(int retcode, string? message) : base($"{message} ({retcode})")
        {
            Retcode = retcode;
        }
    }
}
