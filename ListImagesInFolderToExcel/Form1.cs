using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using System.Threading;

namespace ListImagesInFolderToExcel
{
    enum Direction { Vertical = 0, Horizontal};
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            txtPath.Text = folderBrowserDialog1.SelectedPath;
        }

        private void DoWork(string selectedPath,int direction, int imgWidth, int imgHeight, int spacing)
        {
            string[] filenames = Directory.GetFiles(selectedPath, "*", SearchOption.AllDirectories);
            //DirectoryInfo dir = new DirectoryInfo(selectedPath);
            //FileInfo[] file = dir.GetFiles();
            ArrayList list = new ArrayList();
            foreach (string file2 in filenames)
            {
                FileInfo info = new FileInfo(file2);
                if (info.Extension == ".jpg" || info.Extension == ".jpeg" || info.Extension == ".png")
                {
                    list.Add(info);
                }
            }
            ExcelPackage ExcelPkg = new ExcelPackage();
            ExcelWorksheet wsSheet1 = ExcelPkg.Workbook.Worksheets.Add("Sheet1");
            int rowIndex = 1;
            int colIndex = 1;
            Random random = new Random();
            foreach (FileInfo info in list)
            {

                using (ExcelRange Rng = wsSheet1.Cells[rowIndex, colIndex, rowIndex, colIndex])
                {
                    Rng.Value = info.Name;
                    Rng.Style.Font.Size = 14;
                    Rng.Style.Font.Bold = true;
                    Rng.Style.Font.Italic = true;
                    Rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }

                Image img = Image.FromFile(info.FullName);
                ExcelPicture pic = wsSheet1.Drawings.AddPicture(info.Name + random.NextDouble().ToString(), img);
                if (direction == (int)Direction.Horizontal)
                {
                    pic.SetPosition(rowIndex, 0, colIndex - 1, 0);
                    wsSheet1.Column(colIndex).AutoFit();
                }
                else
                {
                    pic.SetPosition(rowIndex - 1, 0, colIndex, 0);
                    wsSheet1.Row(rowIndex).Height = imgHeight / 1.3;
                    wsSheet1.Column(1).AutoFit();
                }
                pic.SetSize(imgWidth, imgHeight);
                rowIndex = calculateRowIndex(direction, rowIndex, spacing);
                colIndex = calculateColIndex(direction, colIndex, spacing);
            }
            if (list.Count > 0)
            {
                wsSheet1.Protection.IsProtected = false;
                wsSheet1.Protection.AllowSelectLockedCells = false;
                string filename = String.Format(@"D:\ImagesReport{0}.xlsx", random.Next(100).ToString());
                ExcelPkg.SaveAs(new FileInfo(filename));
                MessageBox.Show("Done, output file: " + filename, "Finish", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnDo.Text = "Process It !";
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            String selectedPath = txtPath.Text;
            int direction = cbxDirection.SelectedIndex;
            int imgWidth = Int32.Parse(txtImgW.Text);
            int imgHeight = Int32.Parse(txtImgH.Text);
            int spacing = Convert.ToInt32(numericUpDown1.Value);
            if (selectedPath.Length == 0) return;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            btnDo.Text = "Working, please wait....";
            Thread thread1 = new Thread(()=>DoWork(selectedPath,direction, imgWidth, imgHeight, spacing));
            thread1.Start();

        }

        private int calculateColIndex(int direction, int colIndex, int spacing)
        {
            switch (direction)
            {
                case (int)Direction.Vertical:
                    return colIndex;
                case (int)Direction.Horizontal:
                    return colIndex + spacing;
            }
            return colIndex;
        }

        private int calculateRowIndex(int direction, int rowIndex, int spacing)
        {
            switch (direction)
            {
                case (int)Direction.Vertical:
                    return rowIndex + spacing;
                case (int)Direction.Horizontal:
                    return rowIndex;
            }
            return rowIndex;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txtImgW.Text = txtImgH.Text = "100";
        }
    }
}
