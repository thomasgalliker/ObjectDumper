﻿namespace System.Diagnostics
{
    public struct DumpOptions
    {
        private int? indentSize;
        private char? indentChar;
        private string lineBreakChar;
        private bool? setPropertiesOnly;

        public int IndentSize
        {
            get { return this.indentSize ?? 2; }
            set { this.indentSize = value; }
        }

        public char IndentChar
        {
            get => this.indentChar ?? ' ';
            set => this.indentChar = value;
        }

        public string LineBreakChar
        {
            get => this.lineBreakChar ?? "\n\r";
            set => this.lineBreakChar = value;
        }

        public bool SetPropertiesOnly
        {
            get => this.setPropertiesOnly ?? false;
            set => this.setPropertiesOnly = value;
        }
    }
}