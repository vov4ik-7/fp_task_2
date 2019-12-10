using System;

namespace monad_csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Maybe();
            Try();
        }

        static void Try()
        {
            Func<int, Func<int>> func = (v) =>
            {
                var x = v;
                return () => 10 / x;
            };

            Func<int, Try<int>> overallComputation = (int v) => new Try<int>(func(v))
               .SelectMany(result =>
               new Try<int>(() => 10 / result));

            var success = overallComputation(2);
            success.Result.IfLeft((e) => Console.WriteLine(e.Message));
            success.Result.IfRight(result => { Console.WriteLine("succes with result" + result); });

            var fault = overallComputation(0);
            fault.Result.IfLeft((e) => Console.WriteLine(e.Message));
            fault.Result.IfRight(result => { Console.WriteLine("succes with result" + result); });
        }

        static void Maybe()
        {
            Person person = new Person
            {
                Address = new Address
                {
                    HouseName = "House Name"
                }
            };

            var result = HasHouseName(person);
        }

        static bool HasHouseName(Person person)
        {
            return person.With(p => p.Address)
                .With(p => p.HouseName)
                .If(x => x.Length > 4)
                .Do(Console.WriteLine)
                .ReturnSuccess();
        }
    }


    public class Person
    {
        public Address Address { get; set; }
    }

    public class Address
    {
        public string HouseName { get; set; }
    }
}
