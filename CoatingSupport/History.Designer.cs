namespace CoatingSupport
{
    partial class frmHistory
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
            this.txtHistories = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtHistories
            // 
            this.txtHistories.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtHistories.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHistories.Location = new System.Drawing.Point(20, 60);
            this.txtHistories.Multiline = true;
            this.txtHistories.Name = "txtHistories";
            this.txtHistories.ReadOnly = true;
            this.txtHistories.Size = new System.Drawing.Size(719, 472);
            this.txtHistories.TabIndex = 0;
            // 
            // frmHistory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(759, 552);
            this.Controls.Add(this.txtHistories);
            this.Name = "frmHistory";
            this.Text = "Histories";
            this.Load += new System.EventHandler(this.frmHistory_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtHistories;
    }
}