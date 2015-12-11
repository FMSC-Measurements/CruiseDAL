namespace FMSCORM.CodeGenEngine
{
    partial class Test
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
            this._start_BTN = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _start_BTN
            // 
            this._start_BTN.Location = new System.Drawing.Point(13, 13);
            this._start_BTN.Name = "_start_BTN";
            this._start_BTN.Size = new System.Drawing.Size(75, 23);
            this._start_BTN.TabIndex = 0;
            this._start_BTN.Text = "Start";
            this._start_BTN.UseVisualStyleBackColor = true;
            this._start_BTN.Click += new System.EventHandler(this._start_BTN_Click);
            // 
            // Test
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(598, 534);
            this.Controls.Add(this._start_BTN);
            this.Name = "Test";
            this.Text = "Test";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _start_BTN;
    }
}