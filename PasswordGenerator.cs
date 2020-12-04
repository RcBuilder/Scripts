using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Common
{
    /*  USING
        var pg = new PasswordGenerator();
        pg.Policy = "[0-9]{1,}[a-zA-Z]{1,} | [a-zA-Z]{1,}[0-9]{1,}"; // at least 1 char and 1 digit 
        Console.WriteLine(pg.Generate()); 
        -
        var pg = new PasswordGenerator();        
        Console.WriteLine(pg.Generate()); 
        -
        var pg = new PasswordGenerator(10);
        var newPassword = pg.Generate();
    */
    public class PasswordGenerator
    {        
        private char[] data = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*".ToCharArray();
        private byte policy_attempts = 4;
        private Random rnd;     
        
        public int Length { set; get; }
        public string Policy { set; get; }

        public PasswordGenerator() : this(6) { }
        public PasswordGenerator(int Length) : this(Length, string.Empty) { }
        public PasswordGenerator(int Length, string Policy)
        {
            this.Length = Length;
            this.Policy = Policy; //empty = no policy inforcement
            this.rnd = new Random();
        }
        
        public string Generate() { return Generate(1); }
        public string Generate(byte attemt)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < this.Length; i++)
                sb.Append(data[this.rnd.Next(data.Length)]);
            string password = sb.ToString();

            bool is_valid = this.Policy == string.Empty || Regex.IsMatch(password, this.Policy, RegexOptions.IgnorePatternWhitespace);
            if (!is_valid && attemt <= this.policy_attempts)
                Generate(++attemt);
            return password;
        }
    }
}
