using function.math;
using mlgui;
using System;
using System.Collections.Generic;

namespace function.tree
{

	
	using FunctionStack = util.FunctionStack;

	public class TreeMaker
	{
        public static void CreateFunctionTree(string[] postfixArray, FunctionTree FTree)
		{
			FunctionStack fStk = new FunctionStack();
            FTree.varList = new List<FVar>();
            string f; int i = 0;

            Function temp1, temp2;
            bool chk1, chk2;

			while (i < postfixArray.Length)
			{
				f = postfixArray[i];
			    //Console.Write(" "+ f);
                
				if (f.Equals(FNeg.ID, StringComparison.CurrentCultureIgnoreCase) || f.Equals(FNeg.Symbol, StringComparison.CurrentCultureIgnoreCase))
				{
					fStk.Push(new FNeg(fStk.TopAndPop()));
				}
				else if (f.Equals(FAdd.ID, StringComparison.CurrentCultureIgnoreCase) || f.Equals(FAdd.Symbol, StringComparison.CurrentCultureIgnoreCase))
                {
                    fStk.Push(new FAdd(fStk.TopAndPop(), fStk.TopAndPop(), 1));
				}
				else if (f.Equals(FSub.ID, StringComparison.CurrentCultureIgnoreCase) || f.Equals(FSub.Symbol, StringComparison.CurrentCultureIgnoreCase))
				{
                    fStk.Push(new FSub(fStk.TopAndPop(), fStk.TopAndPop(), 1));
				}
				else if (f.Equals(FMul.ID, StringComparison.CurrentCultureIgnoreCase) || f.Equals(FMul.Symbol, StringComparison.CurrentCultureIgnoreCase))
                {
                    temp1 = fStk.TopAndPop();
                    chk1 = temp1.GetID().Equals(FCons.ID) && !fStk.Top().GetID().Equals(FCons.ID);
                    
                    if (chk1 && fStk.Top().isTwoSidedFunction && !((TwoSidedFunction)fStk.Top()).RHS().GetID().Equals(FCons.ID))
                    {
                        temp2 = ((TwoSidedFunction)fStk.Top()).RHS();
                        ((TwoSidedFunction)fStk.Top()).SetRHS(temp1);
                        fStk.Push(new FMul(temp2, fStk.TopAndPop(), 1));
                    }
                    else if (chk1 && fStk.Top().isVariable)
                    {
                        fStk.Push(new FMul(fStk.TopAndPop(), temp1, 1));
                    }
                    else
                    {
                        fStk.Push(new FMul(temp1, fStk.TopAndPop(), 1));
                    }
                }
				else if (f.Equals(FDiv.ID, StringComparison.CurrentCultureIgnoreCase) || f.Equals(FDiv.Symbol, StringComparison.CurrentCultureIgnoreCase))
				{
					fStk.Push(new FDiv(fStk.TopAndPop(), fStk.TopAndPop(), 1));
				}
				else if (f.Equals(FPow.ID, StringComparison.CurrentCultureIgnoreCase) || f.Equals(FPow.Symbol, StringComparison.CurrentCultureIgnoreCase))
				{
					fStk.Push(new FPow(fStk.TopAndPop(), fStk.TopAndPop(), 1));
				}
				else if (f.Equals(FExp.ID, StringComparison.CurrentCultureIgnoreCase) || f.Equals(FExp.Symbol, StringComparison.CurrentCultureIgnoreCase))
				{
					fStk.Push(new FExp(fStk.TopAndPop()));
				}
				else if (f.Equals(FLn.ID, StringComparison.CurrentCultureIgnoreCase) || f.Equals(FLn.Symbol, StringComparison.CurrentCultureIgnoreCase))
				{
					fStk.Push(new FLn(fStk.TopAndPop()));
				}
				// sin()
				else if (f.Equals(FSin.ID, StringComparison.CurrentCultureIgnoreCase) || f.Equals(FSin.Symbol, StringComparison.CurrentCultureIgnoreCase))
				{
					fStk.Push(new FSin(fStk.TopAndPop()));
				}
				else if (f.Equals(FCos.ID, StringComparison.CurrentCultureIgnoreCase) || f.Equals(FCos.Symbol, StringComparison.CurrentCultureIgnoreCase))
				{
					fStk.Push(new FCos(fStk.TopAndPop()));
				}

				// add rest of the functions

				else if (IsConstant(f)) // double number
				{
					fStk.Push(new FCons(Convert.ToDouble(f)));
				}
				else
				{
					FVar @var = FTree.FindVariable(f);
					if (@var != null)
					{
						fStk.Push(@var);
					}
					else
					{
                        FunctionTree temp;
                        FVar tempVar;
                        if ((tempVar = ListHandler.ML_VariableList(MLGUI.GetVariable, f))!=null && tempVar.IsSet())
                        {
                            fStk.Push(new FCons(tempVar.GetValue()));
                        }
                        else if ((temp=ListHandler.ML_FunctionList(MLGUI.GetFunction, f))!=null)
                        {
                            fStk.Push(temp.rootNode);
                        }
						else fStk.Push(FTree.AddVariable(ListHandler.ML_VariableList(MLGUI.AddItem, f))); // variable
					}
				}

				i++;
			}




			FTree.rootNode = fStk.TopAndPop();
		}

		private static bool IsConstant(string f)
		{
			char c = f[0];
			if ((c >= '0' && c <= '9'))
			{
				//System.out.println("number: " + c);
				return true;
			}
			return false;
		}
	}

}