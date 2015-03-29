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
            this.playerControl1 = new Ai2dShooter.View.PlayerControl();
            this.playerControl2 = new Ai2dShooter.View.PlayerControl();
            this.playerControl3 = new Ai2dShooter.View.PlayerControl();
            this.playerControl4 = new Ai2dShooter.View.PlayerControl();
            this._canvas = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // playerControl1
            // 
            this.playerControl1.Location = new System.Drawing.Point(12, 12);
            this.playerControl1.Name = "playerControl1";
            this.playerControl1.Player = null;
            this.playerControl1.Size = new System.Drawing.Size(240, 136);
            this.playerControl1.TabIndex = 0;
            // 
            // playerControl2
            // 
            this.playerControl2.Location = new System.Drawing.Point(258, 12);
            this.playerControl2.Name = "playerControl2";
            this.playerControl2.Player = null;
            this.playerControl2.Size = new System.Drawing.Size(240, 136);
            this.playerControl2.TabIndex = 1;
            // 
            // playerControl3
            // 
            this.playerControl3.Location = new System.Drawing.Point(504, 12);
            this.playerControl3.Name = "playerControl3";
            this.playerControl3.Player = null;
            this.playerControl3.Size = new System.Drawing.Size(240, 136);
            this.playerControl3.TabIndex = 2;
            // 
            // playerControl4
            // 
            this.playerControl4.Location = new System.Drawing.Point(750, 12);
            this.playerControl4.Name = "playerControl4";
            this.playerControl4.Player = null;
            this.playerControl4.Size = new System.Drawing.Size(240, 136);
            this.playerControl4.TabIndex = 3;
            // 
            // _canvas
            // 
            this._canvas.BackColor = System.Drawing.Color.White;
            this._canvas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._canvas.Location = new System.Drawing.Point(12, 154);
            this._canvas.Name = "_canvas";
            this._canvas.Size = new System.Drawing.Size(978, 461);
            this._canvas.TabIndex = 4;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1029, 627);
            this.Controls.Add(this._canvas);
            this.Controls.Add(this.playerControl4);
            this.Controls.Add(this.playerControl3);
            this.Controls.Add(this.playerControl2);
            this.Controls.Add(this.playerControl1);
            this.Name = "MainForm";
            this.Text = "2D AI Shooter Thing";
            this.ResumeLayout(false);

        }

        #endregion

        private PlayerControl playerControl1;
        private PlayerControl playerControl2;
        private PlayerControl playerControl3;
        private PlayerControl playerControl4;
        private System.Windows.Forms.Panel _canvas;

    }
}

