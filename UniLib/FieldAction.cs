using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Gianos.UniLib
{
    /// <summary>
    /// Class modeling an action to be performed on a field.
    /// </summary>
    public class FieldAction
    {
        /// <summary>
        /// Name of the table containing the field
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Name of the field to be updated
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Name of the State (Unicode or Text) to be set on the field
        /// </summary>
        public FieldState NewState { get; set; }

        /// <summary>
        /// New size to which the field must be set. 0 means "as is".
        /// </summary>
        public int NewSize { get; set; }

        /// <summary>
        /// Link to the Instance of the field information
        /// </summary>
        public FieldInformation FieldInfo { get; set; }

        public FieldAction()
        {
            this.TableName = this.FieldName = String.Empty;
            this.NewState = FieldState.Unspecified;
            this.NewSize = 0;
            this.FieldInfo = null;
        }

        /// <summary>
        /// ToString method.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (String.IsNullOrEmpty(TableName) || String.IsNullOrEmpty(FieldName))
            {
                return String.Empty;
            }

            return String.Format((NewSize > 0) ? "{0}.{1} -> {2}({3})" : "{0}.{1} -> {2}",
                TableName, FieldName, NewState.ToString(), NewSize);
        }

        static private Regex _ParseLine = new Regex(
            @"(\#[^\r\n]*[\r\n]+)*\s*          # ignore comment lines starting from # and ending with newline
            (?<TableName>[a-zA-Z0-9_]+)        # matches table name
            \s*\.\s*                           # the dot between table and field name and ignores whitespace
            (?<FieldName>[a-zA-Z0-9_]+)        # matches the field name
            \s*->\s*                           # the arrow sign
            (?<NewState>(Unicode)|(Ansi))\s*   # Matches state: Unicode or Ansi
            (\(\s*(?<NewSize>[0-9]+)\)\s*)?    # the new size, if present",
            RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
        
        /// <summary>
        /// Parses a string and returns a FieldAction element
        /// </summary>
        /// <param name="inputText"></param>
        /// <returns></returns>
        public static IList<FieldAction> Parse(string inputText)
        {
            var a = from Match aMatch in _ParseLine.Matches(inputText)
                    select new FieldAction()
                    {
                        TableName = aMatch.Groups["TableName"].Value.ToUpper(),
                        FieldName = aMatch.Groups["FieldName"].Value.ToUpper(),
                        NewState = GetNewStateFromMatch(aMatch),
                        NewSize = GetNewSizeFromMatch(aMatch)
                    };

            return a.ToList<FieldAction>();
        }

        private static int GetNewSizeFromMatch(Match aMatch)
        {
            int newSize = 0;

            if (aMatch.Groups["NewSize"].Success && Int32.TryParse(aMatch.Groups["NewSize"].Value, out newSize))
            {
                return newSize;
            }

            return 0;
        }

        private static FieldState GetNewStateFromMatch(Match aMatch)
        {
            var newStateStr = aMatch.Groups["NewState"].Value.ToLower();

            if (newStateStr == "unicode") return FieldState.Unicode;
            if (newStateStr == "ansi") return FieldState.Ansi;

            throw new Exception("An unexpected error occurred parsing the actions: found " +
                newStateStr + " but expected Unicode or Ansi.");
        }
    }
}
