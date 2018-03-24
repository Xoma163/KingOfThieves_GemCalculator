using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GemCalc
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //Кол-во общих возможных исходов
        int good_Combinations = 0;
        //Массив общих возможных исходов
        string[] good_Combinations_arr;
        //Кол-во благоприятных исходовв
        int good_Combinations_minmax = 0;
        //Массив благоприятных исходов
        int[,] all_Combinations;

        //Список камней
        int[] gems;
        //Мин и макс значения для выборки
        int max;
        int min;
        //Макс итераций
        int max_iter;
        //Макс результатов
        int max_res;
        //Кол-во камней
        int gems_count;
        //Бонус гильды
        int guild_bonus;

        int summ=0;
        bool iWasHere = true;

        //Массив для подсчёта количества благоприятных исходов
        int[] tempMas;

        //Добиваем до нужного количества нулей
        private string AddString0(int count)
        {
            string text_0 = "";
            for (int i = 0; i < count; i++)
                text_0 += "0";
            return text_0;
        }
        //проверка на нечётность и >3
        private bool IsOdd(int n)

        {
            if ((n % 2 == 1) && (n > 1))
                return true;
            else
                return false;
        }
        //Возвращает количество единиц из двоичного представления
        private int Count_1(string s)
        {
            return s.Split(new string[] { "1" }, StringSplitOptions.None).Count() - 1;
        }

        //жмяк по кнопке. Точка входа
        private void button1_Click(object sender, EventArgs e)
        { 
            
            int count = dgv.Columns.Count;
            for (int i = 0; i < count; i++)
            {
                dgv.Columns.RemoveAt(dgv.Columns.Count-1);
            }

            if (!iWasHere)
            {
                this.Width -= summ + 35;
                dgv.Width -= summ;
            }
            iWasHere = false;
            // dgv.Rows.Clear();


            gems = new int[richTextBox1.Lines.Length];
            //Костыль для считывания списка камней с richTextBox
            string[] gems_string = new string[richTextBox1.Lines.Length];
            max = Convert.ToInt32(textBoxMax.Text);
            min = Convert.ToInt32(textBoxMin.Text);
            max_iter = Convert.ToInt32(textBoxMaxIter.Text);
            max_res = Convert.ToInt32(textBoxResults.Text);
            gems_count = gems.Length;
            guild_bonus = Convert.ToInt32(textBoxGuild.Text);


            //Считывание камней с richTextBox
            gems_string = richTextBox1.Text.Split();
            for (int i = 0; i < gems_count; i++)
                Int32.TryParse(gems_string[i],out gems[i]);
            //gems[i] =  Convert.ToInt32(gems_string[i]);


            //Узнаю общее число возможных крафтов
            good_Combinations_arr = new string[good_Combinations];
            good_Combinations = 0;
            for (int i = 0; i < Math.Pow(2, gems_count); i++)
            {
                if (IsOdd(Count_1(Convert.ToString(i, 2))))
                    good_Combinations++;
            }


            //Записываю всевозможные крафты в один массив good_Combinations_arr
            good_Combinations_arr = new string[good_Combinations];
            good_Combinations = 0;
            string binary = "";
            for (int i = 0; i < Math.Pow(2, gems_count); i++)
            {
                binary = Convert.ToString(i, 2);
                binary = AddString0(gems_count - binary.Length) + binary;

                if (IsOdd(Count_1(binary)))
                {
                    good_Combinations_arr[good_Combinations] = binary;
                    good_Combinations++;
                }
            }


            //Считаю количество благоприятных, т.е. [min;max]
            good_Combinations_minmax = 0;
            tempMas = new int[gems_count + 2];

            for (int k = 0; k < good_Combinations; k++)
            {
                Array.Clear(tempMas, 0, tempMas.Length);
                for (int i = 0; i < gems_count; i++)
                {
                    tempMas[i] = gems[i] * (int)Char.GetNumericValue(good_Combinations_arr[k][i]);
                    tempMas[gems_count] += tempMas[i];
                    if (tempMas[i] != 0)
                        tempMas[gems_count + 1] += 1;
                }
                tempMas[gems_count + 1] = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(tempMas[gems_count + 1]) / 2D - 1));
                tempMas[gems_count] += tempMas[gems_count + 1] * guild_bonus;
                if (tempMas[gems_count] >= min && tempMas[gems_count] <= max)
                    good_Combinations_minmax++;
            }


            //Записываю в массив all_Combinations все благоприятные, т.е. [min;max]
            all_Combinations = new int[gems_count + 2, good_Combinations_minmax];
            int kk = 0;

            for (int k = 0; k < good_Combinations; k++)
            {
                Array.Clear(tempMas, 0, tempMas.Length);

                for (int i = 0; i < gems_count; i++)
                {
                    tempMas[i] = gems[i] * (int)Char.GetNumericValue(good_Combinations_arr[k][i]);
                    tempMas[gems_count] += tempMas[i];
                    if (tempMas[i] != 0)
                        tempMas[gems_count + 1] += 1;
                }
                tempMas[gems_count + 1] = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(tempMas[gems_count + 1]) / 2D - 1));
                tempMas[gems_count] += tempMas[gems_count + 1] * guild_bonus;
                if (tempMas[gems_count] >= min && tempMas[gems_count] <= max)
                {
                    for (int i = 0; i < gems_count; i++)
                    {
                        all_Combinations[i, kk] = gems[i] * (int)Char.GetNumericValue(good_Combinations_arr[k][i]);
                        all_Combinations[gems_count, kk] += all_Combinations[i, kk];
                        if (all_Combinations[i, kk] != 0)
                            all_Combinations[gems_count + 1, kk] += 1;
                    }
                    all_Combinations[gems_count + 1, kk] = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(all_Combinations[gems_count + 1, kk]) / 2D - 1));
                    all_Combinations[gems_count, kk] += all_Combinations[gems_count + 1, kk] * guild_bonus;

                    kk++;
                }
            }


            //Сортирую полученный массив благоприятных условий
            for (int i = 0; i < good_Combinations_minmax; i++)
                for (int k = 0; k < good_Combinations_minmax - i - 1; k++)
                    if (all_Combinations[gems_count, k] < all_Combinations[gems_count, k + 1])
                        for (int j = 0; j < gems_count + 2; j++)
                        {
                            int temp = all_Combinations[j, k];
                            all_Combinations[j, k] = all_Combinations[j, k + 1];
                            all_Combinations[j, k + 1] = temp;
                        }

            //Записываю полученный массив в dgv
            dgv.Columns.Add("#", "#");
            dgv.Columns["#"].Width = 14+ Convert.ToString(good_Combinations_minmax).Length*5;
            for(int i=0;i<gems_count;i++)
            { 
                dgv.Columns.Add(Convert.ToString(i), "" + Convert.ToString(i+1));
                dgv.Columns[i+1].Width = 45;
            }
            dgv.Columns.Add(Convert.ToString(gems_count), "Sum");
            dgv.Columns[Convert.ToString(gems_count)].Width = 45;
            dgv.Columns.Add(Convert.ToString(gems_count + 1), "Iter");
            dgv.Columns[Convert.ToString(gems_count + 1)].Width = 35;
            int num = 0;

            for (int k = 0; k < good_Combinations_minmax; k++)
            {
                if (max_iter == -1 || all_Combinations[gems_count + 1, k] <= max_iter)
                {
                    dgv.Rows.Add();
                    if (max_res == -1)
                        dgv.Rows[num].Cells[0].Value = AddString0(Convert.ToString(good_Combinations_minmax).Length - Convert.ToString(num + 1).Length) + (Convert.ToString(num + 1));
                    else
                        dgv.Rows[num].Cells[0].Value = AddString0(Convert.ToString(max_res).Length - Convert.ToString(num + 1).Length) + (Convert.ToString(num + 1));
                    for (int i = 0; i < gems.Count() + 2; i++)
                    {
                        if (i != gems_count)
                            dgv.Rows[num].Cells[Convert.ToString(i)].Value = Convert.ToString(all_Combinations[i, k]);
                        else
                            dgv.Rows[num].Cells[Convert.ToString(i)].Value = AddString0(Convert.ToString(max).Length - Convert.ToString(all_Combinations[i, k]).Length) + Convert.ToString(all_Combinations[i, k]);
                    }
                    num++;
                }
                if (num == max_res)
                    break;
            }
            summ = dgv.Columns["#"].Width+ gems_count*45+ dgv.Columns[Convert.ToString(gems_count)].Width+ dgv.Columns[Convert.ToString(gems_count + 1)].Width+50;
            this.Width += summ+35;
            dgv.Width += summ;

        }       

        //Методы загрузки/выгрузки
        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogToFile();
        }
        private void загрузитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            StreamReader sr = new StreamReader("Gems.txt");
            int count = Convert.ToInt32(sr.ReadLine());
            for (int i = 0; i < count; i++)
            {
                if (i != count - 1)
                    richTextBox1.AppendText(sr.ReadLine() + Environment.NewLine);
                else
                    richTextBox1.AppendText(sr.ReadLine());
            }

            textBoxMin.Text = sr.ReadLine();
            textBoxMax.Text = sr.ReadLine();
            textBoxMaxIter.Text = sr.ReadLine();
            textBoxGuild.Text = sr.ReadLine();
            textBoxResults.Text = sr.ReadLine();

            sr.Close();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            LogToFile();
        }
        private void LogToFile()
        {
            StreamWriter sw = new StreamWriter("Gems.txt");
            sw.WriteLine(richTextBox1.Lines.Length);

            for (int i = 0; i < richTextBox1.Lines.Length; i++)
                sw.WriteLine(richTextBox1.Lines[i]);

            sw.WriteLine(textBoxMin.Text);
            sw.WriteLine(textBoxMax.Text);
            sw.WriteLine(textBoxMaxIter.Text);
            sw.WriteLine(textBoxGuild.Text);
            sw.WriteLine(textBoxResults.Text);

            sw.Close();
        }

    }
}
