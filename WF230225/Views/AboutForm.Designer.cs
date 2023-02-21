namespace WF230225.Views
{
    partial class AboutForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.aboutButtonClose = new System.Windows.Forms.Button();
            this.tbAbout = new System.Windows.Forms.TextBox();
            this.lblAbout1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // aboutButtonClose
            // 
            this.aboutButtonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.aboutButtonClose.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.aboutButtonClose.Location = new System.Drawing.Point(456, 376);
            this.aboutButtonClose.Name = "aboutButtonClose";
            this.aboutButtonClose.Size = new System.Drawing.Size(312, 40);
            this.aboutButtonClose.TabIndex = 2;
            this.aboutButtonClose.Text = "Закрыть";
            this.aboutButtonClose.UseVisualStyleBackColor = true;
            // 
            // tbAbout
            // 
            this.tbAbout.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.tbAbout.Location = new System.Drawing.Point(448, 24);
            this.tbAbout.Multiline = true;
            this.tbAbout.Name = "tbAbout";
            this.tbAbout.ReadOnly = true;
            this.tbAbout.Size = new System.Drawing.Size(312, 128);
            this.tbAbout.TabIndex = 3;
            this.tbAbout.Text = "Задание по предмету WindowsForms на 25/02/2023.\r\n\r\nВыполнил студент группы СПД111" +
    " \r\nГрабовский Ярослав.";
            // 
            // lblAbout1
            // 
            this.lblAbout1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblAbout1.Location = new System.Drawing.Point(32, 24);
            this.lblAbout1.Name = "lblAbout1";
            this.lblAbout1.Size = new System.Drawing.Size(400, 392);
            this.lblAbout1.TabIndex = 4;
            this.lblAbout1.Text = resources.GetString("lblAbout1.Text");
            // 
            // AboutForm
            // 
            this.AcceptButton = this.aboutButtonClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.ControlBox = false;
            this.Controls.Add(this.lblAbout1);
            this.Controls.Add(this.tbAbout);
            this.Controls.Add(this.aboutButtonClose);
            this.Name = "AboutForm";
            this.Text = "О программе";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Button aboutButtonClose;
        private TextBox tbAbout;
        private Label lblAbout1;
    }
}