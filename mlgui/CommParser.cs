using function.tree;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using function.math;

namespace mlgui
{
    public static class CommParser
    {
        public static RichTextBox r2, r3;
        public static MLGUI mlgui;

        public static bool comm_isAnsNumVal = false;
        public static bool comm_isAnsFunc = false;
        public static FunctionTree output_Func;
        public static double output_Num;

        public static bool processing = false;

        public static bool parseError = false;
        public static bool syntaxError = false;
        public static bool memoryError = false;

        private static string[] synline = new string[10];
        private static int syn_inx = 0;

        private static int noOfLines = 0;
        private static int cuurrentLineNo = 0;

        private static string p_line = "";
        private static int p_len = 0;

        private static char cT;
        private static int inx;
        private static int beginInx;

        private static string[] commands = { "solve", "value", "derivative", "integral", "roots", "plot", "simplify", "cls", "clear", "del", "delete"};
        private static int comm_inx = -1;

        private static string[] output = new string[10];
        private static int output_inx = 0;

        private static FunctionTree[] funcs = new FunctionTree[10];
        private static FVar[] vars = new FVar[10];
        private static int var_inx = 0;
        private static int func_inx = 0;

        private static string preEq = "";
        private static string postEq = "";
        private const int VariableAssignment = 0;
        private const int FunctionAssignment = 1;
        private static string is_var = "";
        private static string is_func = "";
        private static double is_val = 0.0;
        private static FunctionTree is_existingFunc = null;

        private static bool show_output = true;

        public static bool Error()
        {
            if (syntaxError)
            {
                output[output_inx] = "Syntax Error";
                return true;
            }
            else if (parseError)
            {
                output[output_inx] = "Parse Error";
                output_inx++;
                return true;
            }
            else if (memoryError)
            {
                output[output_inx] = "Memory Error";
                output_inx++;
                return true;
            }
            else return false;
        }

        private static void Initialize()
        {
            output = new string[10];

            for (int i=-0; i<output.Length; i++)
            {
                output[i] = "";
            }

            for (int x = 0; x < synline.Length; x++)
            {
                synline[x] = "";
            }

            comm_inx = -1;

            output_inx = 0;
            syn_inx = 0;
            var_inx = 0;
            func_inx = 0;
            inx = 0;

            parseError = false;
            syntaxError = false;
            memoryError = false;

            comm_isAnsNumVal = false;
            comm_isAnsFunc = false;
        }

        public static void ProcessInput(int startpoint)
        {
            processing = true;

            if (startpoint == 0) r3.Text = "";

            noOfLines = r2.Lines.Length-1;

            int u = startpoint;      // process only last line

            for (; u < noOfLines; u++)
            {
                cuurrentLineNo = u;
                Initialize();
                show_output = true;

                p_line = r2.Lines[u];
                p_line = p_line.Trim(' ');
                if (p_line == "") continue;
                if (p_line[p_line.Length - 1] == ';') show_output = false;
                p_line = p_line.Trim(';');
                p_len = p_line.Length;
                if (p_len == 0) continue;
                
                // Skip Initial Spaces
                SkipSpaces();
                inx--;
                if (End()) continue;

                // Store This Point;
                beginInx = inx;
               
                if (IsCommand())
                {
                    // Skip Spaces
                    SkipSpaces();
                    inx--;
                    if (End()) continue;
                    
                    ParseCommand();
                }
                else if (IsEquation(p_line))
                {
                    if (SplitEquation(p_line, VariableAssignment))
                    {
                        FVar temp = null;
                        if ((temp = ListHandler.ML_VariableList(MLGUI.GetVariable, is_var))!=null)
                        {
                            ListHandler.ML_VariableList(MLGUI.UpdateVariableValue, temp.varName, is_val);
                        }
                        else
                        {
                            ListHandler.ML_VariableList(MLGUI.AddItem, is_var, is_val);
                        }
                    }
                    else if (SplitEquation(p_line, FunctionAssignment))
                    {
                        FunctionTree temp = null;
                        if ((temp = ListHandler.ML_FunctionList(MLGUI.GetFunction, is_var)) != null)
                        {
                            ListHandler.ML_FunctionList(MLGUI.UpdateFunctionExpression, is_var, is_func);
                        }
                        else
                        {
                            ListHandler.ML_FunctionList(MLGUI.AddItem, is_var, is_func);
                        }

                        //foreach (FVar v in temp.varList)
                        //{ 
                            //if (ListHandler.ML_FunctionList(MLGUI.GetFunction, v.varName))
                        //}
                    }
                    else
                    {
                        syntaxError = true;
                        Error();
                    }
                }
                else if (IsVariable(p_line))
                {
                    ListHandler.ML_VariableList(MLGUI.RemoveItem, is_var);
                    ListHandler.ML_VariableList(MLGUI.AddItem, is_var);
                }
                else if (IsFunction(p_line))
                {
                    FunctionEntry temp = null;
                    if ((temp = ListHandler.ML_FunctionListSecondary(MLGUI.GetFunctionFromExpression, is_func)) != null)
                    {
                        r2.AppendText(temp.Name);
                        r2.SelectionStart = r2.TextLength;
                        r2.ScrollToCaret();
                    }
                    else
                    {
                        ListHandler.ML_FunctionList(MLGUI.AddItem, "", is_func);
                    }
                }
                else
                {
                    parseError = true;
                    Error();
                }
                
                if (output[0] != "" && show_output)
                {
                    string lu = (u + 1) + "";
                    r3.AppendText("line-" + lu);
                    for (int o = 0; o < (5 - lu.Length); o++) r3.Text += "  ";
                    r3.AppendText("\t"+output[0] + "\n");

                    int z = 1;
                    while(output[z]!="" && z<output.Length)
                    {
                        for (int o = 0; o < 10; o++) r3.Text += "  ";
                        r3.AppendText("\t"+output[z] + "\n");
                        z++;
                    }
                }
            }
            processing = false;
        }

        public static void SkipSpaces()
        {
            cT = ' ';
            while (NotEnd() && cT == ' ')
            {
                cT = p_line[inx];
                inx++;
            }
        }

        public static bool IsEquation(string str)
        {
            if (str == "") return false;

            for (int x = 0; x < str.Length; x++)
            {
                if (str[x] == '=') return true;
            }

            return false;
        }

        private static char[] notAllowedInFuncName =
{
            ' ', ',', '+', '-', '*', '/', '^', ';', '(', ')', '~', '@', '#', '!', '%', '&', ':', '=', '\\'
        };

        public static bool IsExistingFunctionName(string str)
        {
            // Parse str and check for possible function name match
            // Assign is_existingFunc

            str = str.Trim(notAllowedInFuncName);

            if (str == "") return false;

            foreach (FunctionEntry fe in MLGUI.ML_Functions)
            {
                if (fe.Name == str)
                {
                    is_existingFunc = fe.Function;
                    return true;
                }
            }


            return false;
        }

        // Splits Equation into Pre-Eq and Post-Eq
        // Checks if equation is a valid Variable-Assignment or a valid Function-Assignment 
        public static bool SplitEquation(string equation, int type)
        {
            if (equation == "") return false;

            preEq = "";
            postEq = "";
            int eq_inx = 0;
            
            while (eq_inx < equation.Length && equation[eq_inx]!= '=')
            {
                preEq += equation[eq_inx];
                eq_inx++; 
            }

            eq_inx++;

            while (eq_inx < equation.Length && equation[eq_inx] != '=')
            {
                postEq += equation[eq_inx];
                eq_inx++;
            }

            if (IsVariable(preEq))
            {
                switch (type)
                {
                    case VariableAssignment:    if (IsNumber(postEq)) return true;      break;
                    case FunctionAssignment:    if (IsFunction(postEq)) return true;    break;
                    default:                                                            break;
                }
            }
            else if (IsVariable(postEq))
            {
                switch (type)
                {
                    case VariableAssignment:    if (IsNumber(preEq)) return true;       break;
                    case FunctionAssignment:    if (IsFunction(preEq)) return true;     break;
                    default:                                                            break;
                }
            }

            return false;
        }

        public static bool IsCommand()
        {

            while (NotEnd() && cT != ' ')
            {
                synline[syn_inx] += p_line[inx];
                inx++;
                if (NotEnd()) cT = p_line[inx];
            }

            for (int x=0; x<commands.Length; x++)
            {
                if (synline[syn_inx].Equals(commands[x], StringComparison.CurrentCultureIgnoreCase))
                {
                    syn_inx++;
                    comm_inx = x;
                    return true;
                }
            }

            inx = beginInx;
            syn_inx = 0;

            return false;

        }


        // Variable Checking Method

        private static char[] notAllowedInVarName =
        {
            ' ', ',', '+', '-', '*', '/', '^', ';', '(', ')', '~', '@', '#', '!', '%', '&', ':', '=', '\\'
        };

        // Can also be a substitute for IsFunctionName()
        public static bool IsVariable(string str)
        {
            int i = 0;
            is_var = null;
            string var = "";
            char g = ' ';
            
            // Fixes Start and End of a Possible VarName from any errors
            str = str.Trim(notAllowedInVarName);

            if (IsNumber(str[0] + "")) return false;

            while(i < str.Length && str[i]==' ')
            {
                i++;
            }

            while(i < str.Length)
            {
                g = str[i];
                foreach (char k in notAllowedInVarName)
                {
                    if (g==k)
                        return false;
                }
                
                var += str[i];
                i++;
            }

            if (var!="")
            {
                is_var = var;
                return true;
            }

            return false;
        }

        // Function Checking Method

        private static char[] notAllowedInFunction =
        {
            ';', '|', '=', '\\'
        };


        public static bool IsFunction(string str)
        {
            int i = 0;
            is_func = null;
            string fn = "";
            char g = ' ';
            int brackets = 0;

            // Fixes Start and End of a Possible VarName from any errors
            str = str.Trim(notAllowedInFunction);

            while (i < str.Length && str[i] == ' ')
            {
                i++;
            }

            while (i < str.Length)
            {
                g = str[i];

                if (g == '(') brackets++;
                else if (g == ')') brackets--;

                if (brackets < 0) return false;

                foreach (char k in notAllowedInFunction)
                {
                    if (g == k)
                        return false;
                }

                fn += str[i];
                i++;
            }

            if (brackets != 0) return false;

            if (fn != "")
            {
                is_func = fn;
                return true;
            }

            return false;
        }



        public static bool IsNumber(string str)
        {            
            string numT = str.Trim(' ');
            double num = 0.0;

            if (str == "") return false;

            try
            {
                num = System.Convert.ToDouble(numT);
            }
            catch (FormatException)
            {
                return false;
            }
            catch (OverflowException)
            {
                return false;
            }

            is_val = num;
            return true;
        }

        public static bool NotEnd()
        {
            return inx < p_len;
        }

        public static bool End()
        {
            return !(inx < p_len);
        }

        public static void ParseCommand()
        {
            // commands = { "solve", "value", "derivative", "integral", "roots", "plot" , "simplify"};
            
            switch(comm_inx)
            {
                case 0:     // solve
                case 1:     // value
                    {

                    }
                    break;

                case 2:     // derivative
                    {
                        int bracket = 0;

                        // Parse Function
                        while (NotEnd())
                        {
                            if (bracket == 0 && p_line[inx] == ',') break;
                            if (p_line[inx] == '(') bracket++;
                            if (p_line[inx] == ')') bracket--;
                            synline[syn_inx] += p_line[inx];
                            inx++;
                        }

                        if (synline[syn_inx] == "")
                        {
                            syntaxError = true;
                            Error();
                            break;
                        }
                                                
                        bool isVariable = false;
                        bool isFunction = false;
                        bool isFunctionName = false;

                        int func_fn_inx = 0;
                        int var_fn_inx = 0;
                        
                        // Try Parsing Function or Variable
                        // All Variables get Added to ML_Variables
                        if (isFunctionName = IsExistingFunctionName(synline[syn_inx]))
                        {
                            funcs[func_inx] = is_existingFunc;
                            func_inx++;

                            if (End())
                            {
                                output[output_inx] = funcs[func_fn_inx].GetDerivative(funcs[func_fn_inx].VarList[0]).ToString();
                                break;
                            }
                        }
                        else if (isVariable = IsVariable(synline[syn_inx]))
                        {
                            vars[var_inx] = ListHandler.ML_VariableList(MLGUI.AddItem, is_var);
                            var_fn_inx = var_inx;
                            var_inx++;

                            if (End())
                            {
                                FunctionTree temp = new FunctionTree(vars[var_fn_inx]);
                                output[output_inx] = temp.GetDerivative(vars[var_fn_inx]).ToString();
                                break;
                            }
                        }
                        else if (isFunction = IsFunction(synline[syn_inx]))
                        {
                            funcs[func_inx] = ListHandler.ML_FunctionList(MLGUI.AddItem, "", is_func);
                            func_fn_inx = func_inx;
                            func_inx++;

                            if (End())
                            {
                                output[output_inx] = funcs[func_fn_inx].GetDerivative(funcs[func_fn_inx].VarList[0]).ToString();
                                break;
                            }
                        }
                        else
                        {
                            syntaxError = true;
                            Error();
                            break;
                        }

                        // Parse Further
                        SkipSpaces();
                        inx--;
                        if (End()) break;

                        bool wrtSet = false;
                        FVar wrtVar = null;
                        double wrtVarVal = 0;
                        bool wrtVarValSet = false;
                        syn_inx++;
                        List<string> varEqList = new List<string>();

                        // Next vars[var_inx] Entry will be the w.r.t. variable
                        int func_sl_inx = syn_inx;

                        // Parse rest of the line
                        // Requires addition of "," at end
                        p_line = p_line + ",";
                        p_len = p_line.Length;

                        inx++;

                        while (NotEnd())
                        {
                            if(p_line[inx]==',')
                            {
                                if (!isVariable && IsEquation(synline[syn_inx]) && SplitEquation(synline[syn_inx], VariableAssignment))
                                {
                                    if (ListHandler.ML_VariableList(MLGUI.UpdateVariableValue, is_var, is_val)==null)
                                    {
                                        syntaxError = true;
                                        break;
                                    }
                                    else
                                    {                                        
                                        varEqList.Add(is_var);
                                    }
                                }
                                else if (IsNumber(synline[syn_inx]) && !wrtVarValSet)
                                {
                                    wrtVarVal = is_val;
                                    if (wrtSet)
                                    {
                                        ListHandler.ML_VariableList(MLGUI.UpdateVariableValue, wrtVar.varName, is_val);
                                        wrtVarVal = is_val;
                                    }
                                    wrtVarValSet = true;
                                }
                                else if (IsVariable(synline[syn_inx]) && !wrtSet)
                                {
                                    wrtVar = ListHandler.ML_VariableList(MLGUI.AddItem, is_var);
                                    wrtSet = true;
                                }
                                else
                                {
                                    syntaxError = true;
                                    break;
                                }
                                syn_inx++;
                               
                            }
                            else synline[syn_inx] += p_line[inx];

                            inx++;  
                        }

                        if (Error()) break;

                        if (isVariable)
                        {
                            if (!wrtSet)
                            {
                                wrtVar = vars[var_fn_inx];
                                if (wrtVarValSet)
                                {
                                    ListHandler.ML_VariableList(MLGUI.UpdateVariableValue, wrtVar.varName, wrtVarVal);
                                }
                            }

                            output[output_inx] += vars[var_fn_inx].GetDerivative(wrtVar).ToString();
                            break;
                        }

                        if (!wrtSet)
                        {
                            wrtVar = funcs[func_fn_inx].varList[0];
                        }

                        if (wrtVarValSet)
                        {
                            ListHandler.ML_VariableList(MLGUI.UpdateVariableValue, wrtVar.varName, wrtVarVal);
                        }

                        FunctionTree copy = funcs[func_fn_inx].GetCopy();
                        
                        foreach(string varEq in varEqList)
                        {
                            if (varEq == wrtVar.varName) continue;
                            if (!copy.VariableToSetValue(varEq))
                            {
                                syntaxError = true;
                                break;
                            }
                        }

                        FunctionTree der = copy.GetDerivative(wrtVar);

                        if (Error()) break;

                        // Store result in output string
                        if (der.varList.Count == 0)
                        {
                            output[output_inx] = der.GetValue().ToString();
                            
                            // Important to figure out properly
                            comm_isAnsNumVal = true;

                            // Single answer
                            ListHandler.ML_FunctionList(MLGUI.RemoveItem, "ans");
                            ListHandler.ML_VariableList(MLGUI.RemoveItem, "ans");

                            ListHandler.ML_VariableList(MLGUI.AddItem, "ans", der.GetValue());
                        }
                        else
                        {
                            output[output_inx] = der.ToString();
                            
                            if (der.CanHaveValue())
                            {
                                string derVal = der.GetValue().ToString();
                                
                                if (!(derVal == output[output_inx]))
                                {
                                    output_inx++;
                                    output[output_inx] = derVal;

                                    // Important to figure out properly
                                    comm_isAnsFunc = true;
                                    comm_isAnsNumVal = true;

                                    // Two Answers
                                    ListHandler.ML_FunctionList(MLGUI.RemoveItem, "ans0");
                                    ListHandler.ML_VariableList(MLGUI.RemoveItem, "ans0");
                                    ListHandler.ML_FunctionList(MLGUI.RemoveItem, "ans");
                                    ListHandler.ML_VariableList(MLGUI.RemoveItem, "ans");

                                    ListHandler.ML_FunctionList(MLGUI.AddItem, "ans0", der.ToString());
                                    ListHandler.ML_VariableList(MLGUI.AddItem, "ans", der.GetValue());
                                }
                                else
                                {
                                    // Important to figure out properly
                                    comm_isAnsNumVal = true;
                                    ListHandler.ML_FunctionList(MLGUI.RemoveItem, "ans");
                                    ListHandler.ML_VariableList(MLGUI.RemoveItem, "ans");

                                    ListHandler.ML_VariableList(MLGUI.AddItem, "ans", der.GetValue());
                                }
                            }
                            else
                            {
                                // Important to figure out properly
                                comm_isAnsFunc = true;

                                // Single answer
                                ListHandler.ML_FunctionList(MLGUI.RemoveItem, "ans");
                                ListHandler.ML_VariableList(MLGUI.RemoveItem, "ans");

                                ListHandler.ML_FunctionList(MLGUI.AddItem, "ans", der.ToString());
                            }
                        }
                    }
                    break;

                case 3:     // integral
                    {

                    }
                    break;

                case 4:     // roots
                    {

                    }
                    break;

                case 5:     // plot
                    {

                    }
                    break;
                case 6:     // simplify
                    {
                    
                    }
                    break;
                case 7:     // cls
                    {
                        r2.SelectAll(); r2.SelectionProtected = false;
                        r2.Clear();
                        r3.Clear();
                        mlgui.AddLineNumbers(0);
                    }
                    break;
                default: break;

            }

        }

    }

}
