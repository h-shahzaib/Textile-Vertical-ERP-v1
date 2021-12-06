using Main.Resources.Forms.ProgramEntry.OTHERS;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ShahzaibEMB.Resources.Forms.ProgramEntry.OTHERS
{
    partial class SubEntryForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SubEntryForm));
            this.BorderPanel = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.CloseBtn = new System.Windows.Forms.Button();
            this.DoneBtn = new System.Windows.Forms.Button();
            this.ExtrasPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.FlowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.BorderPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // BorderPanel
            // 
            this.BorderPanel.AutoSize = true;
            this.BorderPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BorderPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.BorderPanel.Controls.Add(this.label2);
            this.BorderPanel.Controls.Add(this.label1);
            this.BorderPanel.Controls.Add(this.CloseBtn);
            this.BorderPanel.Controls.Add(this.DoneBtn);
            this.BorderPanel.Controls.Add(this.ExtrasPanel);
            this.BorderPanel.Controls.Add(this.FlowPanel);
            this.BorderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BorderPanel.Location = new System.Drawing.Point(0, 0);
            this.BorderPanel.Margin = new System.Windows.Forms.Padding(2);
            this.BorderPanel.Name = "BorderPanel";
            this.BorderPanel.Size = new System.Drawing.Size(152, 77);
            this.BorderPanel.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(3, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "Height must be in \'mm\'";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(143, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "Dont forget design properties";
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
            this.CloseBtn.Location = new System.Drawing.Point(120, 6);
            this.CloseBtn.Margin = new System.Windows.Forms.Padding(2);
            this.CloseBtn.Name = "CloseBtn";
            this.CloseBtn.Size = new System.Drawing.Size(25, 26);
            this.CloseBtn.TabIndex = 16;
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
            this.DoneBtn.Location = new System.Drawing.Point(92, 6);
            this.DoneBtn.Margin = new System.Windows.Forms.Padding(2);
            this.DoneBtn.Name = "DoneBtn";
            this.DoneBtn.Size = new System.Drawing.Size(25, 26);
            this.DoneBtn.TabIndex = 15;
            this.DoneBtn.UseVisualStyleBackColor = false;
            // 
            // ExtrasPanel
            // 
            this.ExtrasPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ExtrasPanel.BackColor = System.Drawing.Color.DarkOrange;
            this.ExtrasPanel.Location = new System.Drawing.Point(0, 40);
            this.ExtrasPanel.Margin = new System.Windows.Forms.Padding(2);
            this.ExtrasPanel.Name = "ExtrasPanel";
            this.ExtrasPanel.Size = new System.Drawing.Size(150, 25);
            this.ExtrasPanel.TabIndex = 1;
            // 
            // FlowPanel
            // 
            this.FlowPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FlowPanel.AutoSize = true;
            this.FlowPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.FlowPanel.BackColor = System.Drawing.SystemColors.Control;
            this.FlowPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.FlowPanel.Location = new System.Drawing.Point(0, 66);
            this.FlowPanel.Margin = new System.Windows.Forms.Padding(0);
            this.FlowPanel.Name = "FlowPanel";
            this.FlowPanel.Padding = new System.Windows.Forms.Padding(5);
            this.FlowPanel.Size = new System.Drawing.Size(10, 10);
            this.FlowPanel.TabIndex = 12;
            this.FlowPanel.WrapContents = false;
            // 
            // SubEntryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(152, 77);
            this.Controls.Add(this.BorderPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "SubEntryForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ColorsEntry";
            this.BorderPanel.ResumeLayout(false);
            this.BorderPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Panel BorderPanel;
        private FlowLayoutPanel FlowPanel;
        private FlowLayoutPanel ExtrasPanel;
        private Button CloseBtn;
        private Button DoneBtn;
        private Label label1;
        private Label label2;
    }
}