namespace TheatricalPlayersRefactoringKata
{
    public class Performance
    {
        public string PlayID { get; set; }

        public int Audience { get; set; }

        public Performance(string playID, int audience)
        {
            PlayID = playID;
            Audience = audience;
        }

    }
}
