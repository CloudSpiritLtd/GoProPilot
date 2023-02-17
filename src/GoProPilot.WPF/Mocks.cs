using System.IO;

namespace GoProPilot;

public class Mocks
{
    public Mocks()
    {
        if (File.Exists("Mock-Media_List.json"))
        {
            using StreamReader sr = new("Mock-Media_List.json");
            Media_List = sr.ReadToEnd();
            sr.Close();
        }
        else
        {
            Media_List = "";
        }
    }

    public string Media_List { get; }
}
