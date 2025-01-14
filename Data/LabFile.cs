namespace DS2Lab.Data;

public class LabFile
{
    public struct LabEntry(string phoneme, long startTime, long endTime)
    {
        public string Phoneme = phoneme;
        public long StartTime = startTime;
        public long EndTime = endTime;

        public override string ToString()
        {
            return $"{StartTime} {EndTime} {Phoneme}";
        }
    }
    
    public List<LabEntry> Entries { get; set; } = [];

    public override string ToString()
    {
        var labStr = "";
        foreach (var entry in Entries)
        {
            labStr += $"{entry}\n";
        }

        return labStr;
    }
}