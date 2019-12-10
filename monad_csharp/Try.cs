using System;
namespace monad_csharp
{
    public class Try<T>
    {
        public Either<Exception, T> Result { get; set; }
        public Try(Func<T> @try)
        {
            try
            {
                Result = new Right<Exception, T>(@try());
            }
            catch (Exception e)
            {
                Result = new Left<Exception, T>(e);
            }
        }
        public Try(Exception exception)
        {
            Result = new Left<Exception, T>(exception);
        }


        public Try<T1> SelectMany<T1>(Func<T, Try<T1>> func)
        {
            var bindResult = Result.Match(
                Left: e => new Try<T1>(e),
                Right: result => func(result));
            return bindResult;

        }

    }

    public abstract class Either<TLeft, TRight>
    {
        public abstract void IfLeft(Action<TLeft> action);
        public abstract void IfRight(Action<TRight> action);
        public abstract Either<TLeft, T1Right> Select<T1Right>(Func<TRight, T1Right> mapping);
        public abstract TResult Match<TResult>(Func<TLeft, TResult> Left, Func<TRight, TResult> Right);


    }
    public class Left<TLeft, TRight> : Either<TLeft, TRight>
    {
        private readonly TLeft value;

        public Left(TLeft left)
        {
            this.value = left;
        }
        public override void IfLeft(Action<TLeft> action)
        {
            action(value);
        }

        public override void IfRight(Action<TRight> action) { }

        public override TResult Match<TResult>(Func<TLeft, TResult> Left, Func<TRight, TResult> Right)
        {
            return Left(value);
        }

        public override Either<TLeft, T1Right> Select<T1Right>(Func<TRight, T1Right> mapping)
        {
            return new Left<TLeft, T1Right>(value);
        }
    }

    public class Right<TLeft, TRight> : Either<TLeft, TRight>
    {
        private readonly TRight value;

        public Right(TRight value)
        {
            this.value = value;
        }
        public override void IfLeft(Action<TLeft> action) { }

        public override void IfRight(Action<TRight> action)
        {
            action(value);
        }

        public override Either<TLeft, T1Right> Select<T1Right>(Func<TRight, T1Right> mapping)
        {
            return new Right<TLeft, T1Right>(mapping(value));
        }
        public override TResult Match<TResult>(Func<TLeft, TResult> Left, Func<TRight, TResult> Right)
        {
            return Right(value);
        }
    }
}
