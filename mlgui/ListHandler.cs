using function.math;
using function.tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mlgui
{
    public static  class ListHandler
    {

        public static MLGUI mlgui;
        public static RichTextBox r3;

        // Sets Value to a Valriable in VarList
        public static FVar ML_VariableList(int request, String name, double val)
        {
            if (request == MLGUI.UpdateVariableValue)
            {
                foreach (VariableEntry ve in MLGUI.ML_Variables)
                {
                    if (ve.Name == name)
                    {
                        ve.Variable.SetValue(val);
                        return ve.Variable;
                    }
                }
            }
            else if (request == MLGUI.AddItem)
            {
                foreach (VariableEntry ve in MLGUI.ML_Variables)
                {
                    if (ve.Name == name)
                    {
                        ve.Variable.SetValue(val);
                        return ve.Variable;
                    }
                }
                
                ML_FunctionList(MLGUI.RemoveItem, name);

                FVar newVar = new FVar(name);
                newVar.SetValue(val);
                MLGUI.lastVariableEntry = new VariableEntry(name, newVar);
                MLGUI.ML_Variables.Add(MLGUI.lastVariableEntry);

                return newVar;
            }
            return null;
        }

        // Returns NULL if request did not succeeed
        public static FVar ML_VariableList(int request, String name)
        {
            if (request == MLGUI.GetVariable)
            {
                foreach (VariableEntry ve in MLGUI.ML_Variables)
                {
                    if (ve.Name == name)
                    {
                        return ve.Variable;
                    }
                }
            }
            else if (request == MLGUI.AddItem)
            {
                foreach (VariableEntry ve in MLGUI.ML_Variables)
                {
                    if (ve.Name == name)
                        return ve.Variable;
                }
                
                ML_FunctionList(MLGUI.RemoveItem, name);

                FVar newVar = new FVar(name);
                MLGUI.lastVariableEntry = new VariableEntry(name, newVar);
                MLGUI.ML_Variables.Add(MLGUI.lastVariableEntry);

                return newVar;
            }
            else if (request == MLGUI.RemoveItem)
            {
                foreach (VariableEntry ve in MLGUI.ML_Variables)
                {
                    if (ve.Name == name)
                    {
                        FVar temp = ve.Variable;
                        MLGUI.ML_Variables.Remove(ve);
                        return temp;
                    }
                }
            }
            else if (request == MLGUI.RemoveLastItem)
            {
                MLGUI.ML_Variables.Remove(MLGUI.lastVariableEntry);
                return MLGUI.lastVariableEntry.Variable;
            }
            return null;
        }

        // Returns NULL if request did not succeeed
        public static FunctionTree ML_FunctionList(int request, String name)
        {
            if (request == MLGUI.GetFunction)
            {
                foreach (FunctionEntry fe in MLGUI.ML_Functions)
                {
                    if (fe.Name == name)
                    {
                        return fe.Function;
                    }
                }
            }
            else if (request == MLGUI.RemoveItem)
            {
                foreach (FunctionEntry fe in MLGUI.ML_Functions)
                {
                    if (fe.Name == name)
                    {
                        FunctionTree temp = fe.Function;
                        MLGUI.ML_Functions.Remove(fe);
                        return temp;
                    }
                }
            }
            return null;
        }

        public static FunctionEntry ML_FunctionListSecondary(int request, String funcStr)
        {
            if (request == MLGUI.GetFunctionFromExpression)
            {                
                FunctionTree newFunc = new FunctionTree(funcStr);
                String newFuncStr = newFunc.ToString();

                // Crude Function Comparison
                foreach (FunctionEntry fe in MLGUI.ML_Functions)
                {
                    if (fe.Function.ToString() == newFuncStr)
                    {
                        return fe;
                    }
                }
            }
            return null;
        }

        public static FunctionTree ML_FunctionList(int request, String name, String funcStr)
        {
            if (request == MLGUI.UpdateFunctionExpression)
            {
                foreach (FunctionEntry fe in MLGUI.ML_Functions)
                {
                    if (fe.Name == name)
                    {
                        fe.Function = new FunctionTree(funcStr);
                        if (fe.Function.HasNoVariables() )
                        {
                            ML_VariableList(MLGUI.AddItem, name, fe.Function.GetValue());
                            return null;
                        }
                        else return fe.Function;
                    }
                }
            }
            else if (request == MLGUI.GetFunction)
            {
                foreach (FunctionEntry fe in MLGUI.ML_Functions)
                {
                    if (fe.Name == name)
                    {
                        return fe.Function;
                    }
                }
            }
            else if (request == MLGUI.AddItem)
            {
                FunctionTree newFunc = new FunctionTree(funcStr);
                String newFuncStr = newFunc.ToString();

                //r3.Text += "vars ";
                //foreach (FVar v in newFunc.varList) r3.Text+=v.varName + " ";
                
                // Crude Function Comparison
                foreach (FunctionEntry fe in MLGUI.ML_Functions)
                {
                    if (fe.Function.ToString() == newFuncStr)
                    {
                        if (name != "") fe.Name = name;
                        return fe.Function;
                    }
                }

                if (name == "")
                {
                    int x = 1;
                    bool nameTaken = true;
                    while (nameTaken)
                    {
                        name = "f" + x;
                        nameTaken = false;

                        foreach (FunctionEntry fe in MLGUI.ML_Functions)
                        {
                            if (fe.Name == name)
                            {
                                nameTaken = true;
                                break;
                            }
                            else nameTaken = false;
                        }

                        if (nameTaken) x++;
                        else break;
                    }

                    MLGUI.lastFunctionEntry = new FunctionEntry(name, newFunc);
                    MLGUI.ML_Functions.Add(MLGUI.lastFunctionEntry);
                    return newFunc;
                }
                else
                {
                    foreach (FunctionEntry fe in MLGUI.ML_Functions)
                    {
                        if (fe.Name == name)
                        {
                            fe.Function = new FunctionTree(funcStr);
                            return fe.Function;
                        }
                    }

                    ML_VariableList(MLGUI.RemoveItem, name);

                    MLGUI.lastFunctionEntry = new FunctionEntry(name, newFunc);
                    MLGUI.ML_Functions.Add(MLGUI.lastFunctionEntry);

                    if (MLGUI.lastFunctionEntry.Function.HasNoVariables())
                    {
                        ML_VariableList(MLGUI.AddItem, name, MLGUI.lastFunctionEntry.Function.GetValue());
                    }
                    else return newFunc;
                }
            }
            else if (request == MLGUI.RemoveItem)
            {
                foreach (FunctionEntry fe in MLGUI.ML_Functions)
                {
                    if (fe.Name == name)
                    {
                        FunctionTree temp = fe.Function;
                        MLGUI.ML_Functions.Remove(fe);
                        return temp;
                    }
                }
            }
            else if (request == MLGUI.RemoveLastItem)
            {
                MLGUI.ML_Functions.Remove(MLGUI.lastFunctionEntry);
                return MLGUI.lastFunctionEntry.Function;
            }
            return null;
        }
    }

    public class FunctionEntry
    {
        public string Name;
        public FunctionTree Function;
        private String EntryFormat = "{0, -15}\t{1, 0}";

        public FunctionEntry(string n, FunctionTree f)
        {
            Name = n;
            Function = f;
        }
        public override string ToString()
        {
            return String.Format(EntryFormat, Name, Function.ToString());
        }
    }

    public class VariableEntry
    {
        public FVar Variable;
        public string Name;
        private String EntryFormat = "{0, -15}\t{1, 0}";

        public VariableEntry(string n, FVar v)
        {
            Name = n;
            Variable = v;
        }
        public override string ToString()
        {
            if (Variable.IsSet())
                return String.Format(EntryFormat, Name, Variable.GetValue());
            else
                return String.Format(EntryFormat, Name, "Undefined");
        }

        public string ToStringName()
        {
            return Name;
        }
        
        public string ToStringValue()
        {
            if (Variable.IsSet())
                return Variable.GetValue() + "";
            else
                return "Undefined";
        }
    }
}
