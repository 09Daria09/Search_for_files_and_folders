using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Button = System.Windows.Forms.Button;
using ComboBox = System.Windows.Forms.ComboBox;
using ListView = System.Windows.Forms.ListView;
using TextBox = System.Windows.Forms.TextBox;

namespace Search_for_files_and_folders
{
    public partial class Form1 : Form
    {
        Thread thread = null;
        TextBox textBox1;
        TextBox textBox2;
        ComboBox comboBox1;
        Button button1;
        Button button2;
        Label label1;
        Label label2;
        Label label3;
        ListView listView1;
        public Form1()
        {
            InitializeComponent();

            thread = new Thread(InitializeOwn);
            thread.Start();
            
        }

        private delegate void InWork(int a);

        private void InitializeOwn()
        {
            if (!Controls.Contains(textBox1))
            {
                textBox1 = new TextBox();
                textBox2 = new TextBox();
                comboBox1 = new ComboBox();
                button1 = new Button();
                button2 = new Button();
                label1 = new Label();
                label2 = new Label();
                label3 = new Label();
                listView1 = new ListView();
            }

            comboBox1 = new ComboBox();
            comboBox1.Items.AddRange(System.IO.Directory.GetLogicalDrives());

            try
            {
                Action Act1 = delegate
                {
                    textBox1.Location = new Point(13, 20);
                    textBox1.Size = new Size(170, 23);
                    Controls.Add(textBox1);
                };
                Action Act2 = delegate
                {
                    textBox2.Location = new Point(190, 20);
                    textBox2.Size = new Size(260, 23);
                    Controls.Add(textBox2);
                };
                Action Act3 = delegate
                {
                    comboBox1.Location = new Point(460, 20);
                    comboBox1.Size = new Size(80, 23);
                    Controls.Add(comboBox1);
                };
                Action Act4 = delegate
                {
                    button1.Location = new Point(550, 20);
                    button1.Size = new Size(80, 23);
                    button1.Text = "Найти";
                    Controls.Add(button1);
                };
                Action Act5 = delegate
                {
                    button2.Location = new Point(630, 20);
                    button2.Size = new Size(80, 23);
                    button2.Text = "Остановить";
                    Controls.Add(button2);
                };
                Action Act6 = delegate
                {
                    label1.Location = new Point(25, 5);
                    label1.Size = new Size(40, 23);
                    label1.Text = "Маска";
                    Controls.Add(label1);
                };
                Action Act7 = delegate
                {
                    label2.Location = new Point(230, 3);
                    label2.Size = new Size(150, 15);
                    label2.Text = "Слово или фраза в файле";
                    Controls.Add(label2);
                };
                Action Act8 = delegate
                {
                    label3.Location = new Point(483, 3);
                    label3.Size = new Size(150, 15);
                    label3.Text = "Диски";
                    Controls.Add(label3);
                };
                Action Act9 = delegate
                {
                    listView1.Location = new Point(10, 50);
                    listView1.Size = new Size(800, 800);
                    listView1.View = View.Details;
                    listView1.Columns.Add("Имя файла", 200);
                    listView1.Columns.Add("Размер", 100);
                    listView1.Columns.Add("Дата изменения", 150);
                    Controls.Add(listView1);
                };
                this.Invoke(Act1);
                this.Invoke(Act2);
                this.Invoke(Act3);
                this.Invoke(Act4);
                this.Invoke(Act5);
                this.Invoke(Act6);
                this.Invoke(Act7);
                this.Invoke(Act8);
                this.Invoke(Act9);
            }
            catch (Exception ex)
            {

            }
        }


        private CancellationTokenSource cancellationTokenSource;
        private void button1_Click(object sender, EventArgs e)
        {
            string selectedDrive = comboBox1.SelectedItem.ToString();
            string searchPattern = textBox1.Text;
            string rootFolderPath = selectedDrive + "\\";

            listView1.Items.Clear();

            cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            Thread searchThread = new Thread(() =>
            {
                SearchFiles(rootFolderPath, searchPattern, cancellationToken);
            });

            searchThread.Start();
        }
        private void SearchFiles(string rootFolderPath, string searchPattern, CancellationToken cancellationToken)
        {
            try
            {
                DirectoryInfo rootFolder = new DirectoryInfo(rootFolderPath);

                foreach (DirectoryInfo subFolder in rootFolder.GetDirectories())
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }
                    try
                    {
                        SearchFiles(subFolder.FullName, searchPattern, cancellationToken);
                    }
                    catch (UnauthorizedAccessException)
                    {
                    }
                }

                foreach (FileInfo file in rootFolder.GetFiles(searchPattern))
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }
                    ListViewItem item = new ListViewItem(file.Name);
                    item.SubItems.Add(file.Length.ToString());
                    item.SubItems.Add(file.LastWriteTime.ToString());
                    listView1.Items.Add(item);
                }
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            cancellationTokenSource.Cancel();
        }

    }
}
