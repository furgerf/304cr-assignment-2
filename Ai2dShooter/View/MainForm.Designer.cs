namespace Ai2dShooter.View
{
    partial class MainForm
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
            this._canvas = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // _canvas
            // 
            this._canvas.BackColor = System.Drawing.Color.White;
            this._canvas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._canvas.Location = new System.Drawing.Point(12, 12);
            this._canvas.Name = "_canvas";
            this._canvas.Size = new System.Drawing.Size(962, 482);
            this._canvas.TabIndex = 4;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1062, 643);
            this.Controls.Add(this._canvas);
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.Name = "MainForm";
            this.Text = "2D AI Shooter Game";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel _canvas;

    }
}

