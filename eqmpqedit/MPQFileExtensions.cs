namespace eqmpqedit
{
    public class MPQFileExtensions
    {
        public static string getFileExtensionType(string extension)
        {
            extension = extension.ToLower();

            if (extension == ".dun")
                return "Diablo 1 dungeon file (*.dun)";
            if (extension == ".cel")
                return "Diablo 1 graphics file (*.cel)";
            if (extension == ".cl2")
                return "Diablo 1 advanced graphics file (*.cl2)";
            if (extension == ".wav")
                return "Wave sound file (*.wav)";
            if (extension == ".d2s")
                return "Diablo 2 character file (*.d2s)";
            if (extension == ".sol")
                return "Diablo 1 automap file (*.sol)";
            if (extension == ".bin")
                return "Diablo 2 binary file (*.bin)";
            if (extension == ".txt")
                return "Text file (*.txt)";
            if (extension == ".dt1")
                return "Diablo 2 level file (*.dt1)";
            if (extension == ".dc6")
                return "Diablo 2 graphics file (*.dc6)";
            if (extension == ".pal")
                return "Palette file (*.pal)";
            if (extension == ".mpq")
                return "MPQ archive (*.mpq)";
            if (extension == ".smk")
                return "Smacker video file (*.smk)";
            if (extension == ".mp3")
                return "MPEG Audio File (*.mp3)";
            if (extension == ".j")
                return "JASS (*.j)";

            return "Unknown file extension";
        }
    }
}
