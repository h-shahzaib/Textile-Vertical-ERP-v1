using System.Windows.Forms;

namespace ShahzaibEMB.Resources.Forms.ProgramEntry
{
    partial class RepManagementForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RepManagementForm));
            this.BorderPanel = new System.Windows.Forms.Panel();
            this.CloseBtn = new System.Windows.Forms.Button();
            this.DoneBtn = new System.Windows.Forms.Button();
            this.FlowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.BorderPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // BorderPanel
            // 
            this.BorderPanel.AutoSize = true;
            this.BorderPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BorderPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.BorderPanel.Controls.Add(this.CloseBtn);
            this.BorderPanel.Controls.Add(this.DoneBtn);
            this.BorderPanel.Controls.Add(this.FlowPanel);
            this.BorderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BorderPanel.Location = new System.Drawing.Point(0, 0);
            this.BorderPanel.Margin = new System.Windows.Forms.Padding(2);
            this.BorderPanel.Name = "BorderPanel";
            this.BorderPanel.Size = new System.Drawing.Size(1003, 541);
            this.BorderPanel.TabIndex = 0;
            // 
            // CloseBtn
            // 
            this.CloseBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CloseBtn.BackColor = System.Drawing.SystemColors.Control;
            this.CloseBtn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("CloseBtn.BackgroundImage")));
            this.CloseBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.CloseBtn.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.CloseBtn.FlatAppearance.BorderSize = 6;
            this.CloseBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CloseBtn.ForeColor = System.Drawing.Color.Black;
            this.CloseBtn.Location = new System.Drawing.Point(969, 4);
            this.CloseBtn.Margin = new System.Windows.Forms.Padding(2);
            this.CloseBtn.Name = "CloseBtn";
            this.CloseBtn.Size = new System.Drawing.Size(25, 26);
            this.CloseBtn.TabIndex = 14;
            this.CloseBtn.UseVisualStyleBackColor = false;
            // 
            // DoneBtn
            // 
            this.DoneBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DoneBtn.BackColor = System.Drawing.SystemColors.Control;
            this.DoneBtn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("DoneBtn.BackgroundImage")));
            this.DoneBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.DoneBtn.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.DoneBtn.FlatAppearance.BorderSize = 5;
            this.DoneBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DoneBtn.ForeColor = System.Drawing.Color.Black;
            this.DoneBtn.Location = new System.Drawing.Point(941, 4);
            this.DoneBtn.Margin = new System.Windows.Forms.Padding(2);
            this.DoneBtn.Name = "DoneBtn";
            this.DoneBtn.Size = new System.Drawing.Size(25, 26);
            this.DoneBtn.TabIndex = 13;
            this.DoneBtn.UseVisualStyleBackColor = false;
            this.DoneBtn.Click += new System.EventHandler(this.DoneBtn_Click);
            // 
            // FlowPanel
            // 
            this.FlowPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FlowPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.FlowPanel.BackColor = System.Drawing.SystemColors.Control;
            this.FlowPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.FlowPanel.Location = new System.Drawing.Point(0, 35);
            this.FlowPanel.Margin = new System.Windows.Forms.Padding(0);
            this.FlowPanel.Name = "FlowPanel";
            this.FlowPanel.Padding = new System.Windows.Forms.Padding(5);
            this.FlowPanel.Size = new System.Drawing.Size(1002, 505);
            this.FlowPanel.TabIndex = 12;
            this.FlowPanel.WrapContents = false;
            // 
            // RepManagementForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1003, 541);
            this.Controls.Add(this.BorderPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "RepManagementForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ColorsEntry";
            this.BorderPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Panel BorderPanel;
        private FlowLayoutPanel FlowPanel;
        private Button DoneBtn;
        private Button CloseBtn;
    }
}