public static class Program 
{


    public static void Main(string[] args)
    {
        string filePath;
        //Lex the tokens
        if (args.Length == 0)
        {
            //Console.WriteLine("Missing File to Process");
            //return;
            filePath = "C:\\Code\\Uni\\CompTheory A2\\UTMLang\\bin\\Release\\net10.0\\example.txt";
        }
        else filePath = args[0];

        //all lines soon to be tokens, 1 per line
        string[] contents = File.ReadAllLines(filePath);
        BoxLex box = Lexer.LexFile(contents);

        Interpretor.UTM(box);

    }

}

public class BoxLex 
{
    public Evaluator.ExprNode[] nodes;
    public Lexer.Token[] tokens;
    public int head;

}


