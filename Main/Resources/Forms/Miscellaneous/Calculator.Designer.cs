namespace Main.Resources.Forms.Miscellaneous
{
    partial class Calculator
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.main_txtBx = new System.Windows.Forms.TextBox();
            this.history_txtBx = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.main_txtBx);
            this.panel1.Controls.Add(this.history_txtBx);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(15);
            this.panel1.Size = new System.Drawing.Size(761, 131);
            this.panel1.TabIndex = 0;
            // 
            // main_txtBx
            // 
            this.main_txtBx.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.main_txtBx.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.main_txtBx.Font = new System.Drawing.Font("Bahnschrift", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.main_txtBx.Location = new System.Drawing.Point(15, 56);
            this.main_txtBx.Name = "main_txtBx";
            this.main_txtBx.Size = new System.Drawing.Size(729, 58);
            this.main_txtBx.TabIndex = 1;
            this.main_txtBx.Text = "0";
            this.main_txtBx.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // history_txtBx
            // 
            this.history_txtBx.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.history_txtBx.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.history_txtBx.Dock = System.Windows.Forms.DockStyle.Top;
            this.history_txtBx.Enabled = false;
            this.history_txtBx.Font = new System.Drawing.Font("Consolas", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.history_txtBx.ForeColor = System.Drawing.Color.DarkGray;
            this.history_txtBx.Location = new System.Drawing.Point(15, 15);
            this.history_txtBx.Name = "history_txtBx";
            this.history_txtBx.ReadOnly = true;
            this.history_txtBx.Size = new System.Drawing.Size(729, 32);
            this.history_txtBx.TabIndex = 0;
            this.history_txtBx.TabStop = false;
            this.history_txtBx.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // Calculator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(761, 131);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Calculator";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Calculator";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.Calculator_Shown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox main_txtBx;
        private System.Windows.Forms.TextBox history_txtBx;
    }
}