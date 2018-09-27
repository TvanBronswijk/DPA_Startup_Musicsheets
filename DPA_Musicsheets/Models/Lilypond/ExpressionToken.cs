using System.Linq;
using System.Text.RegularExpressions;

namespace DPA_Musicsheets.Models.Lilypond
{
    class ExpressionToken : Token
    {
        public string Value { get; set; }

        public string Step => Value[0].ToString();
        public int Length => int.Parse(Regex.Match(Value, @"\d+").Value);
        public int Alter => Regex.Matches(Value, "is").Count - Regex.Matches(Value, "es|as").Count;
        public int Dots => Value.Count(c => c.Equals('.'));

        public override void Interpret()
        {
            throw new System.NotImplementedException();
        }
    }
}
