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

        // форма для вывода при старте приложения, форма "О программе"
        private SplashForm _splashForm = new SplashForm();

        public MainForm()
        {
            _controller = new();
            InitializeComponent();

            // привязать _splashForm к этому окну, отобразить 
            AddOwnedForm(_splashForm);
            _splashForm.Show();

            // Для отображения содержимого окна-заставки
            Application.DoEvents();
        }

        
        private void MainForm_Load(object sender, EventArgs e)
        {
            Thread.Sleep(700);

            _splashForm.SetPB(33);

            _controller.Init(true);
            this.Text = Untitled;

            _splashForm.SetPB(66);

            ShowTourOperatorInfo();


            cmbxSortOrder.Items.AddRange(new[] { "по возрастанию", "по убыванию" });
            cmbxSortField.Items.AddRange(new[] { "коду", "начальному пункту", "протяженности" });
            UpdateSelectionComboBoxes();


            // варианты выбора пункта в столбцах DataGridView
            ((DataGridViewComboBoxColumn)dataGridMain.Columns["startDataGridViewTextBoxColumn"])
                .Items.AddRange(RouteFactory.Points);
            ((DataGridViewComboBoxColumn)dataGridMain.Columns["finishDataGridViewTextBoxColumn"])
                .Items.AddRange(RouteFactory.Points);

            _splashForm.SetPB(100);


            Thread.Sleep(700);

            // Убираем заставку до появления главной формы
            _splashForm.Hide();
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
                case 1: // Start
                    if (string.IsNullOrWhiteSpace(e.FormattedValue as string)) 
                        err = "Укажите начальную точку маршрута";
                    break;

                case 2: // Finish
                    if (string.IsNullOrWhiteSpace(e.FormattedValue as string))
                        err = "Укажите конечную точку маршрута";
                    break;

                case 3: // Code
                    if (string.IsNullOrWhiteSpace(e.FormattedValue as string)) 
                        err = "Укажите код маршрута";
                    break;

                case 4: // Length
                    if ((int)(e.FormattedValue) <= 0)
                        err = "Недопустимая протяженность маршрута";
                    break;
            }

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
            // диапазон протяженности
            cmbxLengthFrom.Items.Clear();
            cmbxLengthFrom.Items.AddRange(_controller.GetRangeSteps().ToArray()[..^1]);

            cmbxLengthTo.Items.Clear();
            cmbxLengthTo.Items.AddRange(_controller.GetRangeSteps().ToArray()[1..]);

            // пункты
            cmbxPoint.Items.Clear();
            cmbxPoint.Items.AddRange(_controller.GetPoints());
        }

        // выборка по диапазону протяженности
        private void SelectByLengthRange(object sender, EventArgs e)
        {
            int from, to;

            if (!int.TryParse(cmbxLengthFrom.Text, out from))
            {
                cmbxLengthFrom.Focus();
                return;
            }

            if (!int.TryParse(cmbxLengthTo.Text, out to))
            {
                cmbxLengthTo.Focus();
                return;
            }

            dataGridSelected.DataSource = _controller.SelectByLengthRange(from, to);

            tabs.SelectTab(2);
        }


        // выборка по пункту
        private void SelectByPoint(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cmbxPoint.Text))
            {
                cmbxPoint.Focus();
                return;
            }

            dataGridSelected.DataSource = _controller.SelectByPoint(cmbxPoint.Text);

            tabs.SelectTab(2);
        }

        #endregion

        #region сортировка

        private void SelectSortField(object sender, EventArgs e)
        {
            if (cmbxSortField.SelectedItem == null)
                return;

            _controller.SortComp = cmbxSortField.Text switch
            {
                "коду" => TourOperatorController.CodeComparer,
                "начальному пункту" => TourOperatorController.StartComparer,
                "протяженности" => TourOperatorController.LengthComparer,
                _ => null!
            };

            if (cmbxSortOrder.SelectedIndex == -1)
                cmbxSortOrder.SelectedIndex = 0;

            Sort();
        }
        private void SelectSortOrder(object sender, EventArgs e)
        {
            if (cmbxSortField.SelectedIndex == -1)
                cmbxSortField.Focus();
            else
                Sort();
        }

        private void Sort()
        {
            var sorted = _controller.GetSorted(cmbxSortOrder.SelectedIndex == 0);

            dataGridSorted.DataSource = null!;
            dataGridSorted.DataSource = sorted;

            tabs.SelectTab(1);
        }

        #endregion

        // вывод данных турфирмы
        private void ShowTourOperatorInfo()
        {
            txbxName.Text = _controller.Name;
            txbxAddress.Text = _controller.Address;
            bsrcMain.DataSource = null!;
            bsrcMain.DataSource = _controller.Items;
        }

        // изменение названия и адреса
        private void SetName(object sender, EventArgs e)
        {
            _controller.SetName(txbxName.Text);
            _controller.Serialize();
        }

        private void SetAddress(object sender, EventArgs e)
        {
            _controller.SetAddress(txbxAddress.Text);
            _controller.Serialize();
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
