// See https://aka.ms/new-console-template for more information
using System.Text.RegularExpressions;

var check = new LispValidator();

Console.WriteLine("Availity LISP checker!\n");

check.CheckLisp();

public class LispValidator
{
    private bool LispValidation(List<string> instance)
    {
        var isValid = new bool();

        var openParenPattern = @"\([\s\S]*?";

        var closeParenPattern = @"[\s\S]*?\)";

        var countinstance = 0;

        instance.ForEach(s => {            

            foreach(char c in s)
            {
                if (Regex.IsMatch(c.ToString(), openParenPattern, RegexOptions.IgnorePatternWhitespace) || Regex.IsMatch(c.ToString(), closeParenPattern, RegexOptions.IgnorePatternWhitespace))
                {
                    countinstance++;
                }
            }
            isValid = countinstance % 2 == 0;
            //"\((?>\((?<c>)|[^()]+|\)(?<-c>))*(?(c)(?!))\)"
            //Regex.IsMatch(s, @"^(\()?[^()]*(?(1)\))$", RegexOptions.IgnorePatternWhitespace)            
        });

        Console.WriteLine("It is validly enclosed in parentheses " + isValid.ToString() + ".");

        var stack = new Stack<int>();
        countinstance = 0;

        instance.ForEach(s =>
        {
            isValid = true;
            if (Regex.IsMatch(s.ToString(), openParenPattern, RegexOptions.IgnorePatternWhitespace))
            {
                stack.Push(countinstance++);
            }
            else
            {
                if (Regex.IsMatch(s.ToString(), closeParenPattern, RegexOptions.IgnorePatternWhitespace))
                {
                    if (countinstance < stack.Count)
                        countinstance = stack.Count;

                    if (stack.Count > 0)
                        stack.Pop();
                    else
                        countinstance = -1;
                }
            }
        });        

        return isValid = countinstance > 0;
    }

    private bool LispValidation(string instance)
    {
        var isValid = new bool();

        var openParenPattern = @"\([\s\S]*?";

        var closeParenPattern = @"[\s\S]*?\)";

        var countinstance = 0;

        foreach (char c in instance)
        {
            if (Regex.IsMatch(c.ToString(), openParenPattern, RegexOptions.IgnorePatternWhitespace) || Regex.IsMatch(c.ToString(), closeParenPattern, RegexOptions.IgnorePatternWhitespace))
            {
                countinstance++;
            }
        }
        isValid = countinstance % 2 == 0;

        Console.WriteLine("It is validly enclosed in parentheses " + isValid.ToString() + ".\n");

        var stack = new Stack<int>();
        countinstance = 0;
        //Checks the depth of nested instance and maximum number of instances that exists...TCDW
        instance.ToList().ForEach(s =>
        {
            if (Regex.IsMatch(s.ToString(), openParenPattern, RegexOptions.IgnorePatternWhitespace))
            {
                stack.Push(countinstance++);
            }
            else
            {
                if(Regex.IsMatch(s.ToString(), closeParenPattern, RegexOptions.IgnorePatternWhitespace))
                {
                    if (countinstance < stack.Count)
                        countinstance = stack.Count;

                    if (stack.Count > 0)
                        stack.Pop();
                    else
                        countinstance = -1;
                }
            }
        });

        return isValid = countinstance > 0;
    }

    public void CheckLisp()
    {
        var verify = new LispValidator();

        var id = "parentheses"; //Change up to pass in parameter that states which file to use...TCDW

        var fileName = Directory.GetFiles(@"./Repo").Where(f => f.Contains($"{id}")).FirstOrDefault();

        var instance = File.ReadLines(fileName);

        var isValid = (instance.ToList().Count > 1) ? verify.LispValidation(instance.ToList()) : verify.LispValidation(instance.ToList()[0].ToString());
                
        Console.WriteLine("It is a valid nesting of parentheses " + isValid.ToString() + ".");
        Console.ReadLine();
    }
}
