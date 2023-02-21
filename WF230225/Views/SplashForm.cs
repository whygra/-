using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WF230225.Views
{
    public partial class SplashForm : Form
    {
        private Point p;   // для перемещения формы (точка на метке, где было событие MouseDown)

        public SplashForm()
        {
            InitializeComponent();

            // Назначить обработчик события MouseDown для метки
            // Обработчик - лямбда-выражение запоминает место нажатия 
            // левой кнопкой мыши.
            lblInfo.MouseDown += (sender, e) => p = e.Location;
        } // SplashForm


        // Обработчик перемещения мыши для меткт lblInfo
        private void lblInfo_MouseMove(object sender, MouseEventArgs e)
        {
            // Перемещать окно (т.е. менять его координаты будем только при нажатой
            // левой кнопке мыши
            if (MouseButtons == MouseButtons.Left)
                Location += new Size(e.X - p.X, e.Y - p.Y);
        } // lblInfo_MouseMove


        // Для досрочного завершения приложения по клавише Esc
        // будем прятать форму - а этот факт будет обрабатываться в главной форме 
        private void SplashForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) Hide();
        } // SplashForm_KeyDown

        // заполнение отображается некорректно, с задержкой
        public void SetPB(int value)
        {
            prbSplash.Show();
            prbSplash.Value = value;
            prbSplash.Update();
        }
    }
}
