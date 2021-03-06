﻿using System;

namespace csly.whileLang.compiler
{
    internal class ByteCodeGenerationException : Exception
    {
        private string v;
        private object compilationResult;

        public ByteCodeGenerationException()
        {
        }

        public ByteCodeGenerationException(string message) : base(message)
        {
        }

        public ByteCodeGenerationException(string v, object compilationResult)
        {
            this.v = v;
            this.compilationResult = compilationResult;
        }

        public ByteCodeGenerationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}