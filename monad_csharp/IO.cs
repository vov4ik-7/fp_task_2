using System;
namespace monad_csharp
{
    // An operation is a pure function which takes the result of
    // the previous IO action and returns the IO action that
    // should be evaluated next.
    internal delegate IOAction Operation(string input);

    interface IOAction
    {
        // Bind another operation on to this IOAction
        IOAction bind(Operation operation);
        // Create a plain IO operation that evaluates to 'text'
        IOAction wrap(string text);
    }

    interface Runtime
    {
        // An IO action which reads a line of input from the user.
        IOAction getLine();
        // An IO action which writes the specified line of text to the screen.
        IOAction putStrLn(string text);
    }

    class FunctionalProgram
    {
        public static readonly Runtime rt = RuntimeImpl.Instance;

        private static IOAction checkInput(string input)
        {
            int num;
            if (int.TryParse(input, out num) && num == 4)
                return rt.putStrLn("That's the right answer!");
            else
                return rt.putStrLn(string.Format("{0} sorry, we're not in Orwell's novel 1984. Please try again...", input)).bind(ask);
        }

        private static IOAction ask(string input)
        {
            return rt.putStrLn("What is 2 + 2?").
                bind(unused => rt.getLine()).
                bind(checkInput);
        }

        public static IOAction Main =
            rt.putStrLn("Enter your name").
                bind(unused => rt.getLine()).
                bind(name => rt.putStrLn("Hello " + name + ". It's nice to meet you.")).
                bind(unused => rt.putStrLn("OK time for a little test...")).
                bind(ask);
    }


    abstract class RuntimeAction : IOAction
    {
        public IOAction bind(Operation operation)
        {
            return new CombinedAction(this, operation);
        }

        public IOAction wrap(string text)
        {
            return new Wrapped(text);
        }

        public abstract string perform();
    }

    class ReadLine : RuntimeAction
    {
        public override string perform()
        {
            return Console.ReadLine();
        }
    }

    class WriteLine : RuntimeAction
    {
        readonly string text;
        public WriteLine(string text)
        {
            this.text = text;
        }
        public override string perform()
        {
            Console.WriteLine(text);
            return "";
        }
    }

    class Wrapped : RuntimeAction
    {
        readonly string text;
        public Wrapped(string text)
        {
            this.text = text;
        }
        public override string perform()
        {
            return text;
        }
    }

    class CombinedAction : RuntimeAction
    {
        readonly RuntimeAction first;
        readonly Operation next;
        public CombinedAction(RuntimeAction first, Operation next)
        {
            this.first = first;
            this.next = next;
        }

        public override string perform()
        {
            var result = first.perform();
            var nextAction = next(result);
            return ((RuntimeAction)nextAction).perform();
        }
    }

    class RuntimeImpl : Runtime
    {
        private readonly IOAction readLn = new ReadLine();

        public IOAction getLine()
        {
            return readLn;
        }

        public IOAction putStrLn(string text)
        {
            return new WriteLine(text);
        }

        public static Runtime Instance = new RuntimeImpl();
    }
}
