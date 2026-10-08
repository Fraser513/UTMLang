using System.Numerics;
using static System.Numerics.BigInteger;

public static class Interpretor 
{

    public static void UTM(BoxLex box)
    {
        Vars vars = new(box.head);

        for (int t = 0; t < box.tokens.Length;)
        {
            //if below zero set to 0
            if (t < 0) break;
            if (t > box.tokens.Length - 1) break;
            //Send the token of to a handler
            //Console.WriteLine(box.tokens[t].key + (box.tokens[t].key == Lexer.Key.Jump ? " jump value " + box.tokens[t].jump : ""));
            t += Handler.Handle(box.tokens[t], box.nodes, vars);

        }

    }

}

public class Vars
{
    BigInteger[] IntVars;

    public Vars(int head)
    {
        IntVars = new BigInteger[head];
    }

    public BigInteger GetVar(int index)
    {
        return IntVars[index];
    }

    public void SetVar(int index, BigInteger value)
    {
        IntVars[index] = value;
    }

}


