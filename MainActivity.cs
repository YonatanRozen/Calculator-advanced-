using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using System;
using Android.Content;
using Android.Views;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using Xamarin.Essentials;
using Javax.Net.Ssl;
using Android.Database.Sqlite;
using Android.Icu.Text;

namespace Calculator_Complex_
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, Android.Views.View.IOnClickListener
    {
        private Button btn_AC, btn_Times, btn_Plus, btn_Minus, btn_TAN, btn_SIN, btn_COS, btn_ShowResult, btn_Divide, btn_Dot, btn_LeftParenthesis, btn_RightParenthesis, btn_Pow, btn_CE;
        private TextView tv_Result;
        private Button[] numbers;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            btn_AC = FindViewById<Button>(Resource.Id.btnAC);
            btn_Times = FindViewById<Button>(Resource.Id.btnTimes);
            btn_Divide = FindViewById<Button>(Resource.Id.btnDivide);
            btn_Plus = FindViewById<Button>(Resource.Id.btnPlus);
            btn_Minus = FindViewById<Button>(Resource.Id.btnMinus);
            btn_TAN = FindViewById<Button>(Resource.Id.btnTAN);
            btn_SIN = FindViewById<Button>(Resource.Id.btnSIN);
            btn_COS = FindViewById<Button>(Resource.Id.btnCOS);
            btn_ShowResult = FindViewById<Button>(Resource.Id.btnShowResult);
            btn_Dot = FindViewById<Button>(Resource.Id.btnDot);
            tv_Result = FindViewById<TextView>(Resource.Id.tv_Result);
            btn_LeftParenthesis = FindViewById<Button>(Resource.Id.btnLeftParenthesis);
            btn_RightParenthesis = FindViewById<Button>(Resource.Id.btnRightParenthesis);
            btn_CE = FindViewById<Button>(Resource.Id.btnCE);
            btn_Pow = FindViewById<Button>(Resource.Id.btnPow);

            SetClicks();

            this.numbers = new Button[10];

            int resID;
            for (int i = 0; i <= 9; i++)
            {
                resID = base.Resources.GetIdentifier("btn" + i, "id", PackageName);
                Button num = FindViewById<Button>(resID);
                this.numbers[i] = num;
                num.SetOnClickListener((View.IOnClickListener)this);
            }

        }

        private void SetClicks()
        {
            btn_AC.SetOnClickListener(this);
            btn_Times.SetOnClickListener(this);
            btn_Plus.SetOnClickListener(this);
            btn_Minus.SetOnClickListener(this);
            btn_Divide.SetOnClickListener(this);
            btn_ShowResult.SetOnClickListener(this);
            btn_LeftParenthesis.SetOnClickListener(this);
            btn_RightParenthesis.SetOnClickListener(this);
            btn_Pow.SetOnClickListener(this);
            btn_CE.SetOnClickListener(this);
            btn_Dot.SetOnClickListener(this);
            btn_SIN.SetOnClickListener(this);
            btn_TAN.SetOnClickListener(this);
            btn_COS.SetOnClickListener(this);
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public float Calculate(string expression)
        {
            char[] ex = expression.ToCharArray();

            Stack<float> values = new Stack<float>();

            Stack<char> ops = new Stack<char>();

            for (int i = 0; i < ex.Length; i++)
            {
                if (ex[i] == ' ')
                    continue;
                if (ex[i] >= '0' && ex[i] <= '9')
                {
                    string s = "";

                    while (i < ex.Length && ((ex[i] >= '0' && ex[i] <= '9') || ex[i] == '.') )
                        s += ex[i++].ToString();

                    values.Push(float.Parse(s));

                }

                if ( i < ex.Length && ex[i] == '(')
                    ops.Push(ex[i]);

                if ( i < ex.Length && ex[i] == ')')
                {

                    while (ops.Peek() != '(')
                    {
                        float a = values.Pop();
                        float b = values.Pop();
                        char op = ops.Pop();

                        if (op == '/' && a == 0)
                        {
                            Toast.MakeText(this, "Can't divide by zero", ToastLength.Short).Show();
                            return 0;
                        }

                        else
                            values.Push(ApplyOperator(a, b, op));
                    }

                    ops.Pop();
                }

                else if (i < ex.Length && (ex[i] == '+' || ex[i] == '-' || ex[i] == '*' || ex[i] == '/'))
                {
                    while (ops.Count() > 0 && HasPrecedence(ex[i], ops.Peek()))
                    {
                        float a = values.Pop();
                        float b = values.Pop();
                        char op = ops.Pop();

                        if (op == '/' && a == 0)
                        {
                            Toast.MakeText(this, "Can't divide by zero", ToastLength.Short).Show();
                            return 0;
                        }

                        else
                            values.Push(ApplyOperator(a, b, op));
                    }
                    
                    ops.Push(ex[i]);  
                }
            }

            
            while (ops.Count() > 0)
            {
                float a = values.Pop();
                float b = values.Pop();
                char op = ops.Pop();

                if (op == '/' && a == 0)
                {
                    Toast.MakeText(this, "Can't divide by zero", ToastLength.Short).Show();
                    return 0;
                }

                else
                    values.Push(ApplyOperator(a, b, op));
            }

            return values.Pop();

        }

        public void OnClick(View sender)
        {
            for (int i = 0; i < this.numbers.Length; i++)
            {
                if (sender == this.numbers[i])
                {
                    if (tv_Result.Text == "0")
                        tv_Result.Text = i.ToString();
                    else
                        tv_Result.Text += i.ToString();
                }
            }
            if (sender == btn_AC)
            {
                tv_Result.Text = "0";
            }
            if (sender == btn_CE)
            {

                if (tv_Result.Text.Length == 1)
                    tv_Result.Text = "0";
                else
                    tv_Result.Text = tv_Result.Text.Substring(0, tv_Result.Text.Length - 1);
            }
            if (sender == btn_Pow)
            {
                int i = tv_Result.Text.Length - 1; 

                while (i > 0 &&
                    tv_Result.Text[i] != '+' &&
                    tv_Result.Text[i] != '-' &&
                    tv_Result.Text[i] != '*' &&
                    tv_Result.Text[i] != '/')
                    i--;
                if (i != tv_Result.Text.Length - 1)
                    tv_Result.Text += "*" + float.Parse(tv_Result.Text.Substring(i));
                else
                    Toast.MakeText(this, "Please make sure last chracter is an operand and not an operator!", ToastLength.Short).Show();
            }
            if (sender == btn_Minus)
            {
                tv_Result.Text += "-";
            }
            if (sender == btn_Plus)
            {
                tv_Result.Text += "+";
            }
            if (sender == btn_Times)
            {
                tv_Result.Text += "*";
            }
            if (sender == btn_Divide)
            {
                tv_Result.Text += "/";
            }
            if (sender == btn_LeftParenthesis)
            {
                if (tv_Result.Text == "0")
                    tv_Result.Text = "(";
                else
                    tv_Result.Text += "(";
            }
            if (sender == btn_RightParenthesis)
            {
                if (tv_Result.Text != "0")
                    tv_Result.Text += ")";
            }
            if (sender == btn_Dot)
            {
                tv_Result.Text += ".";
            }
            if (sender == btn_ShowResult)
            {
                int length = tv_Result.Text.Length;
                if (tv_Result.Text[length - 1] == '+' ||
                    tv_Result.Text[length - 1] == '-' ||
                    tv_Result.Text[length - 1] == '*' ||
                    tv_Result.Text[length - 1] == '/')
                {
                    Toast.MakeText(this, "Please make sure last chracter is an operand and not an operator!", ToastLength.Short).Show();

                }
                else
                    tv_Result.Text = Calculate(tv_Result.Text).ToString();
            }
        }

        public static float ApplyOperator(float a, float b, char op)
        {
            switch (op)
            {
                
                case '+':
                    return a + b;
                case '-':
                    return b - a;
                case '*':
                    return a * b;
                case '/':
                    return b / a;
            }

            return 0;
        }
        public static bool HasPrecedence(char op1, char op2)
        {
            if (op2 == '(' || op2 == ')')
            {
                return false;
            }
            if ((op1 == '*' || op1 == '/') && (op2 == '+' || op2 == '-'))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
