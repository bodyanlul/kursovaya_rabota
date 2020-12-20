using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace knowledgetester
{
    public partial class testing : Form
    {
        abstract class BaseQuestion
        {
            public string Quest;
            public abstract bool CheckAnswer(string answer);
            public object Answers;
            public object CorrentAnswer;
        }

        class AQuestion : BaseQuestion
        {
            public new List<string> Answers = new List<string>();
            public new int CorrentAnswer;

            public override bool CheckAnswer(string answer)
            {
                return Convert.ToInt32(answer) == this.CorrentAnswer;
            }
        }

        class BQuestion : BaseQuestion
        {
            public new List<string> Answers = new List<string>();
            public new List<int> CorrentAnswer;

            public override bool CheckAnswer(string answer)
            {
                string pattern = @"(\d+)";
                var answers = new List<int>();
                foreach (Match match in Regex.Matches(answer, pattern))
                {
                    answers.Add(Convert.ToInt32(match.Value));
                }
                answers.Sort();

                if (answers.Count != CorrentAnswer.Count)
                {
                    return false;
                }

                for (int i = 0; i < answers.Count; i++)
                {
                    if (answers[i] != CorrentAnswer[i])
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        class CQuestion : BaseQuestion
        {
            public new object Answers = null;
            public new string CorrentAnswer;

            public override bool CheckAnswer(string answer)
            {
                return answer == this.CorrentAnswer;
            }
        }

        List<BaseQuestion> allqlist = new List<BaseQuestion>();
        List<BaseQuestion> currentqlist = new List<BaseQuestion>();
        int currentquestionnumber = 0;
        int correctanswers = 0;
        int type = 0;

        public testing()
        {
            InitializeComponent();
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0); //завершение программы 
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            ParseQuestionsFromList(File.ReadAllLines("File.txt", Encoding.GetEncoding("windows-1251")));
            Restart(); 
        }
        void ParseQuestionsFromList(IList<string> stringlist)
        {
            int n = 1;
            for (int i = 0; i < stringlist.Count; i++)
            {
                n = 1;
                char letter = stringlist[i][0];

                switch (letter)
                {
                    case 'A':
                        var aq = new AQuestion()
                        {
                            Quest = stringlist[i]
                        };
                        while (!stringlist[i + n].Contains("Ответ:"))
                        {
                            aq.Answers.Add(stringlist[i + n]);
                            n++;
                        }
                        aq.CorrentAnswer = int.Parse(stringlist[i + n].Split(new char[1] { ':' }, StringSplitOptions.RemoveEmptyEntries)[1]);
                        n++;
                        allqlist.Add(aq);
                        break;
                    case 'B':
                        var bq = new BQuestion()
                        {
                            Quest = stringlist[i]
                        };
                        while (!stringlist[i + n].Contains("Ответ:"))
                        {
                            bq.Answers.Add(stringlist[i + n]);
                            n++;
                        }
                        string pattern = @"(\d+)";
                        var answers = new List<int>();
                        foreach (Match match in Regex.Matches(stringlist[i + n], pattern))
                        {
                            answers.Add(Convert.ToInt32(match.Value));
                        }
                        answers.Sort();
                        bq.CorrentAnswer = answers;
                        n++;
                        allqlist.Add(bq);
                        break;
                    case 'C':
                        CQuestion cq = new CQuestion()
                        {
                            Quest = stringlist[i]
                        };
                        cq.CorrentAnswer = stringlist[i + n].Split(new char[1] { ':' }, StringSplitOptions.RemoveEmptyEntries)[1];
                        cq.CorrentAnswer = cq.CorrentAnswer.Trim();
                        allqlist.Add(cq);
                        break;
                    default:
                        continue;
                }
            }
        }
        void NextQuestion()
        {
            switch (type)
            {
                case 0:
                    break;
                case 1:
                    foreach (RadioButton rbb in this.Controls.OfType<RadioButton>())
                    {
                        if (rbb.Checked)
                        {
                            string pattern = @"(\d+)";
                            var match = Regex.Match(rbb.Text, pattern, RegexOptions.Singleline);
                           
                            if (currentqlist[currentquestionnumber - 1].CheckAnswer(match.Value))
                            {
                                correctanswers++;
                            }
                        }
                    }
                    for (int i = 0; i < this.Controls.Count; i++)
                    {
                        if (this.Controls[i].GetType() == typeof(RadioButton))
                        {
                            this.Controls[i].Dispose(); i--;
                        }
                    }
                    break;
                case 2:
                    string answer = "";
                    foreach (CheckBox rbb in this.Controls.OfType<CheckBox>())
                    {
                        if (rbb.Checked)
                        {
                            string pattern = @"(\d+)";
                            var match = Regex.Match(rbb.Text, pattern, RegexOptions.Singleline);

                            answer += match.Value + ", ";
                        }
                    }
                    if (currentquestionnumber != 0 && currentqlist[currentquestionnumber - 1].CheckAnswer(answer))
                    {
                        correctanswers++;
                    }
                    for (int i = 0; i < this.Controls.Count; i++)
                    {
                        if (this.Controls[i].GetType() == typeof(CheckBox))
                        {
                            this.Controls[i].Dispose(); i--;
                        }
                    }
                    break;
                case 3:
                    foreach (TextBox rbb in this.Controls.OfType<TextBox>())
                    {
                        if (rbb.TextLength != 0)
                        {
                            if (currentqlist[currentquestionnumber - 1].CheckAnswer(rbb.Text))
                            {
                                correctanswers++;
                            }
                        }
                    }
                    for (int i = 0; i < this.Controls.Count; i++)
                    {
                        if (this.Controls[i].GetType() == typeof(TextBox))
                        {
                            this.Controls[i].Dispose(); i--;
                        }
                    }
                    break;
            }

            if (currentquestionnumber >= currentqlist.Count)
            {
                MessageBox.Show(String.Format("Тест окончен! Вы набрали {0} правильных ответов из {1} возможных", correctanswers, currentqlist.Count));
                Restart();
                Environment.Exit(0); //завершение программы
                return;
            }

            label2.Text = String.Format("Вопрос {0} из {1}", currentquestionnumber + 1, currentqlist.Count);
            var q = currentqlist[currentquestionnumber];

            

            if (q.GetType() == typeof(AQuestion))
            {
                type = 1;
                AQuestion aq = q as AQuestion;
                label1.Text = aq.Quest;
                Random r = new Random();

                for (int i = 0; i < aq.Answers.Count; i++)
                {
                    var rb = new RadioButton()
                    {
                        Text = aq.Answers[i],
                        Location = new Point(50, 100 + i * 30),
                        Width = 1000
                    };
                    rb.CheckedChanged += (s, ee) =>
                    {
                        if (rb.Checked)
                        {
                            foreach (RadioButton rbb in this.Controls.OfType<RadioButton>())
                            {
                                if (s != rbb) rbb.Checked = false;
                            }
                        }
                    };
                    this.Controls.Add(rb);
                }
            }
            else if (q.GetType() == typeof(BQuestion))
            {
                type = 2;
                BQuestion bq = q as BQuestion;
                label1.Text = bq.Quest;
                Random r = new Random();

                for (int i = 0; i < bq.Answers.Count; i++)
                {
                    var rb = new CheckBox()
                    {
                        Text = bq.Answers[i],
                        Location = new Point(50, 100 + i * 30),
                        Width = 1000
                    };
                    this.Controls.Add(rb);
                }
            } 
            else
            {
                type = 3;
                CQuestion cq = q as CQuestion;
                label1.Text = cq.Quest;
                var inp = new TextBox()
                {
                    Location = new Point(50, 130),
                    Width = 1000
                };
                this.Controls.Add(inp);
            }

            this.Invalidate();
            currentquestionnumber++;
        }
        void Restart()
        {
            Random r = new Random();
            currentqlist = allqlist.OrderBy(x => r.Next()).Take(20).ToList(); //случайный порядок вопросов
            //currentqlist = allqlist; //порядок вопросов как в текстовом файле
            currentquestionnumber = 0;
            correctanswers = 0;
            NextQuestion();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            NextQuestion();
        }
    }
}
