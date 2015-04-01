namespace Ai2dShooter.View
{
    partial class PlayerControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.grpName = new System.Windows.Forms.GroupBox();
            this.txtHealthyThreshold = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtAccuracy = new System.Windows.Forms.TextBox();
            this.txtDamage = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.progressHealth = new System.Windows.Forms.ProgressBar();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtSlowness = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtKills = new System.Windows.Forms.TextBox();
            this.grpName.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpName
            // 
            this.grpName.Controls.Add(this.label5);
            this.grpName.Controls.Add(this.txtKills);
            this.grpName.Controls.Add(this.label4);
            this.grpName.Controls.Add(this.txtSlowness);
            this.grpName.Controls.Add(this.txtHealthyThreshold);
            this.grpName.Controls.Add(this.label3);
            this.grpName.Controls.Add(this.txtAccuracy);
            this.grpName.Controls.Add(this.txtDamage);
            this.grpName.Controls.Add(this.label1);
            this.grpName.Controls.Add(this.progressHealth);
            this.grpName.Controls.Add(this.label2);
            this.grpName.Location = new System.Drawing.Point(6, 3);
            this.grpName.Name = "grpName";
            this.grpName.Size = new System.Drawing.Size(163, 181);
            this.grpName.TabIndex = 0;
            this.grpName.TabStop = false;
            // 
            // txtHealthyThreshold
            // 
            this.txtHealthyThreshold.Enabled = false;
            this.txtHealthyThreshold.Location = new System.Drawing.Point(120, 48);
            this.txtHealthyThreshold.Name = "txtHealthyThreshold";
            this.txtHealthyThreshold.Size = new System.Drawing.Size(37, 20);
            this.txtHealthyThreshold.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 103);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Accuracy/Headshot:";
            // 
            // txtAccuracy
            // 
            this.txtAccuracy.Enabled = false;
            this.txtAccuracy.Location = new System.Drawing.Point(120, 100);
            this.txtAccuracy.Name = "txtAccuracy";
            this.txtAccuracy.Size = new System.Drawing.Size(37, 20);
            this.txtAccuracy.TabIndex = 6;
            // 
            // txtDamage
            // 
            this.txtDamage.Enabled = false;
            this.txtDamage.Location = new System.Drawing.Point(120, 74);
            this.txtDamage.Name = "txtDamage";
            this.txtDamage.Size = new System.Drawing.Size(37, 20);
            this.txtDamage.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 77);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Damage Front/Back:";
            // 
            // progressHealth
            // 
            this.progressHealth.Location = new System.Drawing.Point(6, 19);
            this.progressHealth.Name = "progressHealth";
            this.progressHealth.Size = new System.Drawing.Size(151, 23);
            this.progressHealth.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Healthy Threshold:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 129);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Slowness:";
            // 
            // txtSlowness
            // 
            this.txtSlowness.Enabled = false;
            this.txtSlowness.Location = new System.Drawing.Point(120, 126);
            this.txtSlowness.Name = "txtSlowness";
            this.txtSlowness.Size = new System.Drawing.Size(37, 20);
            this.txtSlowness.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 155);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(28, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Kills:";
            // 
            // txtKills
            // 
            this.txtKills.Enabled = false;
            this.txtKills.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtKills.Location = new System.Drawing.Point(120, 152);
            this.txtKills.Name = "txtKills";
            this.txtKills.Size = new System.Drawing.Size(37, 22);
            this.txtKills.TabIndex = 11;
            // 
            // PlayerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpName);
            this.Name = "PlayerControl";
            this.Size = new System.Drawing.Size(174, 193);
            this.grpName.ResumeLayout(false);
            this.grpName.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpName;
        private System.Windows.Forms.TextBox txtHealthyThreshold;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtAccuracy;
        private System.Windows.Forms.TextBox txtDamage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar progressHealth;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtKills;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtSlowness;
    }
}
