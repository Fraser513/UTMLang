using System.Numerics;

public static class Evaluator
{
    public struct ExprNode 
    {
        public bool isVar;
        public Op op;
        public BigInteger index;
        public int indexRight;
    }

    public enum Op : byte
    {
        Nul,
        Pow,
        Mul, Div, Mod,
        Add, Sub,
        Less, Gre, Equ,
        And,
        Or,

    }

    public static readonly HashSet<char> Ops =
    [
        '^',
        '*', '/', '%',
        '+', '-',
        '<', '>', '=',
        '&',
        '|'
    ];

    public static readonly char[][] OpsPrecedence =
    [
         ['^'],
         ['*', '/', '%'], //mult, div
         ['+', '-'],          // addition, subtraction
         ['<', '>'],
         ['='],
         ['&'],   //&&
         ['|'],   //||
    ];

    public static readonly Op[] CharToOp = BuildCharToOp();

    private static Op[] BuildCharToOp()
    {
        var arr = new Op[128];
        arr['^'] = Op.Pow;
        arr['*'] = Op.Mul;
        arr['/'] = Op.Div;
        arr['%'] = Op.Mod;
        arr['+'] = Op.Add;
        arr['-'] = Op.Sub;
        arr['<'] = Op.Less;
        arr['>'] = Op.Gre;
        arr['='] = Op.Equ;
        arr['&'] = Op.And;
        arr['|'] = Op.Or;
        return arr;
    }


    public static int BuildTree(string expression, List<ExprNode> nodes, Dictionary<string, int> nameSpace)
    {
        expression = expression.Trim();
        if (string.IsNullOrEmpty(expression)) return -1;
        ExprNode node;
        try
        {
            node = BuildNode(expression, nodes, nameSpace);
        }
        catch (Exception e)
        {
            Console.WriteLine("ERROR: \n\t EXPRESSION " + expression + "\n\n" + e.Message);
            Console.ReadKey();
            throw;
        }
        int rootIdx = nodes.Count;
        nodes.Add(node);
        return rootIdx;
    }

    private static ExprNode BuildNode(string expression, List<ExprNode> nodes,  Dictionary<string, int> nameSpace)
    {
        ExprNode result = new();

        expression = expression.Trim();
        // 1. strip outer parentheses while present
        while (expression[0] == '(' && expression[^1] == ')') expression = expression[1..^1];

        // 2. early out on empty expression
        if (string.IsNullOrEmpty(expression))
        {
            throw new Exception("Should not be here, error caused by string \"" + expression + "\"");
        } //return early;

        // 3. unary prefix rewrite ('-' -> "-1*", other op -> "1*")
        if (Ops.Contains(expression[0]))
            expression = (expression[0] == '-') ? expression : expression[1..];


        // 4. find next operator outside parens (FindNextSymbol)
        int symbolIndex = FindNextSymbolLowest(expression);

        if (symbolIndex == -1 && expression[0] == '-' && !IsNumber(expression))
            return BuildNode("-1 * " + expression[1..], nodes, nameSpace);

        if (symbolIndex != -1)
        {
            string left = expression[..symbolIndex].Trim();
            string right = expression[(symbolIndex + 1)..].Trim();

            ExprNode leftNode = BuildNode(left, nodes, nameSpace);
            ExprNode rightNode = BuildNode(right, nodes, nameSpace);

            //callapse known solution
            if (leftNode.op == Op.Nul && rightNode.op == Op.Nul)
            {
                //combine
                //result = the combined of the 2
                //(result.IndexLeft, result.Type) = NodeEvaluator.EvalNode((leftNode.OpSub, leftNode.IndexLeft), (rightNode.OpSub, rightNode.IndexLeft), CharToOp[expression[symbolIndex]], lexVars);
                //if (IsLiteral(result.Type)) result.OpSub = (byte)result.Type;
                //else Console.WriteLine("Error with combine");
                //return result;
            }

            //make result hold the operator and the left and right index
            result.index = nodes.Count;
            nodes.Add(leftNode);
            result.indexRight = nodes.Count;
            nodes.Add(rightNode);
            result.op = CharToOp[expression[symbolIndex]];
            return result;


        }

        //this find type is ok since we are still using user input Eg strings are surrounded with ""
        result.isVar = nameSpace.ContainsKey(expression);

        //get function address
        //call build node for children
        if (result.isVar)
        {
            int temp = nameSpace[expression];
            result.index = temp;
        } else
        {
            result.op = Op.Nul;
            result.index = int.Parse(expression);
        }
        return result;
    }

    //Helpers Build Node -----------------

    public static bool IsNumber(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return false;

        return long.TryParse(s, out _) || double.TryParse(s, out _);
    }

    public static int FindNextSymbolLowest(string expression, bool doBrackets = true)
    {
        int parenthesesCount = 0;
        int[] foundIdexs = [.. Enumerable.Repeat(-1, OpsPrecedence.Length)];

        for (int i = 0; i < expression.Length; i++)
        {
            char currentChar = expression[i];

            if (doBrackets && currentChar == '(') parenthesesCount++;
            else if (doBrackets && currentChar == ')') parenthesesCount--;
            else if (parenthesesCount == 0)
            {
                // Check each precedence group
                for (int level = 0; level < OpsPrecedence.Length; level++)
                {
                    if (OpsPrecedence[level].Contains(currentChar))
                    {
                        if (currentChar == '-')
                        {
                            int p = i - 1;
                            while (p >= 0 && expression[p] == ' ') p--;
                            if (p < 0 || OpsPrecedence.Any(op => op.Contains(expression[p]))) continue; // Skip unary minus
                        }
                        if (level == 1 ? foundIdexs[level] == -1 : true) foundIdexs[level] = i;
                    }
                }
            }
        }

        for (int level = OpsPrecedence.Length - 1; level >= 0; level--)
        {
            if (foundIdexs[level] != -1)
                return foundIdexs[level];
        }

        return -1;
    }


    //Live Eval -------------------------

    public static ExprNode CombineNode(Op op, int leftIndex, int rightIndex, ExprNode[] nodes,  Vars vars)
    {
        ExprNode result = new();
        ExprNode leftNode = nodes[leftIndex];
        ExprNode rightNode = nodes[rightIndex];

        //if both are ops we have to save the result of the left before collapsing the right to avoid losing the information we solved

        leftNode = CollapseNode(leftNode, nodes, vars);
        rightNode = CollapseNode(rightNode, nodes, vars);

        BigInteger r = opMatrix[(int)op - 1](leftNode.index, rightNode.index);
        result.op = Op.Nul;
        result.isVar = false;
        result.index = r;
        return result;
    }

    private static ExprNode CollapseNode(ExprNode node, ExprNode[] nodes,  Vars vars)
    {
        if (node.op != Op.Nul) return CombineNode(node.op, (int)node.index, (int)node.indexRight, nodes, vars);
        if (node.isVar)
        {
            node.isVar = false;
            node.op = Op.Nul;
            node.index = vars.GetVar((int)node.index);
        }
        return node;
    }

    public static BigInteger Compute(int nodeIndex, ExprNode[] nodes, Vars vars)
    {

        ExprNode rootNode = nodes[nodeIndex];
        if (rootNode.op != Op.Nul || rootNode.isVar) rootNode = CollapseNode(rootNode, nodes, vars);
        return rootNode.index;
    }

    public delegate BigInteger FuncImpl(BigInteger a, BigInteger b);
    public static readonly FuncImpl[] opMatrix = new FuncImpl[11]
    {
        (a, b) => { return BigInteger.Pow(a, (int)b); },    // ^
        (a, b) => { return a * b; },    // *
        (a, b) => { return a / b; },    // /
        (a, b) => { return a % b; },    // %
        (a, b) => { return a + b; },    // +
        (a, b) => { return a - b; },    // -
        (a, b) => { return (a < b) ? 1 : 0; },   // <
        (a, b) => { return (a > b) ? 1 : 0; },   // >
        (a, b) => { return (a == b) ? 1 : 0; },   // =
        (a, b) => { return (a >= 1 && b >= 1) ? 1 : 0; }, // &&
        (a, b) => { return (a >= 1 || b >= 1) ? 1 : 0; }, // ||
    };
}

