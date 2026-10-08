using System.Numerics;

public static class Handler 
{

    public static int Handle(Lexer.Token t, Evaluator.ExprNode[] nodes, Vars var)
    {
        switch (t.key)
        {
            case Lexer.Key.Var:
                return HandleVar(t, nodes, var);
            case Lexer.Key.Jump:
                return HandleJump(t, nodes, var);
            case Lexer.Key.Func:
                return HandleFunc(t, nodes, var);
            default:
                break;
        }
        return 1;
    }

    public static int HandleVar(Lexer.Token t, Evaluator.ExprNode[] nodes, Vars var)
    {
        var.SetVar(t.jump, Evaluator.Compute(t.nodeIndex, nodes, var));
        return 1;
    }

    public static int HandleJump(Lexer.Token t, Evaluator.ExprNode[] nodes, Vars var)
    {
        BigInteger result = Evaluator.Compute(t.nodeIndex, nodes, var);
        return result > 0 ? t.jump : 1;
    }

    public static int HandleFunc(Lexer.Token t, Evaluator.ExprNode[] nodes, Vars var)
    {
        //call the func
        funcs[t.jump](Evaluator.Compute(t.nodeIndex, nodes, var));
        return 1;
    }

    static Action<BigInteger>[] funcs =
    [
        Send,
    ];

    public static void Send(BigInteger arg)
    {
        Console.WriteLine(arg);
    }
}


