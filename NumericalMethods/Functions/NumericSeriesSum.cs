using System;

namespace NumericalMethods
{
    public static class NumericSeriesSum
    {
        public static double NumberSeriesSum(int initialIndex, int lastIndex, Func<int, double> members)
        {
            double globalSum = 0.0;
            for (int k = initialIndex; k <= lastIndex; k++) globalSum += members(k);
            return globalSum;
        }

        public static double NumberSeriesSumCountingRelativeError (int initialIndex, double delta, 
            Func<int, double> members, ref int finalIndex)
        {
            double initialIndexMember, globalSum = 0.0;
            int initialIndexCopy = initialIndex; 
            bool flag;
            do
            {
                initialIndexMember = members(initialIndexCopy); 
                globalSum += initialIndexMember; 
                flag = Math.Abs(initialIndexMember / globalSum) >= delta; 
                if (flag) initialIndexCopy++;
            } while (flag);
            int membersCount = initialIndexCopy - initialIndex + 1; initialIndexCopy++;
            do
            {
                initialIndexMember = NumberSeriesSum(initialIndexCopy, 
                    initialIndexCopy + membersCount, members);
                globalSum += initialIndexMember; flag = Math.Abs(initialIndexMember / globalSum) >= delta; 
                if (flag) initialIndexCopy = initialIndexCopy + membersCount + 1;
            } while (flag);
            finalIndex = initialIndexCopy + membersCount; 
            return globalSum;
        }

        public static double NumberSeriesSumCountingAbsoluteError (int initialIndex, double error, 
            Func<int, double> members, ref int finalIndex)
        {
            double memberByIndex, firstSum, globalSum = 0.0;
            int index = initialIndex; 
            bool flag;
            do
            {
                memberByIndex = members(index); 
                globalSum += memberByIndex;
                flag = Math.Abs(memberByIndex) >= error; 
                if (flag) index++;
            } while (flag);
            int membersCount = index - initialIndex + 1; index++;
            do
            {
                firstSum = NumberSeriesSum(index, index + membersCount, members);
                globalSum += firstSum; flag = Math.Abs(firstSum) >= error; 
                if (flag) index += membersCount + 1;
            } while (flag);
            finalIndex = index + membersCount; 
            return globalSum;
        }
    }
}
