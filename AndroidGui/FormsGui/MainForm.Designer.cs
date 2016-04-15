namespace FormsGui
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
            this.canvas = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonMoveUp = new System.Windows.Forms.Button();
            this.buttonMoveDown = new System.Windows.Forms.Button();
            this.buttonMoveLeft = new System.Windows.Forms.Button();
            this.buttonMoveRight = new System.Windows.Forms.Button();
            this.rbShoot = new System.Windows.Forms.RadioButton();
            this.rbHamster = new System.Windows.Forms.RadioButton();
            this.rbHamsterSpray = new System.Windows.Forms.RadioButton();
            this.rbCement = new System.Windows.Forms.RadioButton();
            this.buttonDoNothing = new System.Windows.Forms.Button();
            this.statusPanel = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.textBoxMessages = new System.Windows.Forms.TextBox();
            this.bActionRight = new System.Windows.Forms.Button();
            this.bActionLeft = new System.Windows.Forms.Button();
            this.bActionDown = new System.Windows.Forms.Button();
            this.bActionUp = new System.Windows.Forms.Button();
            this.textBoxPlayerStatus = new System.Windows.Forms.TextBox();
            this.rbGrenade = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).BeginInit();
            this.panel1.SuspendLayout();
            this.statusPanel.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // canvas
            // 
            this.canvas.BackColor = System.Drawing.Color.Silver;
            this.canvas.Location = new System.Drawing.Point(13, 13);
            this.canvas.Name = "canvas";
            this.canvas.Size = new System.Drawing.Size(520, 418);
            this.canvas.TabIndex = 0;
            this.canvas.TabStop = false;
            this.canvas.Paint += new System.Windows.Forms.PaintEventHandler(this.canvas_Paint);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbGrenade);
            this.panel1.Controls.Add(this.bActionRight);
            this.panel1.Controls.Add(this.bActionLeft);
            this.panel1.Controls.Add(this.bActionDown);
            this.panel1.Controls.Add(this.bActionUp);
            this.panel1.Controls.Add(this.buttonDoNothing);
            this.panel1.Controls.Add(this.rbCement);
            this.panel1.Controls.Add(this.rbHamsterSpray);
            this.panel1.Controls.Add(this.rbHamster);
            this.panel1.Controls.Add(this.rbShoot);
            this.panel1.Controls.Add(this.buttonMoveRight);
            this.panel1.Controls.Add(this.buttonMoveLeft);
            this.panel1.Controls.Add(this.buttonMoveDown);
            this.panel1.Controls.Add(this.buttonMoveUp);
            this.panel1.Location = new System.Drawing.Point(539, 13);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(275, 418);
            this.panel1.TabIndex = 1;
            // 
            // buttonMoveUp
            // 
            this.buttonMoveUp.Location = new System.Drawing.Point(110, 20);
            this.buttonMoveUp.Name = "buttonMoveUp";
            this.buttonMoveUp.Size = new System.Drawing.Size(45, 50);
            this.buttonMoveUp.TabIndex = 0;
            this.buttonMoveUp.Text = "Move";
            this.buttonMoveUp.UseVisualStyleBackColor = true;
            this.buttonMoveUp.Click += new System.EventHandler(this.buttonMoveUp_Click);
            // 
            // buttonMoveDown
            // 
            this.buttonMoveDown.Location = new System.Drawing.Point(110, 112);
            this.buttonMoveDown.Name = "buttonMoveDown";
            this.buttonMoveDown.Size = new System.Drawing.Size(45, 50);
            this.buttonMoveDown.TabIndex = 1;
            this.buttonMoveDown.Text = "Move";
            this.buttonMoveDown.UseVisualStyleBackColor = true;
            this.buttonMoveDown.Click += new System.EventHandler(this.buttonMoveDown_Click);
            // 
            // buttonMoveLeft
            // 
            this.buttonMoveLeft.Location = new System.Drawing.Point(61, 67);
            this.buttonMoveLeft.Name = "buttonMoveLeft";
            this.buttonMoveLeft.Size = new System.Drawing.Size(50, 45);
            this.buttonMoveLeft.TabIndex = 2;
            this.buttonMoveLeft.Text = "Move";
            this.buttonMoveLeft.UseVisualStyleBackColor = true;
            this.buttonMoveLeft.Click += new System.EventHandler(this.buttonMoveLeft_Click);
            // 
            // buttonMoveRight
            // 
            this.buttonMoveRight.Location = new System.Drawing.Point(155, 67);
            this.buttonMoveRight.Name = "buttonMoveRight";
            this.buttonMoveRight.Size = new System.Drawing.Size(50, 45);
            this.buttonMoveRight.TabIndex = 3;
            this.buttonMoveRight.Text = "Move";
            this.buttonMoveRight.UseVisualStyleBackColor = true;
            this.buttonMoveRight.Click += new System.EventHandler(this.buttonMoveRight_Click);
            // 
            // rbShoot
            // 
            this.rbShoot.AutoSize = true;
            this.rbShoot.Location = new System.Drawing.Point(171, 273);
            this.rbShoot.Name = "rbShoot";
            this.rbShoot.Size = new System.Drawing.Size(53, 17);
            this.rbShoot.TabIndex = 4;
            this.rbShoot.TabStop = true;
            this.rbShoot.Text = "Shoot";
            this.rbShoot.UseVisualStyleBackColor = true;
            // 
            // rbHamster
            // 
            this.rbHamster.AutoSize = true;
            this.rbHamster.Location = new System.Drawing.Point(171, 319);
            this.rbHamster.Name = "rbHamster";
            this.rbHamster.Size = new System.Drawing.Size(64, 17);
            this.rbHamster.TabIndex = 5;
            this.rbHamster.TabStop = true;
            this.rbHamster.Text = "Hamster";
            this.rbHamster.UseVisualStyleBackColor = true;
            // 
            // rbHamsterSpray
            // 
            this.rbHamsterSpray.AutoSize = true;
            this.rbHamsterSpray.Location = new System.Drawing.Point(171, 342);
            this.rbHamsterSpray.Name = "rbHamsterSpray";
            this.rbHamsterSpray.Size = new System.Drawing.Size(92, 17);
            this.rbHamsterSpray.TabIndex = 6;
            this.rbHamsterSpray.TabStop = true;
            this.rbHamsterSpray.Text = "Hamster spray";
            this.rbHamsterSpray.UseVisualStyleBackColor = true;
            // 
            // rbCement
            // 
            this.rbCement.AutoSize = true;
            this.rbCement.Location = new System.Drawing.Point(171, 365);
            this.rbCement.Name = "rbCement";
            this.rbCement.Size = new System.Drawing.Size(61, 17);
            this.rbCement.TabIndex = 7;
            this.rbCement.TabStop = true;
            this.rbCement.Text = "Cement";
            this.rbCement.UseVisualStyleBackColor = true;
            // 
            // buttonDoNothing
            // 
            this.buttonDoNothing.Location = new System.Drawing.Point(96, 186);
            this.buttonDoNothing.Name = "buttonDoNothing";
            this.buttonDoNothing.Size = new System.Drawing.Size(75, 23);
            this.buttonDoNothing.TabIndex = 8;
            this.buttonDoNothing.Text = "Skip";
            this.buttonDoNothing.UseVisualStyleBackColor = true;
            this.buttonDoNothing.Click += new System.EventHandler(this.buttonDoNothing_Click);
            // 
            // statusPanel
            // 
            this.statusPanel.Controls.Add(this.textBoxPlayerStatus);
            this.statusPanel.Location = new System.Drawing.Point(540, 438);
            this.statusPanel.Name = "statusPanel";
            this.statusPanel.Size = new System.Drawing.Size(274, 214);
            this.statusPanel.TabIndex = 2;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.textBoxMessages);
            this.panel2.Location = new System.Drawing.Point(13, 438);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(520, 211);
            this.panel2.TabIndex = 3;
            this.panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
            // 
            // textBoxMessages
            // 
            this.textBoxMessages.BackColor = System.Drawing.Color.Black;
            this.textBoxMessages.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxMessages.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.textBoxMessages.Location = new System.Drawing.Point(3, 3);
            this.textBoxMessages.Multiline = true;
            this.textBoxMessages.Name = "textBoxMessages";
            this.textBoxMessages.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxMessages.Size = new System.Drawing.Size(514, 205);
            this.textBoxMessages.TabIndex = 0;
            // 
            // bActionRight
            // 
            this.bActionRight.Location = new System.Drawing.Point(97, 310);
            this.bActionRight.Name = "bActionRight";
            this.bActionRight.Size = new System.Drawing.Size(50, 45);
            this.bActionRight.TabIndex = 12;
            this.bActionRight.UseVisualStyleBackColor = true;
            this.bActionRight.Click += new System.EventHandler(this.bActionRight_Click);
            // 
            // bActionLeft
            // 
            this.bActionLeft.Location = new System.Drawing.Point(3, 310);
            this.bActionLeft.Name = "bActionLeft";
            this.bActionLeft.Size = new System.Drawing.Size(50, 45);
            this.bActionLeft.TabIndex = 11;
            this.bActionLeft.UseVisualStyleBackColor = true;
            this.bActionLeft.Click += new System.EventHandler(this.bActionLeft_Click);
            // 
            // bActionDown
            // 
            this.bActionDown.Location = new System.Drawing.Point(52, 355);
            this.bActionDown.Name = "bActionDown";
            this.bActionDown.Size = new System.Drawing.Size(45, 50);
            this.bActionDown.TabIndex = 10;
            this.bActionDown.UseVisualStyleBackColor = true;
            this.bActionDown.Click += new System.EventHandler(this.bActionDown_Click);
            // 
            // bActionUp
            // 
            this.bActionUp.Location = new System.Drawing.Point(52, 263);
            this.bActionUp.Name = "bActionUp";
            this.bActionUp.Size = new System.Drawing.Size(45, 50);
            this.bActionUp.TabIndex = 9;
            this.bActionUp.UseVisualStyleBackColor = true;
            this.bActionUp.Click += new System.EventHandler(this.bActionUp_Click);
            // 
            // textBoxPlayerStatus
            // 
            this.textBoxPlayerStatus.BackColor = System.Drawing.Color.Black;
            this.textBoxPlayerStatus.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxPlayerStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.textBoxPlayerStatus.Location = new System.Drawing.Point(3, 3);
            this.textBoxPlayerStatus.Multiline = true;
            this.textBoxPlayerStatus.Name = "textBoxPlayerStatus";
            this.textBoxPlayerStatus.Size = new System.Drawing.Size(268, 208);
            this.textBoxPlayerStatus.TabIndex = 0;
            this.textBoxPlayerStatus.Text = "Current player: \r\n\r\nAlive.\r\nCarries treasure.\r\nScore: 0\r\n\r\nArrows:\r\nGrenades:\r\nHa" +
    "msters:\r\nHamster spray:\r\nCement:\r\n\r\n";
            // 
            // rbGrenade
            // 
            this.rbGrenade.AutoSize = true;
            this.rbGrenade.Location = new System.Drawing.Point(171, 296);
            this.rbGrenade.Name = "rbGrenade";
            this.rbGrenade.Size = new System.Drawing.Size(66, 17);
            this.rbGrenade.TabIndex = 13;
            this.rbGrenade.TabStop = true;
            this.rbGrenade.Text = "Grenade";
            this.rbGrenade.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(817, 662);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.statusPanel);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.canvas);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "MainForm";
            this.Text = "Hamster Labyrinth";
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.statusPanel.ResumeLayout(false);
            this.statusPanel.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox canvas;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rbCement;
        private System.Windows.Forms.RadioButton rbHamsterSpray;
        private System.Windows.Forms.RadioButton rbHamster;
        private System.Windows.Forms.RadioButton rbShoot;
        private System.Windows.Forms.Button buttonMoveRight;
        private System.Windows.Forms.Button buttonMoveLeft;
        private System.Windows.Forms.Button buttonMoveDown;
        private System.Windows.Forms.Button buttonMoveUp;
        private System.Windows.Forms.Button buttonDoNothing;
        private System.Windows.Forms.Panel statusPanel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox textBoxMessages;
        private System.Windows.Forms.Button bActionRight;
        private System.Windows.Forms.Button bActionLeft;
        private System.Windows.Forms.Button bActionDown;
        private System.Windows.Forms.Button bActionUp;
        private System.Windows.Forms.TextBox textBoxPlayerStatus;
        private System.Windows.Forms.RadioButton rbGrenade;
    }
}

