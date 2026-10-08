public static class Lexer
{

    public struct Token
    {
        public Key key;
        public int nodeIndex;
        public int jump;
    }

    public enum Key
    {
        Var,
        Jump,
        Func,
    }

    //comment value
    public const char Comment = '#';
    public const string VarKey = "int";
    public const string JumpKey = "Jump";
    public static readonly HashSet<string> Funcs =
    [
        "Send",


    ];

    public static readonly Dictionary<string, int> FuncMap = new()
    {
        ["Send"] = 0
    };

    public static BoxLex LexFile(string[] args)
    {
        BoxLex lex = new BoxLex();
        List<Token> tokens = [];
        Dictionary<string, int> nameSpace = [];
        List<Evaluator.ExprNode> nodes = [];
        int head = 0;

        foreach (string arg in args) 
        { 
            Token? t = LexLine(arg, nameSpace, nodes, ref head);
            if (t == null) continue;
            tokens.Add((Token)t);
        }

        lex.tokens = tokens.ToArray();
        lex.nodes = nodes.ToArray();
        lex.head = head;
        return lex;
    }

    //everything uses spaces
    //eg
    //int a 7
    //int b 10
    //int c a + b
    //Send c
    //
    public static Token? LexLine(string line, Dictionary<string, int> nameSpace, List<Evaluator.ExprNode> nodes, ref int head)
    {
        Token output = new();

        //remove comments
        int comment = line.IndexOf(Comment);

        if (comment == 0) return null;
        else if (comment != -1) line = line[..comment];

        int split = line.IndexOf(' ');
        if (split == -1) return null;
        string key = line[..split];
        line = line[(split + 1)..];

        if (key == null) return null;

        //definition
        if (key == VarKey)
        {
            //Format
            //int myInt 7
            
            output.key = Key.Var;
            //call the evaluator to get the node index
            //get the variables name
            split = line.IndexOf(' ');
            if (split == -1) return null;
            output.jump = head;
            nameSpace.Add(line[..split], head++);
            line = line[(split + 1)..];
            output.nodeIndex = Evaluator.BuildTree(line, nodes, nameSpace);
        }
        //jump
        else if (key == JumpKey)
        {
            output.key = Key.Jump;
            int index = line.LastIndexOf(' ');
            output.jump = int.Parse(line[(index + 1)..]);
            output.nodeIndex = Evaluator.BuildTree(line[..index], nodes, nameSpace);
        }
        //function definition
        else if (Funcs.Contains(key))
        {
            output.key = Key.Func;
            output.jump = FuncMap[key];
            output.nodeIndex = Evaluator.BuildTree(line, nodes, nameSpace);
        }
        else if (nameSpace.ContainsKey(key))
        {
            //call get node Index
            output.key = Key.Var;
            output.nodeIndex = Evaluator.BuildTree(line, nodes, nameSpace);
            output.jump = nameSpace[key];
        }
        else return null;


        return output;
    }


}

