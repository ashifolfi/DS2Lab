using DS2Lab.Data;
using Newtonsoft.Json;

namespace DS2Lab;

class Program
{
    public static readonly Version Version = new(1, 0);

    static void DisplayHelp()
    {
        Console.WriteLine("Usage: DS2Lab.exe [INPUT] [OUTPUT] [OPTIONS]");
        Console.WriteLine("Options:");
        Console.WriteLine("-h, --help | Display this message");
        Console.WriteLine("--batch    | Treat input and output as directory for batch conversion");
    }

    static void ConvertFile(List<DiffSingerSegment> inputData, string outputFilepath)
    {
        LabFile labOutput = new();
        
        foreach (var segment in inputData)
        {
            var prevEnd = segment.Offset;
            for (var i = 0; i < segment.PhonemeCount; i++)
            {
                labOutput.Entries.Add(new LabFile.LabEntry(
                    segment.GetPhoneme(i),
                    (long)(prevEnd * 10000000),
                    (long)((prevEnd + segment.GetDuration(i)) * 10000000)
                ));
                prevEnd += segment.GetDuration(i);
            }
        }
        File.WriteAllText(outputFilepath, labOutput.ToString());
    }
    
    static int Main(string[] args)
    {
        Console.WriteLine($"DS2Lab v{Version} - Created by Eden");
        string inputFilename;
        string outputFilename;
        var batchMode = false;

        switch (args.Length)
        {
            case 0:
                DisplayHelp();
                break;
            case 1:
            {
                inputFilename = args[0];
                outputFilename = Path.GetDirectoryName(args[0]) + Path.GetFileNameWithoutExtension(args[0]) + ".lab";

                if (!File.Exists(inputFilename))
                {
                    Console.WriteLine("Specified input does not exist!");
                    return 1;
                }

                var inputFile = File.OpenText(inputFilename);
                var inputData = JsonConvert.DeserializeObject<List<DiffSingerSegment>>(inputFile.ReadToEnd());
                inputFile.Close();

                if (inputData is null)
                {
                    Console.WriteLine("Invalid DiffSinger file!");
                    return 1;
                }

                ConvertFile(inputData, outputFilename);
                break;
            }
            default:
            {
                inputFilename = args[0];
                outputFilename = args[1];
                
                // Command line usage. Unlock full capabilities.
                foreach (var arg in args)
                {
                    switch (arg)
                    {
                        case "-h":
                        case "--help":
                            DisplayHelp();
                            return 0;
                        case "--batch":
                            batchMode = true;
                            break;
                    }
                }

                if (inputFilename == string.Empty || outputFilename == string.Empty)
                {
                    DisplayHelp();
                    return 0;
                }

                if (batchMode)
                {
                    if (!Directory.Exists(inputFilename))
                    {
                        Console.WriteLine("Input directory does not exist!");
                        return 1;
                    }

                    if (!Directory.Exists(outputFilename))
                    {
                        Directory.CreateDirectory(outputFilename);
                    }

                    foreach (var file in Directory.GetFiles(inputFilename))
                    {
                        if (file.EndsWith(".ds"))
                        {
                            var inputFile = File.OpenText(inputFilename);
                            var inputData = JsonConvert.DeserializeObject<List<DiffSingerSegment>>(inputFile.ReadToEnd());
                            inputFile.Close();

                            if (inputData is null)
                            {
                                Console.WriteLine("Invalid DiffSinger file!");
                                return 1;
                            }
                
                            ConvertFile(inputData, outputFilename + Path.GetFileNameWithoutExtension(file) + ".lab");
                        }
                    }
                }
                else
                {
                    if (!File.Exists(inputFilename))
                    {
                        Console.WriteLine("Specified input does not exist!");
                        return 1;
                    }
                
                    var inputFile = File.OpenText(inputFilename);
                    var inputData = JsonConvert.DeserializeObject<List<DiffSingerSegment>>(inputFile.ReadToEnd());
                    inputFile.Close();

                    if (inputData is null)
                    {
                        Console.WriteLine("Invalid DiffSinger file!");
                        return 1;
                    }
                
                    ConvertFile(inputData, outputFilename);
                }
                
                break;
            }
        }

        return 0;
    }
}