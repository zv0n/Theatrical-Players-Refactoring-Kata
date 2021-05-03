namespace TheatricalPlayersRefactoringKata
{
    public class Play
    {
        public string Name { get; set; }

        public PlayType Type { get; set; }

        public Play(string name, PlayType type) {
            this.Name = name;
            this.Type = type;
        }
    }
}
