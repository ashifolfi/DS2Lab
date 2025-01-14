using Newtonsoft.Json;

namespace DS2Lab.Data;

public class DiffSingerSegment
{
    [JsonProperty("offset")] public float Offset { get; set; }
    [JsonProperty("ph_seq")] public string PhonemeSequence { get; set; } = "";
    [JsonProperty("ph_dur")] public string PhonemeDuration { get; set; } = "";

    public float PhonemeCount => PhonemeSequence.Split(" ").Length;
    
    public float GetDuration(int index)
    {
        return float.Parse(PhonemeDuration.Split(" ")[index]);
    }

    public string GetPhoneme(int index)
    {
        return PhonemeSequence.Split(" ")[index];
    }
}