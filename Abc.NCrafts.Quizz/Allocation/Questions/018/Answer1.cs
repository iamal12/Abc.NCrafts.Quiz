﻿using System.Collections.Generic;

namespace Abc.NCrafts.Quizz.Questions._018
{
    public class Answer1
    {
        public static void Run()
        {
            GetEmptySequence<int>();
            
            // begin
            GetEmptySequence<int>();
            // end
        }

        private static IEnumerator<T> GetEmptySequence<T>()
        {
            yield break;
        }
    }
}
