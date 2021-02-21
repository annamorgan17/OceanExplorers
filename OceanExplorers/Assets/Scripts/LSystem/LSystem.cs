using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using System.Text;
public class Lsystem { 
    /// <summary>
    /// Ammend a string using a l system 
    /// </summary>
    /// <param name="s">String to ammend</param>
    /// <param name="rule">Rule dictionary</param>
    /// <param name="appendmentChance">Chance to ammend</param>
    public Lsystem(ref string s, Dictionary<char, string> rule, int appendmentChance = 0) {
        StringBuilder sb = new StringBuilder();
        //Loop each character
        foreach (char c in s) {
            //Chance for nothing to happen
            if (c == 'F' & UnityEngine.Random.Range(0, 100) < appendmentChance) {
            } else if (rule.ContainsKey(c)) { //if rule is there
                sb.Append(rule[c]);
            } else {
                sb.Append(c.ToString()); //if not add old value back
            }
        }
        //output
        s = sb.ToString();
    } 


    /// <summary>
    /// Extract a parmiter from a string
    /// </summary>
    /// <param name="currentString">String to extract from</param>
    /// <param name="index">Index where start of paramiter was found</param>
    /// <param name="transformInfo">Transform infor for keyword reference</param>
    /// <param name="variables">Paramiter variables</param>
    /// <param name="angle">Angle for keyword reference</param>
    /// <returns></returns>
    public static  List<float> ExtractParmiter(string currentString, int index, TransformInfo transformInfo, List<Param> variables, float angle) {                                                                                                                                     

        List<float> retrn = new List<float>();
        int i = index;
        char c = currentString.ToCharArray()[i];  

        //Keyword based of data available
        Param[] Keywords = new Param[] {
            new Param('Y', transformInfo.transform.position.y),
            new Param('L', transformInfo.branchLength),
            new Param('A', angle) };

        //loop untill end of param
        while (c != ')' & i < currentString.ToCharArray().Length - 1) {
            float Value = 0;
            char[] characters = currentString.ToCharArray();
            string param = "";
            List<char> Oprator = new List<char>();
            List<float>  items = new List<float>();
            c = characters[i];
            //loop untill end of param or another param
            while (c != ')' & c != ',' & i < characters.Length - 1) {
                c = characters[i];
                //Check for keywords
                foreach (var item in Keywords) {
                    if (item.Character == c) {
                        param = item.Value.ToString();
                    }
                }
                //Variables
                foreach (var item in variables) { 
                    if (item.Character == c) {
                        param = item.Value.ToString(); 
                    }
                }

                //check for operators
                if ((c == '+' || c == '-' || c == '/' || c == '*') & param.ToCharArray().Length != 0) {
                    Oprator.Add(c);
                    items.Add(float.Parse(param, System.Globalization.NumberStyles.Float));
                    param = "";
                }

                //check for numbers
                if (c >= '0' && c <= '9' && c != ' ') {
                    param = param + c;
                }

                i++;
            }

            //add remaining
            if (param != "") {
                items.Add(float.Parse(param, System.Globalization.NumberStyles.Float));
            }

            //do calculations
            if (items.Count != 0) {
                Value = items[0];
            } 
            //Do the maths
            for (int o = 0; o < Oprator.Count; o++) {
                //exit if out of bounds
                if (items.Count < o + 1) {
                    Debug.LogWarning("Missing end of equation, exiting without final expression");
                    retrn.Add(Value);
                }

                //different operations depending on whats saved
                switch (Oprator[o]) {
                    case '+': //add
                        Value += items[o + 1];
                        break;
                    case '-'://minus
                        Value -= items[o + 1];
                        break;
                    case '/'://divide
                        Value /= items[o + 1];
                        break;
                    case '*'://times
                        Value *= items[o + 1];
                        break;
                    default:
                        break;
                }
            }

            retrn.Add(Value);
        }
        //return value
        return retrn;
    }
}


