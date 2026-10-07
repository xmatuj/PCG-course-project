namespace Mayorov_Matvey_PRI_123_course
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.AnT = new Tao.Platform.Windows.SimpleOpenGlControl();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.файлToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.выходToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.сценаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.векторнаяToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.растроваяToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.видToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.перспективнаяПроекцияToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ортогональнаяПроекцияToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.тениToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.настройкиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.освещениеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.анимацияToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.звукToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.справкаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.оПрограммеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton3D = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2DVec = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2DRast = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAnim = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonLight = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonExit = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabelMouseX = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabelMouseY = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabelFPS = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabelScene = new System.Windows.Forms.ToolStripLabel();
            this.RenderTimer = new System.Windows.Forms.Timer(this.components);
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // AnT
            // 
            this.AnT.AccumBits = ((byte)(0));
            this.AnT.AutoCheckErrors = false;
            this.AnT.AutoFinish = false;
            this.AnT.AutoMakeCurrent = true;
            this.AnT.AutoSwapBuffers = true;
            this.AnT.BackColor = System.Drawing.Color.Black;
            this.AnT.ColorBits = ((byte)(32));
            this.AnT.DepthBits = ((byte)(16));
            this.AnT.Location = new System.Drawing.Point(12, 27);
            this.AnT.Name = "AnT";
            this.AnT.Size = new System.Drawing.Size(1053, 642);
            this.AnT.StencilBits = ((byte)(0));
            this.AnT.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.файлToolStripMenuItem,
            this.сценаToolStripMenuItem,
            this.видToolStripMenuItem,
            this.настройкиToolStripMenuItem,
            this.справкаToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1264, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // файлToolStripMenuItem
            // 
            this.файлToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.выходToolStripMenuItem});
            this.файлToolStripMenuItem.Name = "файлToolStripMenuItem";
            this.файлToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.файлToolStripMenuItem.Text = "Файл";
            // 
            // выходToolStripMenuItem
            // 
            this.выходToolStripMenuItem.Name = "выходToolStripMenuItem";
            this.выходToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.выходToolStripMenuItem.Text = "Выход";
            this.выходToolStripMenuItem.Click += new System.EventHandler(this.выходToolStripMenuItem_Click);
            // 
            // сценаToolStripMenuItem
            // 
            this.сценаToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dToolStripMenuItem,
            this.dToolStripMenuItem1});
            this.сценаToolStripMenuItem.Name = "сценаToolStripMenuItem";
            this.сценаToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
            this.сценаToolStripMenuItem.Text = "Сцена";
            // 
            // dToolStripMenuItem
            // 
            this.dToolStripMenuItem.Name = "dToolStripMenuItem";
            this.dToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.dToolStripMenuItem.Text = "3D";
            this.dToolStripMenuItem.Click += new System.EventHandler(this.dToolStripMenuItem_Click);
            // 
            // dToolStripMenuItem1
            // 
            this.dToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.векторнаяToolStripMenuItem,
            this.растроваяToolStripMenuItem});
            this.dToolStripMenuItem1.Name = "dToolStripMenuItem1";
            this.dToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.dToolStripMenuItem1.Text = "2D";
            // 
            // векторнаяToolStripMenuItem
            // 
            this.векторнаяToolStripMenuItem.Name = "векторнаяToolStripMenuItem";
            this.векторнаяToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.векторнаяToolStripMenuItem.Text = "Векторная";
            this.векторнаяToolStripMenuItem.Click += new System.EventHandler(this.векторнаяToolStripMenuItem_Click);
            // 
            // растроваяToolStripMenuItem
            // 
            this.растроваяToolStripMenuItem.Name = "растроваяToolStripMenuItem";
            this.растроваяToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.растроваяToolStripMenuItem.Text = "Растровая";
            this.растроваяToolStripMenuItem.Click += new System.EventHandler(this.растроваяToolStripMenuItem_Click);
            // 
            // видToolStripMenuItem
            // 
            this.видToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.перспективнаяПроекцияToolStripMenuItem,
            this.ортогональнаяПроекцияToolStripMenuItem,
            this.тениToolStripMenuItem});
            this.видToolStripMenuItem.Name = "видToolStripMenuItem";
            this.видToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.видToolStripMenuItem.Text = "Вид";
            // 
            // перспективнаяПроекцияToolStripMenuItem
            // 
            this.перспективнаяПроекцияToolStripMenuItem.Name = "перспективнаяПроекцияToolStripMenuItem";
            this.перспективнаяПроекцияToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.перспективнаяПроекцияToolStripMenuItem.Text = "Перспективная проекция";
            // 
            // ортогональнаяПроекцияToolStripMenuItem
            // 
            this.ортогональнаяПроекцияToolStripMenuItem.Name = "ортогональнаяПроекцияToolStripMenuItem";
            this.ортогональнаяПроекцияToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.ортогональнаяПроекцияToolStripMenuItem.Text = "Ортогональная проекция";
            // 
            // тениToolStripMenuItem
            // 
            this.тениToolStripMenuItem.Name = "тениToolStripMenuItem";
            this.тениToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.тениToolStripMenuItem.Text = "Тени";
            // 
            // настройкиToolStripMenuItem
            // 
            this.настройкиToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.освещениеToolStripMenuItem,
            this.анимацияToolStripMenuItem,
            this.звукToolStripMenuItem});
            this.настройкиToolStripMenuItem.Name = "настройкиToolStripMenuItem";
            this.настройкиToolStripMenuItem.Size = new System.Drawing.Size(79, 20);
            this.настройкиToolStripMenuItem.Text = "Настройки";
            // 
            // освещениеToolStripMenuItem
            // 
            this.освещениеToolStripMenuItem.Name = "освещениеToolStripMenuItem";
            this.освещениеToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.освещениеToolStripMenuItem.Text = "Освещение";
            // 
            // анимацияToolStripMenuItem
            // 
            this.анимацияToolStripMenuItem.Name = "анимацияToolStripMenuItem";
            this.анимацияToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.анимацияToolStripMenuItem.Text = "Анимация";
            // 
            // звукToolStripMenuItem
            // 
            this.звукToolStripMenuItem.Name = "звукToolStripMenuItem";
            this.звукToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.звукToolStripMenuItem.Text = "Звук";
            // 
            // справкаToolStripMenuItem
            // 
            this.справкаToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.оПрограммеToolStripMenuItem});
            this.справкаToolStripMenuItem.Name = "справкаToolStripMenuItem";
            this.справкаToolStripMenuItem.Size = new System.Drawing.Size(65, 20);
            this.справкаToolStripMenuItem.Text = "Справка";
            // 
            // оПрограммеToolStripMenuItem
            // 
            this.оПрограммеToolStripMenuItem.Name = "оПрограммеToolStripMenuItem";
            this.оПрограммеToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.оПрограммеToolStripMenuItem.Text = "О программе";
            this.оПрограммеToolStripMenuItem.Click += new System.EventHandler(this.оПрограммеToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Right;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton3D,
            this.toolStripButton2DVec,
            this.toolStripButton2DRast,
            this.toolStripButtonAnim,
            this.toolStripButtonLight,
            this.toolStripButtonExit,
            this.toolStripSeparator1,
            this.toolStripLabelMouseX,
            this.toolStripLabelMouseY,
            this.toolStripLabelFPS,
            this.toolStripLabelScene});
            this.toolStrip1.Location = new System.Drawing.Point(1068, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(196, 657);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton3D
            // 
            this.toolStripButton3D.AutoSize = false;
            this.toolStripButton3D.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3D.Image")));
            this.toolStripButton3D.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolStripButton3D.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3D.Name = "toolStripButton3D";
            this.toolStripButton3D.Size = new System.Drawing.Size(96, 32);
            this.toolStripButton3D.Text = "3D";
            this.toolStripButton3D.Click += new System.EventHandler(this.toolStripButton3D_Click);
            // 
            // toolStripButton2DVec
            // 
            this.toolStripButton2DVec.AutoSize = false;
            this.toolStripButton2DVec.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2DVec.Image")));
            this.toolStripButton2DVec.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolStripButton2DVec.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2DVec.Name = "toolStripButton2DVec";
            this.toolStripButton2DVec.Size = new System.Drawing.Size(96, 32);
            this.toolStripButton2DVec.Text = "2D-вектор";
            this.toolStripButton2DVec.Click += new System.EventHandler(this.toolStripButton2DVec_Click);
            // 
            // toolStripButton2DRast
            // 
            this.toolStripButton2DRast.AutoSize = false;
            this.toolStripButton2DRast.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2DRast.Image")));
            this.toolStripButton2DRast.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolStripButton2DRast.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2DRast.Name = "toolStripButton2DRast";
            this.toolStripButton2DRast.Size = new System.Drawing.Size(96, 32);
            this.toolStripButton2DRast.Text = "2D-растр";
            this.toolStripButton2DRast.Click += new System.EventHandler(this.toolStripButton2DRast_Click);
            // 
            // toolStripButtonAnim
            // 
            this.toolStripButtonAnim.AutoSize = false;
            this.toolStripButtonAnim.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAnim.Image")));
            this.toolStripButtonAnim.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolStripButtonAnim.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAnim.Name = "toolStripButtonAnim";
            this.toolStripButtonAnim.Size = new System.Drawing.Size(96, 32);
            this.toolStripButtonAnim.Text = "Анимация";
            this.toolStripButtonAnim.Click += new System.EventHandler(this.toolStripButtonAnim_Click);
            // 
            // toolStripButtonLight
            // 
            this.toolStripButtonLight.AutoSize = false;
            this.toolStripButtonLight.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonLight.Image")));
            this.toolStripButtonLight.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolStripButtonLight.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonLight.Name = "toolStripButtonLight";
            this.toolStripButtonLight.Size = new System.Drawing.Size(96, 32);
            this.toolStripButtonLight.Text = "Свет";
            this.toolStripButtonLight.Click += new System.EventHandler(this.toolStripButtonLight_Click);
            // 
            // toolStripButtonExit
            // 
            this.toolStripButtonExit.AutoSize = false;
            this.toolStripButtonExit.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonExit.Image")));
            this.toolStripButtonExit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolStripButtonExit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonExit.Name = "toolStripButtonExit";
            this.toolStripButtonExit.Size = new System.Drawing.Size(96, 32);
            this.toolStripButtonExit.Text = "Выход";
            this.toolStripButtonExit.Click += new System.EventHandler(this.toolStripButtonExit_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(194, 6);
            // 
            // toolStripLabelMouseX
            // 
            this.toolStripLabelMouseX.Name = "toolStripLabelMouseX";
            this.toolStripLabelMouseX.Size = new System.Drawing.Size(194, 15);
            this.toolStripLabelMouseX.Text = "Mouse X";
            // 
            // toolStripLabelMouseY
            // 
            this.toolStripLabelMouseY.Name = "toolStripLabelMouseY";
            this.toolStripLabelMouseY.Size = new System.Drawing.Size(194, 15);
            this.toolStripLabelMouseY.Text = "Mouse Y";
            // 
            // toolStripLabelFPS
            // 
            this.toolStripLabelFPS.Name = "toolStripLabelFPS";
            this.toolStripLabelFPS.Size = new System.Drawing.Size(194, 15);
            this.toolStripLabelFPS.Text = "FPS";
            // 
            // toolStripLabelScene
            // 
            this.toolStripLabelScene.Name = "toolStripLabelScene";
            this.toolStripLabelScene.Size = new System.Drawing.Size(194, 15);
            this.toolStripLabelScene.Text = "Текущая сцена";
            // 
            // RenderTimer
            // 
            this.RenderTimer.Interval = 16;
            this.RenderTimer.Tick += new System.EventHandler(this.RenderTimer_Tick);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.AnT);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Mayorov_matvey_PRI123_course";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Tao.Platform.Windows.SimpleOpenGlControl AnT;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem файлToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem сценаToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem видToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem настройкиToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem справкаToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem оПрограммеToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem выходToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem векторнаяToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem растроваяToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem перспективнаяПроекцияToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ортогональнаяПроекцияToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem тениToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem освещениеToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem анимацияToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem звукToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton3D;
        private System.Windows.Forms.ToolStripButton toolStripButton2DVec;
        private System.Windows.Forms.ToolStripButton toolStripButton2DRast;
        private System.Windows.Forms.ToolStripButton toolStripButtonAnim;
        private System.Windows.Forms.ToolStripButton toolStripButtonLight;
        private System.Windows.Forms.ToolStripButton toolStripButtonExit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabelMouseX;
        private System.Windows.Forms.ToolStripLabel toolStripLabelMouseY;
        private System.Windows.Forms.ToolStripLabel toolStripLabelFPS;
        private System.Windows.Forms.ToolStripLabel toolStripLabelScene;
        private System.Windows.Forms.Timer RenderTimer;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}