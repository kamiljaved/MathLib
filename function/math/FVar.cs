using System;

namespace function.math
{

	public class FVar : Function
	{
		public const string ID = "variable";
		public const string Symbol = "var";

		public string varName;
		public double varValue;
        public bool hasValue = false;

        public FVar(string varName) : base()
        {
            isVariable = true;
            this.varName = varName;
            varList.Add(this);
            numSubNodes = 0;
            SubNodes = null;
        }

        public void SetValue(double val)
		{
			this.varValue = val;
			hasValue = true;
		}

		public bool IsSet()
		{
			return hasValue;
		}

		public string GetVarName()
		{
			return varName;
		}

		public override double GetValue()
		{
            if (hasValue)
            {
                return varValue;
            }
            else return 0;  // Not Associated with the FVar

			//Console.Write("Enter value for " + varName + ": ");
			//hasValue = true;
			//varValue = double.Parse(Console.ReadLine());
            //return varValue;
		}

		public override Function GetDerivative(FVar var)
		{
			if (this.HasVariable(var))
			{
				return new FCons(1);
			}

			return new FCons(0);
		}

		public override Function GetIntegral(FVar var)
		{
			return null;
		}

		public override string ToString()
		{
			return varName;
		}

		public override string GetID()
		{
			return ID;
		}

		public override string GetSymbol()
		{
			return Symbol;
		}

		public override Function GetCopy()
		{
			return this;
		}

		public override Function GetSimplifiedFunction()
		{
			return this.GetCopy();
		}

        public override bool EqualsSymbolically(Function F)
        {
            throw new NotImplementedException();
        }

        public override bool EqualsNumerically(Function F)
        {
            throw new NotImplementedException();
        }
    }

}