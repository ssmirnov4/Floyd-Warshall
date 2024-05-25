using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace WinFormsApp15

{
    public partial class Form1 : Form
    {
        string[] headers = new string[676];
        int vertices;
        int inf = 9999;

        public Form1()
        {
            InitializeComponent();
            numericUpDown1.Value = 2;

        }



        private void numericUpDown1_ValueChanged_1(object sender, EventArgs e)
        {
            vertices = (int)numericUpDown1.Value;
            dataGridView1.RowCount = dataGridView1.ColumnCount = vertices;
            dataGridView2.RowCount = dataGridView2.ColumnCount = vertices;
            int index = 0;
            for (char c1 = 'A'; c1 <= 'Z'; c1++)
            {
                for (char c2 = 'A'; c2 <= 'Z'; c2++)
                {
                    headers[index++] = $"{c1}{c2}";
                }
            }

            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                dataGridView1.Columns[i].HeaderText = headers[i];
                dataGridView1.Rows[i].HeaderCell.Value = headers[i];
                dataGridView2.Columns[i].HeaderText = headers[i];
                dataGridView2.Rows[i].HeaderCell.Value = headers[i];
            }
            dataGridView1.RowHeadersWidth = 52;
            dataGridView2.RowHeadersWidth = 52;
            dataGridView1.ColumnHeadersHeight = 52;
            dataGridView2.ColumnHeadersHeight = 52;


            for (int i = 0; i < vertices; i++)
            {

                dataGridView2[i, i].Style.BackColor = dataGridView1[i, i].Style.BackColor = Color.Black;

                dataGridView2[i, i].ReadOnly = dataGridView1[i, i].ReadOnly = true;
            }


        }

        private void dataGridView1_CellValueChanged_1(object sender, DataGridViewCellEventArgs e)
        {

            try
            {
                if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.RowCount && e.ColumnIndex >= 0 && e.ColumnIndex < dataGridView1.ColumnCount)
                {
                    if (dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString() == "inf")
                    {
                        dataGridView1[e.ColumnIndex, e.RowIndex].Style.ForeColor = Color.Black;
                    }

                    else
                    {
                        double value = Convert.ToDouble(dataGridView1[e.ColumnIndex, e.RowIndex].Value);
                        if (value > 0)
                        {

                            Convert.ToDouble(dataGridView1[e.ColumnIndex, e.RowIndex].Value);
                            dataGridView1[e.ColumnIndex, e.RowIndex].Style.ForeColor = Color.Black;

                        }
                        else if (value < 0)
                        {
                            dataGridView1[e.ColumnIndex, e.RowIndex].Style.ForeColor = Color.Red;
                            MessageBox.Show("Исправьте красные значения: поддерживаются целые значения, нецелые(через запятую) и inf - бесконечность(нет путей)", "Ошибка ввода");
                        }
                    }
                }
            }
            catch
            {
                if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.RowCount && e.ColumnIndex >= 0 && e.ColumnIndex < dataGridView1.ColumnCount)
                {
                    dataGridView1[e.ColumnIndex, e.RowIndex].Style.ForeColor = Color.Red;
                    MessageBox.Show("Исправьте красные значения: поддерживаются целые значения, нецелые(через запятую) и inf - бесконечность(нет путей)", "Ошибка ввода");

                }
            }

        }

        private void открытьToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            openFileDialog1.DefaultExt = ".xml";
            openFileDialog1.Filter = "Графы в формате *.xml|*.xml|Графы в формате *.graph|*.graph|Все файлы|*.*";

            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            if (openFileDialog1.FilterIndex == 1)
            {
                XmlTextReader r = new XmlTextReader(openFileDialog1.FileName);
                r.ReadToFollowing("Граф");
                numericUpDown1.Value = XmlConvert.ToInt32(r.GetAttribute("Вершин"));
                for (int i = 0; i < vertices; i++)
                {
                    r.ReadToFollowing("Строка" + (i + 1));
                    for (int j = 0; j < vertices; j++)
                    {
                        string attributeValue = r.GetAttribute("Яч" + (j + 1));
                        if (attributeValue == "inf")
                        {
                            dataGridView1[j, i].Value = "inf";
                        }
                        else
                        {
                            dataGridView1[j, i].Value = XmlConvert.ToDouble(r.GetAttribute("Яч" + (j + 1)));
                        }

                    }
                }
                r.Close();
            }
            else
            {
                numericUpDown1.Value = 2;
                FileStream fs = new FileStream(openFileDialog1.FileName, FileMode.Open);
                BinaryReader br = new BinaryReader(fs);
                numericUpDown1.Value = br.ReadInt32();
                for (int i = 0; i < vertices; i++)
                {
                    for (int j = 0; j < vertices; j++)
                    {
                        dataGridView1[j, i].Value = br.ReadInt32();
                    }
                }
                br.Close();
                fs.Close();
            }
        }

        private void сохранитьToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            double[,] graph = new double[vertices, vertices];

            for (int i = 0; i < vertices; i++)
            {
                for (int j = 0; j < vertices; j++)
                {
                    if (Convert.ToString(dataGridView1[j, i].Value) == "inf")
                    {
                        graph[i, j] = inf;
                    }
                    else
                    {
                        graph[i, j] = Convert.ToDouble(dataGridView1[j, i].Value);
                    }

                }
            }



            saveFileDialog1.DefaultExt = ".xml";
            saveFileDialog1.Filter = "Графы в формате *.xml|*.xml|Графы в формате *.graph|*.graph|Все файлы|*.*";


            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            if (saveFileDialog1.FilterIndex == 1)
            {

                XmlTextWriter w = new XmlTextWriter(saveFileDialog1.FileName, Encoding.Unicode);
                w.Formatting = Formatting.Indented;
                w.WriteStartDocument();
                w.WriteStartElement("Граф");
                w.WriteAttributeString("Вершин", XmlConvert.ToString(vertices));
                w.WriteStartElement("Матрица_смежности");
                for (int i = 0; i < vertices; i++)
                {
                    w.WriteStartElement("Строка" + (i + 1));
                    for (int j = 0; j < vertices; j++)
                    {
                        if (graph[i, j] == inf)
                        {
                            w.WriteAttributeString("Яч" + (j + 1), "inf");
                        }
                        else
                        {
                            w.WriteAttributeString("Яч" + (j + 1), XmlConvert.ToString(graph[i, j]));
                        }
                    }
                    w.WriteEndElement();


                }
                w.WriteEndElement();
                w.WriteEndElement();
                w.WriteEndDocument();
                w.Close();
            }
            else
            {

                FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.Create);
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(vertices);
                foreach (double x in graph)
                    bw.Write(x);
                bw.Close();
                fs.Close();

            }
        }



        private void button1_Click(object sender, EventArgs e)
        {
            double[,] distance = new double[vertices, vertices];
            double[,] graph = new double[vertices, vertices];
            for (int i = 0; i < vertices; i++)
            {
                for (int j = 0; j < vertices; j++)
                {
                    if (Convert.ToString(dataGridView1[i, j].Value) == "inf")
                    {
                        graph[i, j] = inf;
                    }
                    else
                    {
                        graph[i, j] = Convert.ToDouble(dataGridView1[i, j].Value);
                    }
                }
            }


            for (int i = 0; i < vertices; i++)
            {
                for (int j = 0; j < vertices; j++)
                {
                    distance[i, j] = graph[i, j];
                }
            }

            for (int k = 0; k < vertices; k++)
            {
                for (int i = 0; i < vertices; i++)
                {
                    for (int j = 0; j < vertices; j++)
                    {
                        if (distance[i, k] + distance[k, j] < distance[i, j])
                            distance[i, j] = (distance[i, k]) + (distance[k, j]);
                    }
                }
            }



            for (int i = 0; i < vertices; i++)
            {
                for (int j = 0; j < vertices; j++)
                {
                    if (distance[i, j] == 9999)
                    {
                        dataGridView2.Rows[j].Cells[i].Value = "inf";
                    }
                    else
                    {
                        dataGridView2.Rows[j].Cells[i].Value = distance[i, j];
                    }


                }
            }
        }

        private void dataGridView1_ColumnStateChanged(object sender, DataGridViewColumnStateChangedEventArgs e)
        {
            e.Column.SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView1.Columns.Clear();
            dataGridView2.Columns.Clear();
        }
    }
}


