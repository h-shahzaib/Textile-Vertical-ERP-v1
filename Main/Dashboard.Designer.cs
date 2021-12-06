namespace Main
{
    partial class Dashboard
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
            this.main_program_entry_btn = new System.Windows.Forms.Button();
            this.new_brand = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.stock_btn = new System.Windows.Forms.Button();
            this.brands_data = new System.Windows.Forms.ListBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.LastDiaryNum_Lbl = new System.Windows.Forms.Label();
            this.Refresh = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // main_program_entry_btn
            // 
            this.main_program_entry_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.main_program_entry_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.main_program_entry_btn.Font = new System.Drawing.Font("Malgun Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.main_program_entry_btn.Location = new System.Drawing.Point(1736, 13);
            this.main_program_entry_btn.Margin = new System.Windows.Forms.Padding(4);
            this.main_program_entry_btn.Name = "main_program_entry_btn";
            this.main_program_entry_btn.Size = new System.Drawing.Size(342, 74);
            this.main_program_entry_btn.TabIndex = 0;
            this.main_program_entry_btn.Text = "Add New Program";
            this.main_program_entry_btn.UseVisualStyleBackColor = true;
            this.main_program_entry_btn.Click += new System.EventHandler(this.main_program_entry_btn_Click);
            // 
            // new_brand
            // 
            this.new_brand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.new_brand.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.new_brand.Font = new System.Drawing.Font("Malgun Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.new_brand.Location = new System.Drawing.Point(1386, 13);
            this.new_brand.Margin = new System.Windows.Forms.Padding(4);
            this.new_brand.Name = "new_brand";
            this.new_brand.Size = new System.Drawing.Size(342, 74);
            this.new_brand.TabIndex = 1;
            this.new_brand.Text = "Add New Brand";
            this.new_brand.UseVisualStyleBackColor = true;
            this.new_brand.Click += new System.EventHandler(this.new_brand_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.stock_btn);
            this.panel1.Controls.Add(this.new_brand);
            this.panel1.Controls.Add(this.main_program_entry_btn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(2091, 95);
            this.panel1.TabIndex = 3;
            // 
            // stock_btn
            // 
            this.stock_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stock_btn.Font = new System.Drawing.Font("Malgun Gothic", 7F);
            this.stock_btn.Location = new System.Drawing.Point(13, 13);
            this.stock_btn.Margin = new System.Windows.Forms.Padding(4);
            this.stock_btn.Name = "stock_btn";
            this.stock_btn.Size = new System.Drawing.Size(128, 74);
            this.stock_btn.TabIndex = 2;
            this.stock_btn.Text = "Clear Console";
            this.stock_btn.UseVisualStyleBackColor = true;
            this.stock_btn.Click += new System.EventHandler(this.stock_btn_Click);
            // 
            // brands_data
            // 
            this.brands_data.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.brands_data.Font = new System.Drawing.Font("Malgun Gothic", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.brands_data.FormattingEnabled = true;
            this.brands_data.ItemHeight = 37;
            this.brands_data.Location = new System.Drawing.Point(397, 85);
            this.brands_data.Name = "brands_data";
            this.brands_data.Size = new System.Drawing.Size(244, 668);
            this.brands_data.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.LastDiaryNum_Lbl);
            this.panel2.Controls.Add(this.Refresh);
            this.panel2.Controls.Add(this.brands_data);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 95);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(2091, 1186);
            this.panel2.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label4.Location = new System.Drawing.Point(973, 502);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(877, 35);
            this.label4.TabIndex = 6;
            this.label4.Text = "Diarynumber must be verified by split count";
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label3.Location = new System.Drawing.Point(973, 573);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(877, 36);
            this.label3.TabIndex = 5;
            this.label3.Text = "Check for Designs if they are Distinct or not, AT STARTUP";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label1.Location = new System.Drawing.Point(973, 412);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(877, 90);
            this.label1.TabIndex = 3;
            this.label1.Text = "DiaryNumber verification functionality. On Every Startup check to see if all diar" +
    "y numbers are unique, in Stock, Billing and Complete file";
            // 
            // LastDiaryNum_Lbl
            // 
            this.LastDiaryNum_Lbl.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.LastDiaryNum_Lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LastDiaryNum_Lbl.Location = new System.Drawing.Point(398, 721);
            this.LastDiaryNum_Lbl.Name = "LastDiaryNum_Lbl";
            this.LastDiaryNum_Lbl.Padding = new System.Windows.Forms.Padding(0, 0, 78, 0);
            this.LastDiaryNum_Lbl.Size = new System.Drawing.Size(242, 31);
            this.LastDiaryNum_Lbl.TabIndex = 2;
            this.LastDiaryNum_Lbl.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // Refresh
            // 
            this.Refresh.Location = new System.Drawing.Point(397, 758);
            this.Refresh.Name = "Refresh";
            this.Refresh.Size = new System.Drawing.Size(244, 46);
            this.Refresh.TabIndex = 1;
            this.Refresh.Text = "Refresh";
            this.Refresh.UseVisualStyleBackColor = true;
            this.Refresh.Click += new System.EventHandler(this.Refresh_Click);
            // 
            // Dashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(2091, 1281);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Dashboard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Dashboard";
            this.Shown += new System.EventHandler(this.Main_Shown);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button main_program_entry_btn;
        private System.Windows.Forms.Button new_brand;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListBox brands_data;
        private System.Windows.Forms.Button stock_btn;
        private System.Windows.Forms.Panel panel2;
        new private System.Windows.Forms.Button Refresh;
        private System.Windows.Forms.Label LastDiaryNum_Lbl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}

