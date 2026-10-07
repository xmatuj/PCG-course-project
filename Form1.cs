using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tao.OpenGl;
using Tao.FreeGlut;
using Tao.Platform.Windows;

namespace Mayorov_Matvey_PRI_123_course
{
    public partial class Form1 : Form
    {
        // Камера 
        private float camX = 0.0f;
        private float camY = 5.0f;
        private float camZ = 15.0f;
        private float camAngleX = 0.0f;
        private float camAngleY = 0.0f;

        // Мышь 
        private float Mcoord_X = 0.0f;
        private float Mcoord_Y = 0.0f;
        private bool isMouseDown = false;

        // Сцена 
        private int currentScene = 0;   // 0 — 3D, 1 — 2D-векторная, 2 — 2D-растровая
        private readonly string[] sceneNames = { "3D", "2D-векторная", "2D-растровая" };

        // Флаги режимов 
        private bool animationEnabled = true;
        private bool lightingEnabled = true;
        private bool soundEnabled = true;

        // Таймер и FPS 
        private int frameCount = 0;
        private int fps = 0;

        // Многопоточность 
        private object locker = new object();
        private Thread physicsThread;
        private bool physicsRunning = false;

        // Инициализация OpenGL 
        private bool glInitialized = false;

        public Form1()
        {
            InitializeComponent();
            AnT.InitializeContexts();

            // Подписываемся на события мыши вручную
            AnT.MouseMove += AnT_MouseMove;
            AnT.MouseDown += AnT_MouseDown;
            AnT.MouseUp += AnT_MouseUp;
        }

        
        // ЗАГРУЗКА ФОРМЫ
        
        private void Form1_Load(object sender, EventArgs e)
        {
            // Инициализация GLUT
            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_RGB | Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH);

            // Цвет очистки — светло-голубое небо
            Gl.glClearColor(0.5f, 0.7f, 1.0f, 1.0f);

            // Порт вывода по размерам AnT
            Gl.glViewport(0, 0, AnT.Width, AnT.Height);

            // Перспективная проекция
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluPerspective(45.0, (double)AnT.Width / AnT.Height, 0.1, 200.0);

            // Объектно-видовая матрица
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();

            // Включаем тест глубины и освещение
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glEnable(Gl.GL_LIGHT0);
            Gl.glEnable(Gl.GL_COLOR_MATERIAL);

            glInitialized = true;

            // Старт таймера
            RenderTimer.Start();
        }

        
        // ТАЙМЕР
        
        private void RenderTimer_Tick(object sender, EventArgs e)
        {
            // Счётчик кадров
            frameCount++;

            // Раз в ~60 кадров обновляем FPS
            if (frameCount >= 60)
            {
                fps = frameCount;
                frameCount = 0;
                toolStripLabelFPS.Text = $"FPS: {fps}";
            }

            // Отрисовка кадра
            Draw();

            // Обновляем метку текущей сцены
            toolStripLabelScene.Text = $"Сцена: {sceneNames[currentScene]}";
        }


        // ОТРИСОВКА

        private void Draw()
        {
            if (!glInitialized) return;

            // Очистка буферов
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            Gl.glLoadIdentity();

            // Камера
            Glu.gluLookAt(camX, camY, camZ, 0, 0, 0, 0, 1, 0);

            // Управление освещением по флагу
            if (lightingEnabled)
            {
                Gl.glEnable(Gl.GL_LIGHTING);
                Gl.glEnable(Gl.GL_LIGHT0);

                // Позиция источника света
                float[] lightPos = { 10.0f, 20.0f, 10.0f, 1.0f };
                Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, lightPos);
            }
            else
            {
                Gl.glDisable(Gl.GL_LIGHTING);
                Gl.glDisable(Gl.GL_LIGHT0);
            }

            // В зависимости от текущей сцены
            if (currentScene == 0)
            {
                DrawTestCube();          // заглушка 3D-сцены
            }
            else if (currentScene == 1)
            {
                Draw2DVectorScene();     // заглушка 2D-векторной сцены
            }
            // currentScene == 2 — 2D-растровая сцена будет реализована на следующем этапе

            Gl.glFlush();
            AnT.Invalidate();
        }

        // Заглушка 3D-сцены — тестовый куб
        private void DrawTestCube()
        {
            Gl.glPushMatrix();
            Gl.glColor3f(0.0f, 0.6f, 0.0f);
            Glut.glutSolidCube(2.0f);
            Gl.glPopMatrix();
        }

        // 2D-векторная сцена — надпись "test"
        private void Draw2DVectorScene()
        {
            // Переключаемся на ортогональную проекцию
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glPushMatrix();
            Gl.glLoadIdentity();
            Glu.gluOrtho2D(0, AnT.Width, 0, AnT.Height);

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPushMatrix();
            Gl.glLoadIdentity();

            // Отключаем свет, чтобы цвет текста не искажался
            Gl.glDisable(Gl.GL_LIGHTING);

            // Цвет — тёмно-зелёный (вариант 10)
            Gl.glColor3f(0.0f, 0.39f, 0.0f);

            // Позиция текста — центр экрана
            Gl.glRasterPos2f(AnT.Width / 2.0f - 30, AnT.Height / 2.0f);

            // Вывод строки посимвольно через GLUT
            string text = "test";
            foreach (char c in text)
            {
                Glut.glutBitmapCharacter(Glut.GLUT_BITMAP_HELVETICA_18, c);
            }

            // Возвращаем свет
            Gl.glEnable(Gl.GL_LIGHTING);

            Gl.glPopMatrix();

            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glPopMatrix();

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
        }

        
        // МЫШЬ
        
        private void AnT_MouseMove(object sender, MouseEventArgs e)
        {
            Mcoord_X = e.X;
            Mcoord_Y = e.Y;

            toolStripLabelMouseX.Text = $"Mouse X: {Mcoord_X:F0}";
            toolStripLabelMouseY.Text = $"Mouse Y: {Mcoord_Y:F0}";
        }

        private void AnT_MouseDown(object sender, MouseEventArgs e)
        {
            isMouseDown = true;
        }

        private void AnT_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
        }

        
        // МЕНЮ
        
        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Курсовой проект\n" +
                "Тема: Воссоздание сцены из мультфильма «Где обедал воробей»\n" +
                "Автор: Майоров М. С., гр. ПРИ-123\n" +
                "Дисциплина: Программирование компьютерной графики\n" +
                "ВлГУ, 2026",
                "О программе",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        // Переключение сцен через меню
        private void dToolStripMenuItem_Click(object sender, EventArgs e)          // 3D
        {
            currentScene = 0;
        }

        private void векторнаяToolStripMenuItem_Click(object sender, EventArgs e)  // 2D-векторная
        {
            currentScene = 1;
        }

        private void растроваяToolStripMenuItem_Click(object sender, EventArgs e)  // 2D-растровая
        {
            currentScene = 2;
        }

        
        // КНОПКИ TOOLSTRIP
        
        private void toolStripButton3D_Click(object sender, EventArgs e)
        {
            currentScene = 0;
        }

        private void toolStripButton2DVec_Click(object sender, EventArgs e)
        {
            currentScene = 1;
        }

        private void toolStripButton2DRast_Click(object sender, EventArgs e)
        {
            currentScene = 2;
        }

        private void toolStripButtonAnim_Click(object sender, EventArgs e)
        {
            animationEnabled = !animationEnabled;
        }

        private void toolStripButtonLight_Click(object sender, EventArgs e)
        {
            lightingEnabled = !lightingEnabled;
        }

        private void toolStripButtonExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}