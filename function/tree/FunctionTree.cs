using System;
using System.Collections.Generic;

namespace function.tree
{

	using FCons = math.FCons;
	using FVar = math.FVar;

	public class FunctionTree
	{
		protected internal Function rootNode;
		protected internal string[] postfixArray;
		internal List<FVar> varList;
        private string funcTreeText = "";
        public static int funcTreeTextStyle = 0;

		public FunctionTree(string s)
		{
			s = StringRefiner.RefineFunctionString(s);
			PostfixMaker.CreatePostfixArray(s, this);
			TreeMaker.CreateFunctionTree(postfixArray, this);
			this.varList = rootNode.varList;
            GenerateFunctionTreeText(funcTreeTextStyle);
		}
                
		public FunctionTree(Function F)
		{
			this.rootNode = F;
            varList = new List<FVar>();
			this.varList = rootNode.varList;

			VarFix(rootNode);
            GenerateFunctionTreeText(funcTreeTextStyle);
		}

        public string GetFunctionTreeText()
        {
            return funcTreeText;
        }

        public void GenerateFunctionTreeText(int style)
        {
            funcTreeText = "";
            if (style == 0)     PTree(rootNode);
            else                PBTree(rootNode);
            traversal = 0;
            ptree_str = "";
        }

        private static int traversal = 0;
        private static string ptree_str = "";

        public void PrintTree()
        {
            PTree(this.rootNode);
            traversal = 0;
            ptree_str = "";
        }
        public void PrintBigTree()
        {
            PBTree(this.rootNode);
            traversal = 0;
            ptree_str = "";
        }


        private void PTree(Function F)
        {
            funcTreeText += ((F.GetID() == FCons.ID || F.GetID() == FVar.ID) ? F.GetSymbol().ToUpper() : F.GetID().ToUpper()) + "  " + F.ToString()+ "\n";
            for (int i = 0; i < F.numSubNodes; i++)
            {
                Function Fnode = F.GetSubNode(i);
                ptree_str = ptree_str.Substring(0, traversal) + ((i + 1) == F.numSubNodes ? "T" : "F") + ptree_str.Substring(traversal + 1 <= ptree_str.Length ? traversal + 1 : traversal);
                traversal++;
                for (int e = 0; e < traversal - 1; e++) funcTreeText += ptree_str[e].Equals('T') ? "    " : "│   ";
                funcTreeText += (i + 1) == F.numSubNodes ? "└───" : "├───";
                PTree(F.GetSubNode(i));
            }
            traversal--;
        }



        public void PBTree(Function F)
        {
            funcTreeText += ((F.GetID() == FCons.ID || F.GetID() == FVar.ID) ? F.GetSymbol().ToUpper() : F.GetID().ToUpper()) + "  " + F.ToString();
            for (int i = 0; i < F.numSubNodes; i++)
            {
                Function Fnode = F.GetSubNode(i);

                ptree_str = ptree_str.Substring(0, traversal) + ((i + 1) == F.numSubNodes ? "T" : "F") + ptree_str.Substring(traversal + 1 <= ptree_str.Length ? traversal + 1 : traversal);

                traversal++;
                for (int e = 0; e < traversal - 1; e++)
                    funcTreeText += ptree_str[e].Equals('T') ? "       " : "│      ";
                funcTreeText += "│   ";
                funcTreeText += "";
                for (int e = 0; e < traversal - 1; e++)
                    funcTreeText += ptree_str[e].Equals('T') ? "       " : "│      ";
                funcTreeText += (i + 1) == F.numSubNodes ? "└──────" : "├──────";
                PBTree(F.GetSubNode(i));
            }
            traversal--;
        }


        public virtual List<FVar> VarList
		{
			get
			{
				return varList;
			}
		}

		internal virtual Function RootNode()
		{
			return rootNode;
		}

        public FunctionTree GetCopy()
        {
            return new FunctionTree(rootNode.GetCopy());
        }

		public double GetValue()
		{
			return rootNode.GetValue();
		}

        public bool CanHaveValue()
        {
            foreach (FVar v in rootNode.varList)
            {
                if (v.IsSet() == false) return false;
            }
            return true;
        }

        public bool HasNoVariables()
        {
            if (varList == null) return true;
            if (varList.Count == 0) return true;
            return false;
        }

		public virtual FunctionTree GetDerivative(FVar var)
		{
			if (rootNode.HasVariable(var))
			{
				return new FunctionTree(rootNode.GetDerivative(var));
			}
			return new FunctionTree(new FCons(0));
		}

		public virtual FunctionTree GetDerivative(string var)
		{
			FVar v = new FVar(@var);
			if (rootNode.HasVariable(v))
			{
				return new FunctionTree(rootNode.GetDerivative(v));
			}
			return new FunctionTree(new FCons(0));
		}

		public virtual double GetValue(params string[] str)
		{
            // parse and set values
            
			for (int i = 0; i < str.Length; i++)
			{

                int w = 0;
				char c;
				string s = "";
				int len = str[i].Length;

				while (w < len)
				{
					c = str[i][w];
					if (c == '=')
					{
						break;
					}
					s = s + char.ToString(c);

					//System.out.println(str[i] + " // " + s);

					w++;
				}

                FVar f = FindVariable(s);
				if (f != null && w < len)
				{
                    try
					{
						f.SetValue(Convert.ToDouble(str[i].Substring(++w)));
					}
					catch (FormatException)
					{
					}
				}
			}


			return rootNode.GetValue();
		}

		internal virtual FVar FindVariable(string varName)
		{
			foreach (FVar v in varList)
			{
				if (v.varName.Equals(varName))
				{
					return v;
				}
			}
			return null;
		}

		public virtual double GetValue(params double[] val)
		{
			// set Values

			for (int i = 0; i < varList.Count && i < val.Length; i++)
			{
				varList[i].SetValue(val[i]);
			}

			return rootNode.GetValue();
		}

		public virtual Function AddVariable(FVar var)
		{
            if (FindVariable(var.varName)==null)
			    varList.Add(var);
			return var;
		}

		public override string ToString()
		{
			return rootNode.ToString();

		}

		public virtual void VarFix(Function F)
		{
			for (int i = 0; i < F.numSubNodes;i++)
			{
				Function Fnode = F.GetSubNode(i);

				if (Fnode.GetID() == FVar.ID)
				{
					FVar var = FindVariable(((FVar)Fnode).varName);
					if (var != null)
					{
						F.SetSubNode(i, var);
					}
					continue;
				}
				if (Fnode.GetID().Equals(FCons.ID))
				{
					continue;
				}
				VarFix(F.GetSubNode(i));
			}
		}

		public virtual void Simplify()
		{
			rootNode = rootNode.GetSimplifiedFunction();
			VarFix(rootNode);
            GenerateFunctionTreeText(funcTreeTextStyle);
		}

		public virtual FunctionTree GetSimplifiedFunction()
		{
			return new FunctionTree(rootNode.GetSimplifiedFunction());
		}

        private FVar tempVar;
        private FCons tempCons;
        private Function tempFunc;

        public bool VariableToValue(String varName, double cons)
        {
            if ((tempVar=FindVariable(varName))!=null)
            {
                tempCons = new FCons(cons);
                VarToVal(rootNode);
                varList.Remove(tempVar);
                tempVar = null;
                tempCons = null;
                GenerateFunctionTreeText(funcTreeTextStyle);
                return true;
            }
            return false;
        }

        public bool VariableToFunction(String varName, Function F)
        {
            if ((tempVar = FindVariable(varName)) != null)
            {
                tempFunc = F;
                VarToFunc(rootNode);
                varList.Remove(tempVar);
                foreach(FVar v in F.varList)
                {
                    AddVariable(v);
                }
                tempVar = null;
                tempCons = null;
                GenerateFunctionTreeText(funcTreeTextStyle);
                return true;
            }
            return false;
        }

        public bool VariableToSetValue(String varName)
        {
            if ((tempVar = FindVariable(varName)) != null && tempVar.IsSet())
            {
                tempCons = new FCons(tempVar.varValue);
                VarToVal(rootNode);
                varList.Remove(tempVar);
                tempVar = null;
                tempCons = null;
                GenerateFunctionTreeText(funcTreeTextStyle);
                return true;
            }
            return false;
        }

        private void VarToVal(Function F)
        {
            for (int i = 0; i < F.numSubNodes; i++)
            {
                Function Fnode = F.GetSubNode(i);

                if (Fnode.GetID() == FVar.ID)
                {
                    if (((FVar)Fnode).varName==tempVar.varName)
                    {
                        F.SetSubNode(i, tempCons);
                    }
                    continue;
                }
                if (Fnode.GetID().Equals(FCons.ID))
                {
                    continue;
                }
                VarToVal(F.GetSubNode(i));
            }
        }

        private void VarToFunc(Function F)
        {
            for (int i = 0; i < F.numSubNodes; i++)
            {
                Function Fnode = F.GetSubNode(i);

                if (Fnode.GetID() == FVar.ID)
                {
                    if (((FVar)Fnode).varName == tempVar.varName)
                    {
                        F.SetSubNode(i, tempFunc);
                    }
                    continue;
                }
                if (Fnode.GetID().Equals(FCons.ID))
                {
                    continue;
                }
                VarToFunc(F.GetSubNode(i));
            }
        }
    }

}