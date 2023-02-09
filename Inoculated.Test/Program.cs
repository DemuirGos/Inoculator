﻿using System;
using System.Collections;
namespace Program {
    class Program : Printable<Program> {
        public string Name {get; set;} = "Program";
        async static Task Main(string[] args) {
            _ = (new Program()).SumIntervalIsEven(7, 23, out _);
        }
        

        [Date, LogEntrency]
        public bool SumIntervalIsEven(int start, int end, out int r) {
            int result = 0;
            for (int j = start; j < end; j++) {
                result += j;
            }
            r = result;
            Console.WriteLine(Name);
            return r % 2 == 0;
        }
    }
}