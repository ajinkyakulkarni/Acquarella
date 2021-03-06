﻿namespace Acquarella.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public class LexerConfiguration
    {
        private IList<string> keywords;
        private IList<string> operators;
        private IList<string> linecomments;
        private IList<char> stringdelimeters;
        private State state = State.None;

        private enum State
        {
            None,
            Operators,
            Keywords,
            StringDelimeters,
            LineComments
        }

        public IList<string> Keywords { get { return this.keywords; } }

        public IList<string> Operators { get { return this.operators; } }

        public IList<char> StringDelimeters { get { return this.stringdelimeters; } }

        public IList<string> LineComments { get { return this.linecomments; } }

        public static LexerConfiguration LoadFromFile(string filename)
        {
            var result = new LexerConfiguration();
            result.Load(File.ReadAllLines(filename));
            return result;
        }

        public static LexerConfiguration Load(string name)
        {
            string filename = name + ".txt";

            if (File.Exists(filename))
                return LoadFromFile(filename);

            filename = Path.Combine("Configuration", filename);

            if (File.Exists(filename))
                return LoadFromFile(filename);

            return null;
        }

        private void Load(string[] lines)
        {
            foreach (var item in lines)
            {
                string line = item.Trim();

                if (string.IsNullOrEmpty(line))
                    continue;

                if (line[0] == '#')
                    continue;

                line = line.Replace("\\#", "#");

                if (this.state != State.None && line.Equals("end", StringComparison.InvariantCultureIgnoreCase))
                {
                    this.state = State.None;
                    continue;
                }

                switch (this.state)
                {
                    case State.None:
                        if (line.Equals("operators", StringComparison.InvariantCultureIgnoreCase))
                            this.state = State.Operators;
                        else if (line.Equals("keywords", StringComparison.InvariantCultureIgnoreCase))
                            this.state = State.Keywords;
                        else if (line.Equals("stringdelimeters", StringComparison.InvariantCultureIgnoreCase))
                            this.state = State.StringDelimeters;
                        else if (line.Equals("linecomments", StringComparison.InvariantCultureIgnoreCase))
                            this.state = State.LineComments;
                        else
                            throw new InvalidDataException(string.Format("Invalid Line: '{0}'", line));
                        break;

                    case State.Keywords:
                        string[] words = line.Split(' ', '\t');
                        
                        if (this.keywords == null)
                            this.keywords = new List<string>();

                        this.keywords = this.keywords.Union(words).ToList();
                        break;

                    case State.LineComments:
                        string[] seps = line.Split(' ', '\t');

                        if (this.linecomments == null)
                            this.linecomments = new List<string>();

                        this.linecomments = this.linecomments.Union(seps).ToList();
                        break;

                    case State.Operators:
                        words = line.Split(' ', '\t');

                        if (this.operators == null)
                            this.operators = new List<string>();

                        this.operators = this.operators.Union(words).ToList();
                        break;
                    case State.StringDelimeters:
                        IList<char> chars = line.Split(' ', '\t').Select(n => n[0]).ToList();

                        if (this.stringdelimeters == null)
                            this.stringdelimeters = new List<char>();

                        this.stringdelimeters = this.stringdelimeters.Union(chars).ToList();
                        break;
                }
            }
        }
    }
}
