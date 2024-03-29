﻿namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking.DiskStoring;

public static class NetworkExtension
{
    public static void Save(this Network thisNetwork, string fileName)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(fileName));
        using (FileStream fileStream = File.Open(fileName, FileMode.Create))
        using (StreamWriter streamWriter = new StreamWriter(fileStream))
        {
            streamWriter.Write(thisNetwork.ToString());
            streamWriter.Flush();
        }
    }
    public static Network Load(string fileName)
    {
        using (FileStream fileStream = File.Open(fileName, FileMode.Open))
        using (StreamReader streamReader = new StreamReader(fileStream))
        {
            string text = streamReader.ReadToEnd();
            Network network = NetworkTextExtension.Parse(text);

            return network;
        }
    }
}
