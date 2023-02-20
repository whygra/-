using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using WF230225.Controllers;
using WF230225.Infrastructure;
using WF230225.Models;
using WF230225.Views;
using static System.ComponentModel.Design.ObjectSelectorEditor;

namespace WF230225
{
    public partial class MainForm : Form
    {
        static readonly string Untitled = "Безымянный";
        TourOperatorController _controller;

        public MainForm()
        {
            _controller = new();
            InitializeComponent();


        }

        
        private void MainForm_Load(object sender, EventArgs e)
        {
            _controller.Init(true);

            this.Text = Untitled;

            ShowTourOperatorInfo();

            cmbxSortOrder.Items.AddRange(new[] { "по возрастанию", "по убыванию" });

            UpdateSelectionComboBoxes();

        }


        #region файл

        // новая запись
        private void New(object sender, EventArgs e)
        {
            // сброс контроллера
            _controller.Init();

            this.Text = Untitled;

            ShowTourOperatorInfo();

            // обновить варианты выборки
            UpdateSelectionComboBoxes();
        }

        // сохранение
        private void Save(object sender, EventArgs e)
        {
            _controller.Serialize();
        }

        // сохранение с выбором пути
        private void SaveAs(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Сохранение";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                _controller.FilePath = saveFileDialog1.FileName;
                this.Text = _controller.FilePath;
                _controller.Serialize();
                toolStripStatusLabel1.Text = "Сохранено";
            }
            toolStripStatusLabel1.Text = "Готово";
        }

        // открыть файл
        private void Open(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                _controller.FilePath = openFileDialog1.FileName;
                this.Text = _controller.FilePath;

                _controller.Deserialize();
                ShowTourOperatorInfo();

                UpdateSelectionComboBoxes();
            }
        }

        // открытие перетаскиванием
        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.StringFormat) // строковые данные 
                || e.Data.GetDataPresent(DataFormats.FileDrop) ?  // файл данных - имя файла
                DragDropEffects.Copy :     // разрешена только операция Copy
                DragDropEffects.None;

        }

        private void dataGridMain_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Прием текста
                string[] paths = e.Data.GetData(DataFormats.FileDrop) as string[];
                _controller.FilePath = paths[0];

                _controller.Deserialize();
                ShowTourOperatorInfo();

                UpdateSelectionComboBoxes();
                this.Text = _controller.FilePath;

            }
        }
        #endregion

        #region работа с коллекцией
        // заполнение
        private void Fill(object sender, EventArgs e)
        {
            _controller.Fill(15);
            _controller.Serialize();
            ShowTourOperatorInfo();
        }

        // удаление
        // отключаем стандартное поведение 
        private void dataGridMain_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            e.Cancel = true;
        }

        // удаление по клавише
        private void dataGridMain_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                // удаление
                case Keys.Delete:
                    RemoveSelected(sender, e);
                    break;
                default:
                    return;
            }
        }

        // собственно удаление
        private void RemoveSelected(object sender, EventArgs e)
        {
            // не трогаем новую строку
            dataGridMain.Rows[dataGridMain.NewRowIndex].Selected = false;

            ConfirmForm confirmForm = 
                new($"Вы действительно хотите удалить элементы ({dataGridMain.SelectedRows.Count} шт.)");
            if (confirmForm.ShowDialog() != DialogResult.Yes)
                return;

            // удаление всех выделенных
            foreach (DataGridViewRow row in dataGridMain.SelectedRows)
                dataGridMain.Rows.Remove(row);

            // сохранение в файл
            _controller.Serialize();
            UpdateSelectionComboBoxes();
        }

        private void dataGridMain_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            _controller.SetLastId();
            UpdateSelectionComboBoxes();
        }

        // Проверка правильности данных на уровне ячейки таблицы
        private void dgvItems_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (sender is not DataGridView)
                return;
            var dgvItems = (DataGridView)sender;

            // Не проверяем последнюю строку таблицы - т.е. новую строку
            if (dgvItems.Rows[e.RowIndex].IsNewRow) return;

            string err = "";    // сообщение об ошибке или пустая сторока, если ошибки нет
            string str = e.FormattedValue.ToString();  // текст из проверяемой ячейки

            // Проверка данных в зависимости от номера столбца
            switch (e.ColumnIndex)
            {

            } // switch

            // вывод сообщения об ошибке
            dgvItems.Rows[e.RowIndex].ErrorText = err;

            e.Cancel = err != "";
        }
        
        // проверка данных на уровне строки
        private void dgvItems_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {

            if (sender is not DataGridView)
                return;
            var dgvItems = (DataGridView)sender;

            // Не обрабатывать новую строку (нижнюю дополнительную строку)
            if (dgvItems.Rows[e.RowIndex].IsNewRow) return;

            string err = "";
            
            foreach(DataGridViewCell cell in dgvItems.Rows[e.RowIndex].Cells)
            {
                if (cell.ColumnIndex == 0) // не трогаем Id
                    continue;

                if ((cell.Value as string) == "")
                {
                    err = $"Заполните поле \"{cell.OwningColumn.HeaderText}\"";
                    break;
                }
            }

            dgvItems.Rows[e.RowIndex].ErrorText = err;

             e.Cancel = err != "";
        }

        private void dgvItems_RowValidated(object sender, DataGridViewCellEventArgs e)
        {
            UpdateSelectionComboBoxes();
            _controller.Serialize();
        }

        #endregion


        #region выборка

        // обновление комбобоксов выборки
        private void UpdateSelectionComboBoxes()
        {
            
        }

        // выборка
        private void SelectBy(object sender, EventArgs e)
        {
            if (sender is not ToolStripComboBox)
                return;

            var cmbx = sender as ToolStripComboBox;

            dataGridSelected.DataSource = null!;
            dataGridSelected.DataSource = cmbx.Tag switch
            {
                _ => new()
            };


            tabs.SelectTab(2);
        }
        #endregion

        #region сортировка
        private void SortBy(object sender, EventArgs e)
        {

            dataGridSorted.DataSource = null!;
            dataGridSorted.DataSource = cmbxSortField switch
            {
                _ => new()
            };
            tabs.SelectTab(1);

        }

        #endregion

        // вывод данных турфирмы
        private void ShowTourOperatorInfo()
        {
            // txbxName.Text = _controller.Name;
            // txbxAddress.Text = _controller.Address;
            bdSrcItems.DataSource = null!;
            bdSrcItems.DataSource = _controller.Items;
        }


        // выход из приложения
        private void Exit(object sender, EventArgs e)
        {
            Close();
            return;
        }

        // о программе
        private void ShowAboutForm(object sender, EventArgs e)
        {
            AboutForm splashForm = new AboutForm();
            splashForm.ShowDialog();
        }

        // выбор шрифта
        private void fontBtn_Click(object sender, EventArgs e)
        {

            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                DataGridView dgv = tabs.SelectedIndex switch
                {
                    0 => dataGridMain,
                    1 => dataGridSorted,
                    2 => dataGridSelected,
                    _ => new()
                };

                dgv.Font = fontDialog1.Font;
            }

        }

        // выбор цвета фона
        private void backColorBtn_Click(object sender, EventArgs e)
        {

            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                DataGridView dgv = tabs.SelectedIndex switch
                {
                    0 => dataGridMain,
                    1 => dataGridSorted,
                    2 => dataGridSelected,
                    _ => new()
                };

                dgv.DefaultCellStyle.BackColor = colorDialog1.Color;
            }
        }

        // сворачивание в трей
        private void toTray(object sender, EventArgs e)
        {
            this.Hide();
            notifyIcon1.Visible = true;
        }

        private void Maximize(object sender, EventArgs e)
        {
            this.Show();
            notifyIcon1.Visible = false;
        }

        // выбор элемента по правому клику мыши
        private void dataGridMain_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            var dgv = sender as DataGridView;
            foreach (DataGridViewRow row in dgv.SelectedRows) row.Selected = false;
            dgv.Rows[e.RowIndex].Selected = true;
        }
    }
}
