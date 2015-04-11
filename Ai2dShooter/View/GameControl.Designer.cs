namespace Ai2dShooter.View
{
    partial class GameControl
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
            this.grpTeamHot = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comPlayerControllerHot = new System.Windows.Forms.ComboBox();
            this.numPlayerCountHot = new System.Windows.Forms.NumericUpDown();
            this.grpTeamCold = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comPlayerControllerCold = new System.Windows.Forms.ComboBox();
            this.numPlayerCountCold = new System.Windows.Forms.NumericUpDown();
            this.chkSoundEffects = new System.Windows.Forms.CheckBox();
            this.grpTeamHot.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPlayerCountHot)).BeginInit();
            this.grpTeamCold.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPlayerCountCold)).BeginInit();
            this.SuspendLayout();
            // 
            // grpTeamHot
            // 
            this.grpTeamHot.Controls.Add(this.label1);
            this.grpTeamHot.Controls.Add(this.comPlayerControllerHot);
            this.grpTeamHot.Controls.Add(this.numPlayerCountHot);
            this.grpTeamHot.Location = new System.Drawing.Point(3, 3);
            this.grpTeamHot.Name = "grpTeamHot";
            this.grpTeamHot.Size = new System.Drawing.Size(102, 75);
            this.grpTeamHot.TabIndex = 0;
            this.grpTeamHot.TabStop = false;
            this.grpTeamHot.Text = "Team Hot";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Players:";
            // 
            // comPlayerControllerHot
            // 
            this.comPlayerControllerHot.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comPlayerControllerHot.FormattingEnabled = true;
            this.comPlayerControllerHot.Location = new System.Drawing.Point(6, 19);
            this.comPlayerControllerHot.Name = "comPlayerControllerHot";
            this.comPlayerControllerHot.Size = new System.Drawing.Size(89, 21);
            this.comPlayerControllerHot.TabIndex = 1;
            // 
            // numPlayerCountHot
            // 
            this.numPlayerCountHot.Location = new System.Drawing.Point(56, 46);
            this.numPlayerCountHot.Maximum = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.numPlayerCountHot.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numPlayerCountHot.Name = "numPlayerCountHot";
            this.numPlayerCountHot.Size = new System.Drawing.Size(39, 20);
            this.numPlayerCountHot.TabIndex = 0;
            this.numPlayerCountHot.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // grpTeamCold
            // 
            this.grpTeamCold.Controls.Add(this.label2);
            this.grpTeamCold.Controls.Add(this.comPlayerControllerCold);
            this.grpTeamCold.Controls.Add(this.numPlayerCountCold);
            this.grpTeamCold.Location = new System.Drawing.Point(111, 3);
            this.grpTeamCold.Name = "grpTeamCold";
            this.grpTeamCold.Size = new System.Drawing.Size(102, 75);
            this.grpTeamCold.TabIndex = 3;
            this.grpTeamCold.TabStop = false;
            this.grpTeamCold.Text = "Team Cold";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Players:";
            // 
            // comPlayerControllerCold
            // 
            this.comPlayerControllerCold.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comPlayerControllerCold.FormattingEnabled = true;
            this.comPlayerControllerCold.Location = new System.Drawing.Point(6, 19);
            this.comPlayerControllerCold.Name = "comPlayerControllerCold";
            this.comPlayerControllerCold.Size = new System.Drawing.Size(89, 21);
            this.comPlayerControllerCold.TabIndex = 1;
            // 
            // numPlayerCountCold
            // 
            this.numPlayerCountCold.Location = new System.Drawing.Point(56, 46);
            this.numPlayerCountCold.Maximum = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.numPlayerCountCold.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numPlayerCountCold.Name = "numPlayerCountCold";
            this.numPlayerCountCold.Size = new System.Drawing.Size(39, 20);
            this.numPlayerCountCold.TabIndex = 0;
            this.numPlayerCountCold.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // chkSoundEffects
            // 
            this.chkSoundEffects.AutoSize = true;
            this.chkSoundEffects.Checked = true;
            this.chkSoundEffects.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSoundEffects.Location = new System.Drawing.Point(3, 84);
            this.chkSoundEffects.Name = "chkSoundEffects";
            this.chkSoundEffects.Size = new System.Drawing.Size(93, 17);
            this.chkSoundEffects.TabIndex = 4;
            this.chkSoundEffects.Text = "Sound Effects";
            this.chkSoundEffects.UseVisualStyleBackColor = true;
            // 
            // GameControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkSoundEffects);
            this.Controls.Add(this.grpTeamCold);
            this.Controls.Add(this.grpTeamHot);
            this.Name = "GameControl";
            this.Size = new System.Drawing.Size(216, 107);
            this.grpTeamHot.ResumeLayout(false);
            this.grpTeamHot.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPlayerCountHot)).EndInit();
            this.grpTeamCold.ResumeLayout(false);
            this.grpTeamCold.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPlayerCountCold)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpTeamHot;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comPlayerControllerHot;
        private System.Windows.Forms.NumericUpDown numPlayerCountHot;
        private System.Windows.Forms.GroupBox grpTeamCold;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comPlayerControllerCold;
        private System.Windows.Forms.NumericUpDown numPlayerCountCold;
        private System.Windows.Forms.CheckBox chkSoundEffects;
    }
}
