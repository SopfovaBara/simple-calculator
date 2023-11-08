using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using SimpleCalculator.Entities;
using SimpleCalculator.Types;

namespace SimpleCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<TokenEntity> tokens = new List<TokenEntity>()
        {
            new TokenEntity("0", TokenTypes.None)
        };
        private double ans = 0;
        private bool error = false;

        public List<RecordEntity> historyList { get; set; } 
            = new List<RecordEntity>();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Change_Theme_Button(object sender, RoutedEventArgs e)
        {
            Button themeButton = (Button)sender;

            Uri uri = new Uri(
                ((string)themeButton.Content == "☾")
                ? "/Resources/Themes/DarkTheme.xaml"
                : "/Resources/Themes/LightTheme.xaml", 
                UriKind.Relative);

            themeButton.Content = 
                ((string)themeButton.Content == "☾") ? "☀" : "☾";

            Application.Current.Resources.MergedDictionaries[0].Source = uri;
        }

        private void History_Button(object sender, RoutedEventArgs e)
        {
            var historyListBox = (ListBox)this.FindName("History_ListBox");
            historyListBox.Visibility = (historyListBox.IsVisible) 
                ? Visibility.Collapsed 
                : Visibility.Visible;
        }

        private void PushAns()
        {
            if (tokens[LastToken()].Type == TokenTypes.Ans)
            {
                var topTextBlock = (TextBlock)this.FindName("Top_TextBlock");
                topTextBlock.Text = tokens[LastToken()].Content;
                tokens[LastToken()].Type = TokenTypes.None;

                if (tokens[LastToken()].Content == "ERROR")
                {
                    var bottomTextBlock = (TextBlock)this.FindName("Bottom_TextBlock");

                    bottomTextBlock.Text = "0";
                    tokens[LastToken()].Content = "0";
                    error = false;
                }
            }
        }
        private void Backspace(int count)
        {
            var bottomTextBlock = (TextBlock)this.FindName("Bottom_TextBlock");

            bottomTextBlock.Text =
                bottomTextBlock.Text.Remove(bottomTextBlock.Text.Length - count);
        }
        private int LastToken()
        {
            return tokens.Count - 1;
        }
        private int LastChar(int token)
        {
            return tokens[token].Content.Length - 1;
        }

        private void ReplaceLastToken(string content, TokenTypes type)
        {
            var bottomTextBlock = (TextBlock)this.FindName("Bottom_TextBlock");

            Backspace(tokens[LastToken()].Content.Length);
            bottomTextBlock.Text += content;

            tokens[LastToken()].Content = content;
            tokens[LastToken()].Type = type;
        }
        private void CreateNewToken(string content, TokenTypes type)
        {
            var bottomTextBlock = (TextBlock)this.FindName("Bottom_TextBlock");

            tokens.Add(new TokenEntity(content, type));
            bottomTextBlock.Text += content;
        }
        private void AddToLastToken(string content)
        {
            var bottomTextBlock = (TextBlock)this.FindName("Bottom_TextBlock");

            tokens[LastToken()].Content += content;
            bottomTextBlock.Text += content;
        }

        private void CheckDecimal()
        {
            if (tokens[LastToken()].Type == TokenTypes.Decimal
                && tokens[LastToken()].Content[LastChar(LastToken())] == ',')
            {
                tokens[LastToken()].Content += "0";
            }
        }

        private void Num_Button_Click(object sender, RoutedEventArgs e)
        {
            string num = (string)((Button)sender).Content;


            PushAns();

            if (tokens[LastToken()].Type == TokenTypes.None)
            {
                ReplaceLastToken(num, TokenTypes.Number);
            }
            else if (tokens[LastToken()].Type == TokenTypes.Number
                || tokens[LastToken()].Type == TokenTypes.Decimal)
            {
                AddToLastToken(num);
            }
            else if (tokens[LastToken()].Type != TokenTypes.ClosePar)
            {
                CreateNewToken(num, TokenTypes.Number);
            }
        }

        private void Ans_Button_Click(object sender, RoutedEventArgs e)
        {
            string num = ans.ToString();
            TokenTypes type = (ans % 1 == 0)
                ? ((ans == 0) 
                    ? TokenTypes.None
                    : TokenTypes.Number)
                : TokenTypes.Decimal;


            PushAns();

            if (tokens[LastToken()].Type == TokenTypes.None
                || tokens[LastToken()].Type == TokenTypes.Number
                || tokens[LastToken()].Type == TokenTypes.Decimal)
            {
                ReplaceLastToken(num, type);
            }
            else if (tokens[LastToken()].Type != TokenTypes.ClosePar)
            {
                CreateNewToken(num, type);
            }
        }

        private void Zero_Button_Click(object sender, RoutedEventArgs e)
        {
            PushAns();

            if (tokens[LastToken()].Type == TokenTypes.None)
            {
                ReplaceLastToken("0", TokenTypes.None);
            }
            else if (tokens[LastToken()].Type == TokenTypes.Number
                || tokens[LastToken()].Type == TokenTypes.Decimal)
            {
                AddToLastToken("0");
            }
            else if (tokens[LastToken()].Type == TokenTypes.Operator
                || tokens[LastToken()].Type == TokenTypes.OpenPar)
            {
                CreateNewToken("0", TokenTypes.None);
            }
        }

        private void Op_Button_Click(object sender, RoutedEventArgs e)
        {
            string op = ((Button)sender).Content.ToString();


            PushAns();

            if (tokens[LastToken()].Type == TokenTypes.None)
            {
                tokens[LastToken()].Type = TokenTypes.Number;
                CreateNewToken(op, TokenTypes.Operator);
            }
            else if (tokens[LastToken()].Type == TokenTypes.Operator)
            {
                ReplaceLastToken(op, TokenTypes.Operator);
            }
            else if (tokens[LastToken()].Type != TokenTypes.OpenPar)
            {
                CheckDecimal();
                CreateNewToken(op, TokenTypes.Operator);
            }
        }

        private void Sqrt_Button_Click(object sender, RoutedEventArgs e)
        {
            PushAns();

            if (tokens[LastToken()].Type == TokenTypes.None)
            {
                ReplaceLastToken("√", TokenTypes.Operator);
            }
            else if (tokens[LastToken()].Type == TokenTypes.Number
                || tokens[LastToken()].Type == TokenTypes.Decimal
                || tokens[LastToken()].Type == TokenTypes.ClosePar)
            {
                CheckDecimal();
                CreateNewToken("×", TokenTypes.Operator);
            }
            else
            {
                CreateNewToken("√", TokenTypes.Operator);
            }
        }

        private void Dot_Button_Click(object sender, RoutedEventArgs e)
        {
            if (tokens[LastToken()].Type == TokenTypes.Ans
                && ans % 1 == 0)
            {
                PushAns();
            }
            
            if (tokens[LastToken()].Type == TokenTypes.None
                || tokens[LastToken()].Type == TokenTypes.Number)
            {
                ReplaceLastToken($"{tokens[LastToken()].Content},", TokenTypes.Decimal);
            }
        }

        private void OpenPar_Button_Click(object sender, RoutedEventArgs e)
        {
            PushAns();

            if (tokens[LastToken()].Type == TokenTypes.None)
            {
                ReplaceLastToken("(", TokenTypes.OpenPar);
            }
            else
            {
                if (tokens[LastToken()].Type != TokenTypes.Operator
                    && tokens[LastToken()].Type != TokenTypes.OpenPar)
                {
                    CheckDecimal();
                    CreateNewToken("×", TokenTypes.Operator);
                }

                CreateNewToken("(", TokenTypes.OpenPar);
            }
        }

        private void ClosePar_Button_Click(object sender, RoutedEventArgs e)
        {
            List<TokenEntity> open = tokens.FindAll(token => token.Content.Equals("("));
            List<TokenEntity> close = tokens.FindAll(token => token.Content.Equals(")"));


            if (close.Count != open.Count)
            {
                PushAns();

                if (tokens[LastToken()].Type == TokenTypes.None)
                {
                    tokens[LastToken()].Type = TokenTypes.Number;
                    CreateNewToken(")", TokenTypes.ClosePar);
                }
                else if (tokens[LastToken()].Type == TokenTypes.Number
                    || tokens[LastToken()].Type == TokenTypes.Decimal
                    || tokens[LastToken()].Type == TokenTypes.ClosePar)
                {
                    CheckDecimal();
                    CreateNewToken(")", TokenTypes.ClosePar);
                }
            }
        }

        private double Pwr(double a, double b) { return Math.Pow(a, b); }
        private double Mod(double a, double b) { return a % b; }
        private double Div(double a, double b) 
        { 
            if (b == 0)
            {
                error = true;
                return 0;
            }
            return a / b; 
        }
        private double Mul(double a, double b) { return a * b; }
        private double Sub(double a, double b) { return a - b; }
        private double Add(double a, double b) { return a + b; }

        private void DoMath(int start, int end)
        {
            int index;
            double a, b, c;

            string[] ops = new string[]
            {
                "^", "%", "/", "×", "-", "+"
            };

            Func<double, double, double>[] fcs = new Func<double, double, double>[]
            {
                (x, y) => Pwr(x, y),
                (x, y) => Mod(x, y),
                (x, y) => Div(x, y),
                (x, y) => Mul(x, y),
                (x, y) => Sub(x, y),
                (x, y) => Add(x, y)
            };


            while ((index = tokens.FindIndex(start, end - start + 1, token => token.Content.Equals("√"))) != -1)
            {
                a = double.Parse(tokens[index + 1].Content);

                if (a < 0)
                {
                    error = true;
                    return;
                }

                c = Math.Sqrt(a);

                tokens[index].Content = c.ToString();
                tokens[index].Type = (c % 1 == 0)
                    ? TokenTypes.Number
                    : TokenTypes.Decimal;

                tokens.RemoveAt(index + 1);
                end--;
            }

            for (int i = 0; i < ops.Length; i++)
            {
                if (start == end) break;
                
                while ((index = tokens.FindIndex(start, end - start + 1, token => token.Content.Equals(ops[i]))) != -1)
                {
                    a = double.Parse(tokens[index - 1].Content);
                    b = double.Parse(tokens[index + 1].Content);
                    c = fcs[i](a, b);

                    if (error)
                    {
                        return;
                    }

                    tokens[index - 1].Content = c.ToString();
                    tokens[index - 1].Type = (c % 1 == 0)
                        ? TokenTypes.Number
                        : TokenTypes.Decimal;

                    tokens.RemoveAt(index + 1);
                    tokens.RemoveAt(index);
                    end -= 2;
                }
            }
        }

        private void FindParentheses(int start)
        {
            int openIndex, closeIndex;

            while ((openIndex = tokens.FindIndex(start, token => token.Type.Equals(TokenTypes.OpenPar))) != -1 
                && (closeIndex = tokens.FindIndex(start, token => token.Type.Equals(TokenTypes.ClosePar))) > openIndex)
            {
                start = ResolveParentheses(openIndex, closeIndex);
            }
        }

        private int ResolveParentheses(int start, int closeIndex)
        {
            int openIndex = tokens.FindIndex(start + 1, token => token.Type.Equals(TokenTypes.OpenPar));

            if (openIndex != -1 && openIndex < closeIndex)
            {
                FindParentheses(openIndex);
                closeIndex = tokens.FindIndex(start, token => token.Type.Equals(TokenTypes.ClosePar));
            }

            DoMath(start + 1, closeIndex - 1);
            tokens.RemoveAt(start);
            tokens.RemoveAt(start + 1);

            return start;
        }

        private bool CheckParentheses()
        {
            List<TokenEntity> open = tokens.FindAll(token => token.Content.Equals("("));
            List<TokenEntity> close = tokens.FindAll(token => token.Content.Equals(")"));

            return open.Count == close.Count;
        }

        private void Equals_Button_Click(object sender, RoutedEventArgs e)
        {
            var topTextBlock = (TextBlock)this.FindName("Top_TextBlock");
            var bottomTextBlock = (TextBlock)this.FindName("Bottom_TextBlock");
            var historyListBox = (ListBox)this.FindName("History_ListBox");


            CheckDecimal();

            if (!CheckParentheses() || tokens[LastToken()].Type == TokenTypes.Operator)
            {
                topTextBlock.Text = "SYNTAX ERROR";
                return;
            }

            FindParentheses(0);
            DoMath(0, tokens.Count - 1);

            topTextBlock.Text = bottomTextBlock.Text;
            topTextBlock.Text += " =";

            if (error)
            {
                ans = 0;
                bottomTextBlock.Text = "ERROR";

                tokens = new List<TokenEntity>
                {
                    new TokenEntity("ERROR", TokenTypes.Ans)
                };

                return;
            }

            ans = double.Parse(tokens[0].Content);

            bottomTextBlock.Text = tokens[0].Content;
            tokens[0].Type = TokenTypes.Ans;

            historyList.Add(new RecordEntity { Content = topTextBlock.Text, Ans = ans });
            historyListBox.Items.Refresh();
        }

        private void Clear_Button_Click(object sender, RoutedEventArgs e)
        {
            var topTextBlock = (TextBlock)this.FindName("Top_TextBlock");
            var bottomTextBlock = (TextBlock)this.FindName("Bottom_TextBlock");

            tokens = new List<TokenEntity>
            {
                new TokenEntity("0", TokenTypes.None)
            };

            topTextBlock.Text = "";
            bottomTextBlock.Text = "0";
        }

        private void Backspace_Button_Click(object sender, RoutedEventArgs e)
        {
            var bottomTextBlock = (TextBlock)this.FindName("Bottom_TextBlock");


            if (bottomTextBlock.Text.Length == 1)
            {
                ReplaceLastToken("0", TokenTypes.None);
            }
            else
            {
                if (tokens[LastToken()].Type == TokenTypes.Decimal
                    && tokens[LastToken()].Content[LastChar(LastToken())] == ',')
                {
                    tokens[LastToken()].Type = TokenTypes.Number;
                }

                if (tokens[LastToken()].Content.Length == 1)
                {
                    tokens.RemoveAt(LastToken());
                }
                else
                {
                    tokens[LastToken()].Content =
                        tokens[LastToken()].Content.Remove(LastChar(LastToken()));
                }

                Backspace(1);
            }

            if (tokens[LastToken()].Content == "0")
            {
                tokens[LastToken()].Type = TokenTypes.None;
            }
        }

        private void History_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var historyListBox = (ListBox)this.FindName("History_ListBox");

            if (historyListBox.SelectedIndex == -1) return;

            var topTextBlock = (TextBlock)this.FindName("Top_TextBlock");
            var bottomTextBlock = (TextBlock)this.FindName("Bottom_TextBlock");


            tokens = new List<TokenEntity>
            {
                new TokenEntity(historyList[historyListBox.SelectedIndex].Ans.ToString(), TokenTypes.Ans)
            };

            ans = historyList[historyListBox.SelectedIndex].Ans;

            topTextBlock.Text = historyList[historyListBox.SelectedIndex].Content;
            bottomTextBlock.Text = ans.ToString();

            historyListBox.SelectedIndex = -1;
            History_Button(sender, e);
        }
    }
}
