namespace Main.Resources.Forms.Miscellaneous
{
    partial class Party_Entry_Form
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
            this.close = new System.Windows.Forms.Button();
            this.title = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.grouper = new System.Windows.Forms.GroupBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.code_txtBx = new System.Windows.Forms.TextBox();
            this.code_lbl = new System.Windows.Forms.Label();
            this.name_txtBx = new System.Windows.Forms.TextBox();
            this.name_lbl = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.grouper.SuspendLayout();
            this.SuspendLayout();
            // 
            // close
            // 
            this.close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.close.BackColor = System.Drawing.Color.Red;
            this.close.FlatAppearance.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.close.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.close.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.close.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.close.Location = new System.Drawing.Point(484, 6);
            this.close.Margin = new System.Windows.Forms.Padding(6);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(52, 44);
            this.close.TabIndex = 0;
            this.close.Text = "X";
            this.close.UseVisualStyleBackColor = false;
            this.close.Click += new System.EventHandler(this.Close_Click);
            // 
            // title
            // 
            this.title.AutoSize = true;
            this.title.Font = new System.Drawing.Font("Lucida Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.title.Location = new System.Drawing.Point(16, 12);
            this.title.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.title.Name = "title";
            this.title.Size = new System.Drawing.Size(271, 31);
            this.title.TabIndex = 1;
            this.title.Text = "Enter Brand Details:";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.grouper);
            this.panel1.Controls.Add(this.close);
            this.panel1.Controls.Add(this.title);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(6);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(544, 217);
            this.panel1.TabIndex = 2;
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            // 
            // grouper
            // 
            this.grouper.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grouper.Controls.Add(this.progressBar);
            this.grouper.Controls.Add(this.code_txtBx);
            this.grouper.Controls.Add(this.code_lbl);
            this.grouper.Controls.Add(this.name_txtBx);
            this.grouper.Controls.Add(this.name_lbl);
            this.grouper.Location = new System.Drawing.Point(22, 62);
            this.grouper.Margin = new System.Windows.Forms.Padding(6);
            this.grouper.Name = "grouper";
            this.grouper.Padding = new System.Windows.Forms.Padding(6);
            this.grouper.Size = new System.Drawing.Size(498, 133);
            this.grouper.TabIndex = 4;
            this.grouper.TabStop = false;
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(0, 127);
            this.progressBar.Margin = new System.Windows.Forms.Padding(6);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(498, 6);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 0;
            this.progressBar.Visible = false;
            // 
            // code_txtBx
            // 
            this.code_txtBx.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.code_txtBx.Font = new System.Drawing.Font("Lucida Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.code_txtBx.Location = new System.Drawing.Point(116, 75);
            this.code_txtBx.Margin = new System.Windows.Forms.Padding(6);
            this.code_txtBx.Name = "code_txtBx";
            this.code_txtBx.Size = new System.Drawing.Size(366, 33);
            this.code_txtBx.TabIndex = 4;
            // 
            // code_lbl
            // 
            this.code_lbl.AutoSize = true;
            this.code_lbl.Font = new System.Drawing.Font("Lucida Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.code_lbl.Location = new System.Drawing.Point(12, 79);
            this.code_lbl.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.code_lbl.Name = "code_lbl";
            this.code_lbl.Size = new System.Drawing.Size(86, 31);
            this.code_lbl.TabIndex = 5;
            this.code_lbl.Text = "Code:";
            // 
            // name_txtBx
            // 
            this.name_txtBx.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.name_txtBx.Font = new System.Drawing.Font("Lucida Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.name_txtBx.Location = new System.Drawing.Point(116, 25);
            this.name_txtBx.Margin = new System.Windows.Forms.Padding(6);
            this.name_txtBx.Name = "name_txtBx";
            this.name_txtBx.Size = new System.Drawing.Size(366, 33);
            this.name_txtBx.TabIndex = 2;
            // 
            // name_lbl
            // 
            this.name_lbl.AutoSize = true;
            this.name_lbl.Font = new System.Drawing.Font("Lucida Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.name_lbl.Location = new System.Drawing.Point(12, 29);
            this.name_lbl.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.name_lbl.Name = "name_lbl";
            this.name_lbl.Size = new System.Drawing.Size(96, 31);
            this.name_lbl.TabIndex = 3;
            this.name_lbl.Text = "Name:";
            // 
            // Party_Entry_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(544, 217);
            this.Controls.Add(this.panel1);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "Party_Entry_Form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Party Entry Form";
            this.Shown += new System.EventHandler(this.Party_Entry_Form_Shown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.grouper.ResumeLayout(false);
            this.grouper.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button close;
        private System.Windows.Forms.Label title;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox grouper;
        private System.Windows.Forms.TextBox code_txtBx;
        private System.Windows.Forms.Label code_lbl;
        private System.Windows.Forms.TextBox name_txtBx;
        private System.Windows.Forms.Label name_lbl;
        private System.Windows.Forms.ProgressBar progressBar;
    }
}