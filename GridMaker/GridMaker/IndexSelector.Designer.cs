
namespace GridMaker
{
    partial class IndexSelector
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.TLP = new System.Windows.Forms.TableLayoutPanel();
            this.BtnIncludeAll = new System.Windows.Forms.Button();
            this.BtnSkipAll = new System.Windows.Forms.Button();
            this.BtnConfirm = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.TLP.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox
            // 
            this.pictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.TLP.SetColumnSpan(this.pictureBox, 3);
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox.Location = new System.Drawing.Point(4, 4);
            this.pictureBox.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(507, 467);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            // 
            // TLP
            // 
            this.TLP.ColumnCount = 3;
            this.TLP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.TLP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.TLP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.TLP.Controls.Add(this.BtnIncludeAll, 1, 1);
            this.TLP.Controls.Add(this.BtnSkipAll, 0, 1);
            this.TLP.Controls.Add(this.pictureBox, 0, 0);
            this.TLP.Controls.Add(this.BtnConfirm, 2, 1);
            this.TLP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TLP.Location = new System.Drawing.Point(0, 0);
            this.TLP.Margin = new System.Windows.Forms.Padding(4);
            this.TLP.Name = "TLP";
            this.TLP.RowCount = 2;
            this.TLP.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TLP.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TLP.Size = new System.Drawing.Size(515, 511);
            this.TLP.TabIndex = 1;
            // 
            // BtnIncludeAll
            // 
            this.BtnIncludeAll.BackColor = System.Drawing.Color.White;
            this.BtnIncludeAll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BtnIncludeAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnIncludeAll.Location = new System.Drawing.Point(175, 479);
            this.BtnIncludeAll.Margin = new System.Windows.Forms.Padding(4);
            this.BtnIncludeAll.Name = "BtnIncludeAll";
            this.BtnIncludeAll.Size = new System.Drawing.Size(163, 28);
            this.BtnIncludeAll.TabIndex = 3;
            this.BtnIncludeAll.Text = "Include All";
            this.BtnIncludeAll.UseVisualStyleBackColor = false;
            this.BtnIncludeAll.Click += new System.EventHandler(this.BtnIncludeAll_Click);
            // 
            // BtnSkipAll
            // 
            this.BtnSkipAll.BackColor = System.Drawing.Color.White;
            this.BtnSkipAll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BtnSkipAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnSkipAll.Location = new System.Drawing.Point(4, 479);
            this.BtnSkipAll.Margin = new System.Windows.Forms.Padding(4);
            this.BtnSkipAll.Name = "BtnSkipAll";
            this.BtnSkipAll.Size = new System.Drawing.Size(163, 28);
            this.BtnSkipAll.TabIndex = 2;
            this.BtnSkipAll.Text = "Skip All";
            this.BtnSkipAll.UseVisualStyleBackColor = false;
            this.BtnSkipAll.Click += new System.EventHandler(this.BtnSkipAll_Click);
            // 
            // BtnConfirm
            // 
            this.BtnConfirm.BackColor = System.Drawing.Color.LightGreen;
            this.BtnConfirm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BtnConfirm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnConfirm.Location = new System.Drawing.Point(346, 479);
            this.BtnConfirm.Margin = new System.Windows.Forms.Padding(4);
            this.BtnConfirm.Name = "BtnConfirm";
            this.BtnConfirm.Size = new System.Drawing.Size(165, 28);
            this.BtnConfirm.TabIndex = 1;
            this.BtnConfirm.Text = "Confirm";
            this.BtnConfirm.UseVisualStyleBackColor = false;
            this.BtnConfirm.Click += new System.EventHandler(this.BtnConfirm_Click);
            // 
            // IndexSelector
            // 
            this.AcceptButton = this.BtnConfirm;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(515, 511);
            this.Controls.Add(this.TLP);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "IndexSelector";
            this.Text = "Index Selector";
            this.Load += new System.EventHandler(this.IndexSelector_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.TLP.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.TableLayoutPanel TLP;
        private System.Windows.Forms.Button BtnConfirm;
        private System.Windows.Forms.Button BtnIncludeAll;
        private System.Windows.Forms.Button BtnSkipAll;
    }
}

