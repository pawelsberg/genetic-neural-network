namespace Pawelsberg.GeneticNeuralNetworkConsole.Model
{

    public class QuitCommand : Command
    {
        public static string Name = "quit";
        public override string ShortDescription { get { return "Exit the program without saving"; } }
    }
}
